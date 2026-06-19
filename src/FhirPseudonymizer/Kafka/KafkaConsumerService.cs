using System.Text.RegularExpressions;
using System.Threading.Channels;
using Confluent.Kafka;
using FhirPseudonymizer.Config;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Prometheus;

namespace FhirPseudonymizer.Kafka;

/// <summary>
///     Consumes FHIR resources/bundles from one or more Kafka topics, anonymizes them, and produces
///     the result to a per-topic output topic.
///
///     A single thread owns the <see cref="IConsumer{TKey,TValue}" /> and polls for new messages
///     (Consume/StoreOffset are not guaranteed thread-safe). Each consumed message is routed, based
///     on a stable hash of its TopicPartition, to one of a fixed number of worker channels so that
///     messages from the same partition are always processed by the same worker and therefore stay
///     in order, while different partitions are anonymized concurrently across workers. Workers
///     report back which messages completed successfully so the poll thread can store their offsets.
/// </summary>
public class KafkaConsumerService : BackgroundService
{
    private static readonly Counter ProcessedMessagesCounter = Metrics.CreateCounter(
        "fhirpseudonymizer_kafka_messages_total",
        "Total number of FHIR resources consumed from Kafka, by source topic and outcome.",
        new CounterConfiguration { LabelNames = new[] { "topic", "outcome" } }
    );

    private readonly IConsumer<byte[], string> consumer;
    private readonly IProducer<byte[], string> producer;
    private readonly IAnonymizerEngine anonymizer;
    private readonly AnonymizationConfig anonymizationConfig;
    private readonly KafkaConfig kafkaConfig;
    private readonly ILogger<KafkaConsumerService> logger;
    private readonly FhirJsonParser fhirJsonParser = new();
    private readonly FhirJsonSerializer fhirJsonSerializer = new();
    private readonly Channel<ConsumeResult<byte[], string>>[] workerChannels;
    private readonly Channel<ConsumeResult<byte[], string>> completedResults =
        Channel.CreateUnbounded<ConsumeResult<byte[], string>>();
    private readonly Regex outputTopicPattern;

    public KafkaConsumerService(
        IConsumer<byte[], string> consumer,
        IProducer<byte[], string> producer,
        IAnonymizerEngine anonymizer,
        AnonymizationConfig anonymizationConfig,
        KafkaConfig kafkaConfig,
        ILogger<KafkaConsumerService> logger
    )
    {
        this.consumer = consumer;
        this.producer = producer;
        this.anonymizer = anonymizer;
        this.anonymizationConfig = anonymizationConfig;
        this.kafkaConfig = kafkaConfig;
        this.logger = logger;

        outputTopicPattern = new Regex(kafkaConfig.OutputTopicPattern, RegexOptions.Compiled);

        workerChannels =
        [
            .. Enumerable
                .Range(0, Math.Max(1, kafkaConfig.WorkerCount))
                .Select(_ =>
                    Channel.CreateBounded<ConsumeResult<byte[], string>>(
                        new BoundedChannelOptions(Math.Max(1, kafkaConfig.WorkerChannelCapacity))
                        {
                            FullMode = BoundedChannelFullMode.Wait,
                            SingleReader = true,
                            SingleWriter = true,
                        }
                    )
                ),
        ];
    }

    protected override async System.Threading.Tasks.Task ExecuteAsync(
        CancellationToken stoppingToken
    )
    {
        consumer.Subscribe(kafkaConfig.Topics);
        logger.LogInformation(
            "Subscribed to Kafka topics: {Topics} using {WorkerCount} workers",
            string.Join(", ", kafkaConfig.Topics),
            workerChannels.Length
        );

        var workerTasks = workerChannels
            .Select(channel => RunWorkerAsync(channel.Reader))
            .ToArray();

        try
        {
            await RunConsumeLoopAsync(stoppingToken);
        }
        finally
        {
            foreach (var channel in workerChannels)
            {
                channel.Writer.TryComplete();
            }

            await System.Threading.Tasks.Task.WhenAll(workerTasks);

            // pick up offsets stored by workers that finished after the last drain in the loop above
            StoreCompletedOffsets();

            consumer.Close();
        }
    }

    private async System.Threading.Tasks.Task RunConsumeLoopAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            ConsumeResult<byte[], string> result;

            try
            {
                result = consumer.Consume(TimeSpan.FromMilliseconds(100));
            }
            catch (ConsumeException exc)
            {
                logger.LogError(exc, "Failed to consume message from Kafka");
                continue;
            }

            if (result?.Message?.Value is not null)
            {
                var workerIndex = GetWorkerIndex(result.TopicPartition);

                try
                {
                    await workerChannels[workerIndex].Writer.WriteAsync(result, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            StoreCompletedOffsets();
        }
    }

    private void StoreCompletedOffsets()
    {
        while (completedResults.Reader.TryRead(out var result))
        {
            consumer.StoreOffset(result);
        }
    }

    /// <summary>
    ///     Maps a partition to one of the fixed worker channels using a stable hash, so that every
    ///     message of a given partition is always routed to, and processed in order by, the same
    ///     worker - without needing a dedicated channel per partition.
    /// </summary>
    public int GetWorkerIndex(TopicPartition partition)
    {
        var hash = Math.Abs(HashCode.Combine(partition.Topic, partition.Partition.Value));
        return hash % workerChannels.Length;
    }

    private async System.Threading.Tasks.Task RunWorkerAsync(
        ChannelReader<ConsumeResult<byte[], string>> reader
    )
    {
        await foreach (var result in reader.ReadAllAsync(CancellationToken.None))
        {
            await ProcessResultAsync(result);
        }
    }

    public async System.Threading.Tasks.Task ProcessResultAsync(
        ConsumeResult<byte[], string> result
    )
    {
        try
        {
            var output = AnonymizeMessage(result.Topic, result.Message.Value);
            var outputTopic = GetOutputTopic(result.Topic);

            producer.Produce(
                outputTopic,
                new Message<byte[], string> { Key = result.Message.Key, Value = output }
            );

            ProcessedMessagesCounter.WithLabels(result.Topic, "success").Inc();

            await completedResults.Writer.WriteAsync(result, CancellationToken.None);
        }
        catch (Exception exc)
        {
            ProcessedMessagesCounter.WithLabels(result.Topic, "error").Inc();
            logger.LogError(exc, "Failed to process message from topic {Topic}", result.Topic);
        }
    }

    /// <summary>
    ///     Derives the output topic for a given input topic by applying the configured
    ///     <see cref="KafkaConfig.OutputTopicPattern" />/<see cref="KafkaConfig.OutputTopicReplacement" />
    ///     regex match-and-replace, e.g. matching "^fhir\." and replacing it with
    ///     "fhir.pseudonymized." turns "fhir.test" into "fhir.pseudonymized.test".
    /// </summary>
    public string GetOutputTopic(string sourceTopic)
    {
        return outputTopicPattern.Replace(sourceTopic, kafkaConfig.OutputTopicReplacement);
    }

    public string AnonymizeMessage(string sourceTopic, string json)
    {
        using var activity = Program.ActivitySource.StartActivity(nameof(AnonymizeMessage));
        activity?.AddTag("kafka.topic", sourceTopic);

        var resource = fhirJsonParser.Parse<Resource>(json);

        var settings = new AnonymizerSettings
        {
            ShouldAddSecurityTag = anonymizationConfig.ShouldAddSecurityTag,
        };

        var anonymized = anonymizer.AnonymizeResource(resource, settings);
        return fhirJsonSerializer.SerializeToString(anonymized);
    }

    public override async System.Threading.Tasks.Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        producer.Flush(cancellationToken);
    }
}
