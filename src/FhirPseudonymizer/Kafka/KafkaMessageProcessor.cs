using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Confluent.Kafka;
using FhirPseudonymizer.Config;
using FhirPseudonymizer.Pseudonymization;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;

namespace FhirPseudonymizer.Kafka;

/// <summary>
///     How a message consumed from Kafka was ultimately handled, reported by
///     <see cref="KafkaMessageProcessor.ProcessAsync" /> once the broker has acknowledged
///     (or rejected) whatever was produced for it.
/// </summary>
public enum KafkaMessageOutcome
{
    /// <summary>The pseudonymized message was delivered to its output topic.</summary>
    Produced,

    /// <summary>
    ///     Processing failed and the original message was delivered to its dead letter topic
    ///     instead.
    /// </summary>
    DeadLettered,

    /// <summary>
    ///     Processing failed and so did delivering the original message to its dead letter topic:
    ///     the message was not handled at all and must not be marked as consumed.
    /// </summary>
    Failed,
}

/// <summary>
///     Pseudonymizes a single FHIR resource/bundle consumed from Kafka and produces the result to
///     its output topic (and a Provenance audit trail via the <see cref="IProvenancePublisher" />),
///     or - if that fails - the original message to its dead letter topic. Knows nothing about
///     partitions, ordering, or offsets; see <see cref="KafkaConsumerService" /> for those.
/// </summary>
public class KafkaMessageProcessor
{
    private static readonly Counter<long> ProcessedMessagesCounter =
        Program.Meter.CreateCounter<long>(
            "fhirpseudonymizer.kafka.messages",
            description: "Total number of FHIR resources consumed from Kafka, by source topic and outcome (success, dead-lettered, or error), counted once the broker acknowledged the produced message."
        );

    // Shared, like the REST API's System.Text.Json formatters, see FhirFormatter.
    private static readonly JsonSerializerOptions FhirJsonOptions =
        new JsonSerializerOptions().ForFhir(ModelInfo.ModelInspector);

    private static readonly TimeSpan QueueFullRetryDelay = TimeSpan.FromMilliseconds(100);

    private readonly IProducer<byte[], string> producer;
    private readonly IAnonymizerEngine anonymizer;
    private readonly AnonymizationConfig anonymizationConfig;
    private readonly KafkaConfig kafkaConfig;
    private readonly IProvenancePublisher provenancePublisher;
    private readonly ILogger<KafkaMessageProcessor> logger;
    private readonly FhirJsonParser legacyFhirJsonParser = new();
    private readonly Regex outputTopicPattern;
    private readonly string groupId;

    public KafkaMessageProcessor(
        IProducer<byte[], string> producer,
        IAnonymizerEngine anonymizer,
        AnonymizationConfig anonymizationConfig,
        KafkaConfig kafkaConfig,
        IProvenancePublisher provenancePublisher,
        ILogger<KafkaMessageProcessor> logger
    )
    {
        this.producer = producer;
        this.anonymizer = anonymizer;
        this.anonymizationConfig = anonymizationConfig;
        this.kafkaConfig = kafkaConfig;
        this.provenancePublisher = provenancePublisher;
        this.logger = logger;

        outputTopicPattern = new Regex(kafkaConfig.OutputTopicPattern, RegexOptions.Compiled);
        groupId = kafkaConfig.Consumer.GroupId ?? KafkaExtensions.DefaultGroupId;
    }

    /// <summary>
    ///     Parses, pseudonymizes and produces a consumed message to its output topic, falling back
    ///     to producing the original message to its dead letter topic if any of that fails.
    ///
    ///     The returned task completes as soon as the resulting message has been handed to the
    ///     producer, so the caller can move on to the next message (and messages produced in that
    ///     order keep it). <paramref name="onCompleted" /> is called later, exactly once, once the
    ///     broker acknowledged or rejected it - only then is the message actually safe to mark as
    ///     consumed. It is called from the producer's delivery report thread, so must be fast and
    ///     thread-safe.
    ///
    ///     A transient pseudonymization backend failure (<see cref="TransientPseudonymizationException" />)
    ///     is retried indefinitely with backoff rather than dead-lettered, since the message itself
    ///     isn't bad, just badly timed. If <paramref name="cancellationToken" /> is cancelled while
    ///     retrying, or while waiting for room in the producer's queue, this returns without ever
    ///     calling <paramref name="onCompleted" />: the message was simply not handled.
    /// </summary>
    public async System.Threading.Tasks.Task ProcessAsync(
        ConsumeResult<byte[], string> result,
        Action<KafkaMessageOutcome> onCompleted,
        CancellationToken cancellationToken = default
    )
    {
        Resource original;
        Resource anonymized;
        string output;

        try
        {
            original = ParseResource(result.Message.Value);
            anonymized = await AnonymizeWithRetryAsync(original, result.Topic, cancellationToken);
            output = JsonSerializer.Serialize(anonymized, FhirJsonOptions);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        catch (Exception exc)
        {
            logger.LogError(
                exc,
                "Failed to process message from {TopicPartitionOffset}, sending it to the dead letter topic",
                result.TopicPartitionOffset
            );

            await SendToDeadLetterTopicAsync(result, exc, onCompleted, cancellationToken);
            return;
        }

        var message = new Message<byte[], string>
        {
            Key = result.Message.Key,
            Value = output,
            Headers = CopyHeaders(result.Message.Headers),
        };

        void OnDelivered(DeliveryReport<byte[], string> report)
        {
            if (!report.Error.IsError)
            {
                ProcessedMessagesCounter.Add(
                    1,
                    new TagList { { "topic", result.Topic }, { "outcome", "success" } }
                );
                onCompleted(KafkaMessageOutcome.Produced);
                return;
            }

            var exc = new ProduceException<byte[], string>(report.Error, report);
            logger.LogError(
                exc,
                "Failed to deliver the pseudonymized message from {TopicPartitionOffset} to {Topic}, sending the original to the dead letter topic",
                result.TopicPartitionOffset,
                report.Topic
            );

            // Runs on the producer's delivery report thread, which must not be blocked; the
            // dead letter send handles (and reports) all of its own failures.
            _ = SendToDeadLetterTopicAsync(result, exc, onCompleted, CancellationToken.None);
        }

        try
        {
            await ProduceAsync(
                GetOutputTopic(result.Topic),
                message,
                OnDelivered,
                cancellationToken
            );
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        catch (KafkaException exc)
        {
            // e.g. the pseudonymized message exceeds message.max.bytes
            logger.LogError(
                exc,
                "Failed to produce the pseudonymized message from {TopicPartitionOffset}, sending the original to the dead letter topic",
                result.TopicPartitionOffset
            );

            await SendToDeadLetterTopicAsync(result, exc, onCompleted, cancellationToken);
            return;
        }

        provenancePublisher.Publish(original, anonymized, CopyHeaders(result.Message.Headers));
    }

    /// <summary>
    ///     Parses with the System.Text.Json-based deserializer, which is several times faster than
    ///     the legacy <see cref="FhirJsonParser" /> and doesn't serialize all concurrent callers
    ///     on a process-wide lock like it does. It also validates the resource though, rejecting
    ///     inputs the legacy parser accepts (e.g. ids longer than 64 characters or containing an
    ///     underscore, missing required elements, empty strings): to not start dead-lettering
    ///     messages that used to be processed fine, those fall back to the legacy parser.
    /// </summary>
    private Resource ParseResource(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<Resource>(json, FhirJsonOptions)
                ?? throw new JsonException("The message is the JSON literal 'null'.");
        }
        catch (DeserializationFailedException exc)
        {
            logger.LogDebug(
                exc,
                "Message is not a strictly valid FHIR resource, falling back to the legacy parser"
            );

            return legacyFhirJsonParser.Parse<Resource>(json);
        }
    }

    private async System.Threading.Tasks.Task<Resource> AnonymizeWithRetryAsync(
        Resource resource,
        string sourceTopic,
        CancellationToken cancellationToken
    )
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                using var activity = Program.ActivitySource.StartActivity("AnonymizeMessageAsync");
                activity?.AddTag("kafka.topic", sourceTopic);

                var settings = new AnonymizerSettings
                {
                    ShouldAddSecurityTag = anonymizationConfig.ShouldAddSecurityTag,
                };

                return await anonymizer.AnonymizeResourceAsync(resource, settings);
            }
            catch (TransientPseudonymizationException exc)
            {
                logger.LogWarning(
                    exc,
                    "Pseudonymization backend unavailable while processing message from topic {Topic} (attempt {Attempt}); retrying",
                    sourceTopic,
                    attempt
                );

                var delaySeconds = Math.Min(60, Math.Pow(2, attempt - 1));
                await System.Threading.Tasks.Task.Delay(
                    TimeSpan.FromSeconds(delaySeconds),
                    cancellationToken
                );
            }
        }
    }

    /// <summary>
    ///     Sends a message that failed processing to its dead letter topic unchanged, along with
    ///     headers describing the failure, reporting <see cref="KafkaMessageOutcome.DeadLettered" />
    ///     once that is acknowledged, or <see cref="KafkaMessageOutcome.Failed" /> if that fails too.
    /// </summary>
    private async System.Threading.Tasks.Task SendToDeadLetterTopicAsync(
        ConsumeResult<byte[], string> result,
        Exception exc,
        Action<KafkaMessageOutcome> onCompleted,
        CancellationToken cancellationToken
    )
    {
        var headers = CopyHeaders(result.Message.Headers);
        headers.Add(
            "x-error-type",
            Encoding.UTF8.GetBytes(exc.GetType().FullName ?? exc.GetType().Name)
        );
        headers.Add("x-error-message", Encoding.UTF8.GetBytes(exc.Message));
        headers.Add("x-source-topic", Encoding.UTF8.GetBytes(result.Topic));
        headers.Add(
            "x-source-partition",
            Encoding.UTF8.GetBytes(result.Partition.Value.ToString(CultureInfo.InvariantCulture))
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

        void Fail(Exception deadLetterExc)
        {
            ProcessedMessagesCounter.Add(
                1,
                new TagList { { "topic", result.Topic }, { "outcome", "error" } }
            );
            logger.LogError(
                deadLetterExc,
                "Failed to send message from {TopicPartitionOffset} to the dead letter topic",
                result.TopicPartitionOffset
            );
            onCompleted(KafkaMessageOutcome.Failed);
        }

        try
        {
            await ProduceAsync(
                GetDeadLetterTopic(result.Topic),
                message,
                report =>
                {
                    if (report.Error.IsError)
                    {
                        Fail(new ProduceException<byte[], string>(report.Error, report));
                        return;
                    }

                    ProcessedMessagesCounter.Add(
                        1,
                        new TagList { { "topic", result.Topic }, { "outcome", "dead-lettered" } }
                    );
                    onCompleted(KafkaMessageOutcome.DeadLettered);
                },
                cancellationToken
            );
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // shutting down while waiting for room in the producer's queue: left unhandled
        }
        catch (Exception deadLetterExc)
        {
            Fail(deadLetterExc);
        }
    }

    /// <summary>
    ///     <see cref="IProducer{TKey,TValue}.Produce(string,Message{TKey,TValue},Action{DeliveryReport{TKey,TValue}})" />,
    ///     except that a full local producer queue - which just means the broker currently can't
    ///     keep up - is waited out instead of being treated as a failure of this message.
    /// </summary>
    private async System.Threading.Tasks.Task ProduceAsync(
        string topic,
        Message<byte[], string> message,
        Action<DeliveryReport<byte[], string>> deliveryHandler,
        CancellationToken cancellationToken
    )
    {
        while (true)
        {
            try
            {
                producer.Produce(topic, message, deliveryHandler);
                return;
            }
            catch (KafkaException exc) when (exc.Error.Code == ErrorCode.Local_QueueFull)
            {
                await System.Threading.Tasks.Task.Delay(QueueFullRetryDelay, cancellationToken);
            }
        }
    }

    /// <summary>
    ///     Waits for all outstanding produce requests to be acknowledged (or fail), so their
    ///     outcomes get reported before shutting down.
    /// </summary>
    public void Flush(TimeSpan timeout)
    {
        var remaining = producer.Flush(timeout);
        if (remaining > 0)
        {
            logger.LogWarning(
                "{Count} produced messages were still unacknowledged after waiting {Timeout} on shutdown",
                remaining,
                timeout
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
}
