using System.Globalization;
using System.Text;
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
///     the result to a per-topic output topic. Every anonymized message is also passed to the
///     configured <see cref="IProvenancePublisher" />, which publishes a Bundle of Provenance
///     resources documenting the pseudonymization to <see cref="KafkaConfig.ProvenanceTopic" />, if
///     configured.
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
        "Total number of FHIR resources consumed from Kafka, by source topic and outcome (success, dead-lettered, or error).",
        new CounterConfiguration { LabelNames = ["topic", "outcome"] }
    );

    private readonly IConsumer<byte[], string> consumer;
    private readonly IProducer<byte[], string> producer;
    private readonly IAnonymizerEngine anonymizer;
    private readonly AnonymizationConfig anonymizationConfig;
    private readonly KafkaConfig kafkaConfig;
    private readonly IProvenancePublisher provenancePublisher;
    private readonly ILogger<KafkaConsumerService> logger;
    private readonly FhirJsonParser fhirJsonParser = new();
    private readonly FhirJsonSerializer fhirJsonSerializer = new();
    private readonly Channel<ConsumeResult<byte[], string>>[] workerChannels;
    private readonly Channel<ConsumeResult<byte[], string>> completedResults =
        Channel.CreateUnbounded<ConsumeResult<byte[], string>>();
    private readonly Regex outputTopicPattern;
    private readonly string groupId;

    public KafkaConsumerService(
        IConsumer<byte[], string> consumer,
        IProducer<byte[], string> producer,
        IAnonymizerEngine anonymizer,
        AnonymizationConfig anonymizationConfig,
        KafkaConfig kafkaConfig,
        IProvenancePublisher provenancePublisher,
        ILogger<KafkaConsumerService> logger
    )
    {
        this.consumer = consumer;
        this.producer = producer;
        this.anonymizer = anonymizer;
        this.anonymizationConfig = anonymizationConfig;
        this.kafkaConfig = kafkaConfig;
        this.provenancePublisher = provenancePublisher;
        this.logger = logger;

        outputTopicPattern = new Regex(kafkaConfig.OutputTopicPattern, RegexOptions.Compiled);
        groupId = kafkaConfig.Consumer.GroupId ?? KafkaExtensions.DefaultGroupId;

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
            var original = fhirJsonParser.Parse<Resource>(result.Message.Value);
            var anonymized = await AnonymizeResourceAsync(original, result.Topic);
            var output = fhirJsonSerializer.SerializeToString(anonymized);
            var outputTopic = GetOutputTopic(result.Topic);

            producer.Produce(
                outputTopic,
                new Message<byte[], string>
                {
                    Key = result.Message.Key,
                    Value = output,
                    Headers = CopyHeaders(result.Message.Headers),
                }
            );

            ProcessedMessagesCounter.WithLabels(result.Topic, "success").Inc();

            provenancePublisher.Publish(original, anonymized, CopyHeaders(result.Message.Headers));

            await completedResults.Writer.WriteAsync(result, CancellationToken.None);
        }
        catch (ProduceException<byte[], string> exc)
        {
            logger.LogError(
                exc,
                "Failed to process message from topic {Topic}, sending to dead letter queue",
                result.Topic
            );

            await SendToDeadLetterQueueAsync(result, exc);
        }
        catch (ConsumeException exc)
        {
            logger.LogError(
                exc,
                "Failed to process message from topic {Topic}, sending to dead letter queue",
                result.Topic
            );

            await SendToDeadLetterQueueAsync(result, exc);
        }
        catch (FormatException exc)
        {
            logger.LogError(
                exc,
                "Failed to process message from topic {Topic}, sending to dead letter queue",
                result.Topic
            );

            await SendToDeadLetterQueueAsync(result, exc);
        }
        catch (InvalidOperationException exc)
        {
            logger.LogError(
                exc,
                "Failed to process message from topic {Topic}, sending to dead letter queue",
                result.Topic
            );

            await SendToDeadLetterQueueAsync(result, exc);
        }
    }

    /// <summary>
    ///     Sends a message that failed processing to its dead letter topic unchanged, along with
    ///     headers describing the failure, and only then marks it as handled (i.e. its offset gets
    ///     stored). If producing to the dead letter topic itself fails, the message is left
    ///     unhandled so it gets reprocessed after a restart, same as if no dead letter queue existed.
    /// </summary>
    private async System.Threading.Tasks.Task SendToDeadLetterQueueAsync(
        ConsumeResult<byte[], string> result,
        Exception exc
    )
    {
        try
        {
            var deadLetterTopic = GetDeadLetterTopic(result.Topic);

            var headers = CopyHeaders(result.Message.Headers);
            headers.Add(
                "x-error-type",
                Encoding.UTF8.GetBytes(exc.GetType().FullName ?? exc.GetType().Name)
            );
            headers.Add("x-error-message", Encoding.UTF8.GetBytes(exc.Message));
            headers.Add("x-source-topic", Encoding.UTF8.GetBytes(result.Topic));
            headers.Add(
                "x-source-partition",
                Encoding.UTF8.GetBytes(
                    result.Partition.Value.ToString(CultureInfo.InvariantCulture)
                )
            );
            headers.Add(
                "x-source-offset",
                Encoding.UTF8.GetBytes(result.Offset.Value.ToString(CultureInfo.InvariantCulture))
            );

            var message = new Message<byte[], string>
            {
                Key = result.Message.Key,
                Value = result.Message.Value,
                Headers = headers,
            };

            producer.Produce(deadLetterTopic, message);

            ProcessedMessagesCounter.WithLabels(result.Topic, "dead-lettered").Inc();

            await completedResults.Writer.WriteAsync(result, CancellationToken.None);
        }
        catch (Exception dlqExc)
        {
            ProcessedMessagesCounter.WithLabels(result.Topic, "error").Inc();
            logger.LogError(
                dlqExc,
                "Failed to send message from topic {Topic} to dead letter queue",
                result.Topic
            );
        }
    }

    /// <summary>
    ///     Copies a consumed message's headers (e.g. distributed tracing context, correlation
    ///     ids) onto a new <see cref="Headers" /> instance, so they survive being forwarded onto
    ///     the message produced to the output/dead letter topic.
    /// </summary>
    private static Headers CopyHeaders(Headers originalHeaders)
    {
        var headers = new Headers();

        if (originalHeaders is not null)
        {
            foreach (var header in originalHeaders)
            {
                headers.Add(header.Key, header.GetValueBytes());
            }
        }

        return headers;
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

    /// <summary>
    ///     Derives the dead letter topic for a given input topic, named
    ///     "error.&lt;input-topic&gt;.&lt;group-id&gt;" (mirroring Spring Kafka's default DLT naming).
    /// </summary>
    public string GetDeadLetterTopic(string sourceTopic)
    {
        return $"error.{sourceTopic}.{groupId}";
    }

    public async System.Threading.Tasks.Task<string> AnonymizeMessageAsync(
        string sourceTopic,
        string json
    )
    {
        var resource = fhirJsonParser.Parse<Resource>(json);
        var anonymized = await AnonymizeResourceAsync(resource, sourceTopic);
        return fhirJsonSerializer.SerializeToString(anonymized);
    }

    /// <summary>
    ///     Anonymizes an already-parsed resource. Takes the parsed <see cref="Resource" /> rather
    ///     than raw JSON so callers (see <see cref="ProcessResultAsync" />) retain the
    ///     pre-anonymization resource, needed to tell which security labels
    ///     <see cref="IProvenancePublisher" /> newly applied versus already had.
    /// </summary>
    private async System.Threading.Tasks.Task<Resource> AnonymizeResourceAsync(
        Resource resource,
        string sourceTopic
    )
    {
        using var activity = Program.ActivitySource.StartActivity(nameof(AnonymizeMessageAsync));
        activity?.AddTag("kafka.topic", sourceTopic);

        var settings = new AnonymizerSettings
        {
            ShouldAddSecurityTag = anonymizationConfig.ShouldAddSecurityTag,
        };

        return await anonymizer.AnonymizeResourceAsync(resource, settings);
    }

    public override async System.Threading.Tasks.Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        producer.Flush(cancellationToken);
    }
}
