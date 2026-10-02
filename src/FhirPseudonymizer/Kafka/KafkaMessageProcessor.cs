using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Security.Cryptography;
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
    /// <summary>
    ///     The pseudonymized message was delivered to its output topic, and its provenance (if
    ///     recorded at all) to the provenance topic.
    /// </summary>
    Produced,

    /// <summary>
    ///     Processing failed, or delivering the pseudonymized message or its provenance did, and
    ///     the original message was delivered to its dead letter topic instead.
    /// </summary>
    DeadLettered,

    /// <summary>
    ///     Processing failed and so did delivering the original message to its dead letter topic:
    ///     the message was not handled at all and must not be marked as consumed.
    /// </summary>
    Failed,

    /// <summary>
    ///     Processing was cancelled (because the service is stopping, or the message's partition
    ///     was revoked) before everything was produced for it: the message was not handled, so it
    ///     must not be marked as consumed, but that's no reason to stop.
    /// </summary>
    Abandoned,
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

    private static readonly Histogram<double> MessageProcessingDuration =
        Program.Meter.CreateHistogram<double>(
            "fhirpseudonymizer.kafka.message.duration",
            unit: "s",
            description: "Time spent processing a single Kafka message end-to-end."
        );

    // Shared, like the REST API's System.Text.Json formatters, see FhirFormatter.
    private static readonly JsonSerializerOptions FhirJsonOptions =
        new JsonSerializerOptions().ForFhir(ModelInfo.ModelInspector);

    private const int MinimumMessageKeyCryptoHashKeyLength = 32;

    private readonly IProducer<byte[], string> producer;
    private readonly IAnonymizerEngine anonymizer;
    private readonly AnonymizationConfig anonymizationConfig;
    private readonly KafkaConfig kafkaConfig;
    private readonly IProvenancePublisher provenancePublisher;
    private readonly ILogger<KafkaMessageProcessor> logger;
    private readonly Regex outputTopicPattern;
    private readonly string groupId;

    // null if message keys are kept as they are
    private readonly byte[] messageKeyCryptoHashKey;

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

        if (kafkaConfig.CryptoHashMessageKeys.Enabled)
        {
            messageKeyCryptoHashKey = GetMessageKeyCryptoHashKey(kafkaConfig.CryptoHashMessageKeys);
        }
    }

    private static byte[] GetMessageKeyCryptoHashKey(CryptoHashMessageKeysConfig config)
    {
        if (string.IsNullOrEmpty(config.Key))
        {
            throw new ValidationException(
                "Kafka message keys are crypto-hashed by default, which requires "
                    + "Kafka__CryptoHashMessageKeys__Key to be set to a randomly generated key, "
                    + "e.g. from `openssl rand -hex 32`. Set "
                    + "Kafka__CryptoHashMessageKeys__Enabled=false to keep the input messages' "
                    + "keys instead."
            );
        }

        // Same minimum as for the anonymization config's keys. Never includes the key itself,
        // since this ends up in the logs.
        var keyBytes = Encoding.UTF8.GetBytes(config.Key);
        if (keyBytes.Length < MinimumMessageKeyCryptoHashKeyLength)
        {
            throw new ValidationException(
                $"Kafka__CryptoHashMessageKeys__Key is only {keyBytes.Length} bytes long, but "
                    + $"must be at least {MinimumMessageKeyCryptoHashKeyLength} bytes. Use a "
                    + "randomly generated key instead, e.g. from `openssl rand -hex 32`."
            );
        }

        return keyBytes;
    }

    /// <summary>
    ///     The key to produce the pseudonymized version of a message with, see
    ///     <see cref="KafkaConfig.CryptoHashMessageKeys" />.
    /// </summary>
    private byte[] GetOutputKey(byte[] key)
    {
        // an empty key identifies nothing, so is left empty - as is an empty value by cryptoHash
        if (messageKeyCryptoHashKey is null || key is null || key.Length == 0)
        {
            return key;
        }

        Span<byte> hash = stackalloc byte[HMACSHA256.HashSizeInBytes];
        HMACSHA256.HashData(messageKeyCryptoHashKey, key, hash);

        return Encoding.UTF8.GetBytes(Convert.ToHexStringLower(hash));
    }

    /// <summary>
    ///     Parses, pseudonymizes and produces a consumed message to its output topic, along with
    ///     its provenance, falling back to producing the original message to its dead letter topic
    ///     if any of that fails.
    ///
    ///     The returned task completes as soon as the resulting messages have been handed to the
    ///     producer, so the caller can move on to the next message (and messages produced in that
    ///     order keep it). <paramref name="onCompleted" /> is called exactly once, once the broker
    ///     acknowledged or rejected both the pseudonymized message and its provenance - only then
    ///     is the message actually safe to mark as consumed. It may be called from the producer's
    ///     delivery report thread, so must be fast and thread-safe.
    ///
    ///     If the pseudonymization backend is unavailable, the
    ///     <see cref="TransientPseudonymizationException" /> is thrown instead - without calling
    ///     <paramref name="onCompleted" /> - for the caller to retry the message later: it isn't
    ///     bad, just badly timed. <paramref name="attempt" /> is which attempt this is, for the
    ///     metrics. If <paramref name="cancellationToken" /> is cancelled while waiting for room in
    ///     the producer's queue, or before a failure, the message is reported as
    ///     <see cref="KafkaMessageOutcome.Abandoned" />.
    /// </summary>
    public async System.Threading.Tasks.Task ProcessAsync(
        ConsumeResult<byte[], string> result,
        int attempt,
        Action<KafkaMessageOutcome> onCompleted,
        CancellationToken cancellationToken = default
    )
    {
        var startTimestamp = Stopwatch.GetTimestamp();

        Resource preImage;
        Resource anonymized;
        string output;

        try
        {
            (preImage, anonymized) = await AnonymizeAsync(result.Message.Value, result.Topic);
            output = JsonSerializer.Serialize(anonymized, FhirJsonOptions);
        }
        catch (Exception exc) when (cancellationToken.IsCancellationRequested)
        {
            // Failing once the message's processing was cancelled is more likely a consequence of
            // stopping than of the message - e.g. the host disposing the services it depends on
            // after its shutdown timeout - so it is abandoned (and reprocessed later, by this
            // consumer after a restart or by the partition's new owner) rather than dead-lettered.
            if (exc is not OperationCanceledException)
            {
                logger.LogWarning(
                    exc,
                    "Processing message from {TopicPartitionOffset} failed after it was cancelled, leaving it to be reprocessed",
                    result.TopicPartitionOffset
                );
            }

            onCompleted(KafkaMessageOutcome.Abandoned);
            return;
        }
        catch (TransientPseudonymizationException)
        {
            throw;
        }
        catch (Exception exc)
        {
            logger.LogError(
                exc,
                "Failed to process message from {TopicPartitionOffset}, sending it to the dead letter topic",
                result.TopicPartitionOffset
            );

            await SendToDeadLetterTopicAsync(
                result,
                exc,
                onCompleted,
                startTimestamp,
                attempt,
                cancellationToken
            );
            return;
        }

        var message = new Message<byte[], string>
        {
            Key = GetOutputKey(result.Message.Key),
            Value = output,
            Headers = CopyHeaders(result.Message.Headers),
        };

        var deliveries = new PendingDeliveries(delivered =>
            OnDelivered(result, delivered, onCompleted, startTimestamp, attempt)
        );

        try
        {
            await producer.ProduceWaitingForRoomAsync(
                GetOutputTopic(result.Topic),
                message,
                report =>
                    deliveries.ReportOutput(
                        report.Error.IsError
                            ? new ProduceException<byte[], string>(report.Error, report)
                            : null
                    ),
                cancellationToken
            );
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            onCompleted(KafkaMessageOutcome.Abandoned);
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

            await SendToDeadLetterTopicAsync(
                result,
                exc,
                onCompleted,
                startTimestamp,
                attempt,
                cancellationToken
            );
            return;
        }

        try
        {
            await provenancePublisher.PublishAsync(
                preImage,
                anonymized,
                CopyHeaders(result.Message.Headers),
                deliveries.ReportProvenance,
                cancellationToken
            );
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            deliveries.AbandonProvenance();
        }
    }

    /// <summary>
    ///     Reports how a message was handled, once both its pseudonymized version and its
    ///     provenance were acknowledged or rejected - sending the original to the dead letter topic
    ///     if either was rejected: the pseudonymized message is then either missing, or its
    ///     provenance is.
    /// </summary>
    private void OnDelivered(
        ConsumeResult<byte[], string> result,
        PendingDeliveries deliveries,
        Action<KafkaMessageOutcome> onCompleted,
        long startTimestamp,
        int attempt
    )
    {
        if (deliveries.IsProvenanceAbandoned)
        {
            // only happens when stopping, and the message isn't done without its provenance
            onCompleted(KafkaMessageOutcome.Abandoned);
            return;
        }

        if (deliveries.OutputFailure is null && deliveries.ProvenanceFailure is null)
        {
            ProcessedMessagesCounter.Add(
                1,
                new TagList { { "topic", result.Topic }, { "outcome", "success" } }
            );
            MessageProcessingDuration.Record(
                Stopwatch.GetElapsedTime(startTimestamp).TotalSeconds,
                new TagList
                {
                    { "topic", result.Topic },
                    { "outcome", "success" },
                    { "attempts", attempt },
                }
            );
            onCompleted(KafkaMessageOutcome.Produced);
            return;
        }

        if (deliveries.OutputFailure is not null)
        {
            logger.LogError(
                deliveries.OutputFailure,
                "Failed to deliver the pseudonymized message from {TopicPartitionOffset} to {Topic}, sending the original to the dead letter topic",
                result.TopicPartitionOffset,
                GetOutputTopic(result.Topic)
            );
        }
        else
        {
            logger.LogError(
                deliveries.ProvenanceFailure,
                "Failed to publish the provenance of the pseudonymized message from {TopicPartitionOffset}, sending the original to the dead letter topic",
                result.TopicPartitionOffset
            );
        }

        // Usually runs on the producer's delivery report thread, which must not be blocked; the
        // dead letter send handles (and reports) all of its own failures.
        _ = SendToDeadLetterTopicAsync(
            result,
            deliveries.OutputFailure ?? deliveries.ProvenanceFailure,
            onCompleted,
            startTimestamp,
            attempt,
            CancellationToken.None
        );
    }

    /// <summary>
    ///     Parses with the System.Text.Json-based deserializer, which also validates the resource:
    ///     one that isn't valid FHIR (e.g. with an id containing characters FHIR doesn't allow, or
    ///     missing required elements) is rejected, and so ends up in the dead letter topic.
    /// </summary>
    private static Resource ParseResource(string json)
    {
        return JsonSerializer.Deserialize<Resource>(json, FhirJsonOptions)
            ?? throw new JsonException("The message is the JSON literal 'null'.");
    }

    private async System.Threading.Tasks.Task<(
        Resource PreImage,
        Resource Anonymized
    )> AnonymizeAsync(string json, string sourceTopic)
    {
        // Parsed afresh for every attempt: the anonymizer modifies the resource it is given in
        // place, so an attempt failing midway (e.g. after some of its pseudonymization calls
        // already went through) would otherwise leave the next one a partially pseudonymized
        // resource to pseudonymize again.
        var resource = ParseResource(json);

        // Snapshot before anonymizing: the anonymizer mutates `resource` in place and returns
        // that same instance, so `resource` is no longer the pre-image afterwards.
        var preImage = provenancePublisher.CapturePreImage(resource);

        using var activity = Program.ActivitySource.StartActivity("AnonymizeMessageAsync");
        activity?.AddTag("kafka.topic", sourceTopic);

        var settings = new AnonymizerSettings
        {
            ShouldAddSecurityTag = anonymizationConfig.ShouldAddSecurityTag,
        };

        return (preImage, await anonymizer.AnonymizeResourceAsync(resource, settings));
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
        long startTimestamp,
        int attempt,
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
            MessageProcessingDuration.Record(
                Stopwatch.GetElapsedTime(startTimestamp).TotalSeconds,
                new TagList
                {
                    { "topic", result.Topic },
                    { "outcome", "error" },
                    { "attempts", attempt },
                }
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
            await producer.ProduceWaitingForRoomAsync(
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
                    MessageProcessingDuration.Record(
                        Stopwatch.GetElapsedTime(startTimestamp).TotalSeconds,
                        new TagList
                        {
                            { "topic", result.Topic },
                            { "outcome", "dead-lettered" },
                            { "attempts", attempt },
                        }
                    );
                    onCompleted(KafkaMessageOutcome.DeadLettered);
                },
                cancellationToken
            );
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // cancelled while waiting for room in the producer's queue
            onCompleted(KafkaMessageOutcome.Abandoned);
        }
        catch (Exception deadLetterExc)
        {
            Fail(deadLetterExc);
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

    /// <summary>
    ///     Collects how producing a pseudonymized message and its provenance went. The two are
    ///     reported independently - in either order, and possibly on different threads - and
    ///     whichever comes last hands both on.
    /// </summary>
    private sealed class PendingDeliveries(Action<PendingDeliveries> onAllReported)
    {
        private int pending = 2;

        public Exception OutputFailure { get; private set; }

        public ProvenancePublishingException ProvenanceFailure { get; private set; }

        public bool IsProvenanceAbandoned { get; private set; }

        public void ReportOutput(Exception failure)
        {
            OutputFailure = failure;
            ReportOne();
        }

        public void ReportProvenance(ProvenancePublishingException failure)
        {
            ProvenanceFailure = failure;
            ReportOne();
        }

        public void AbandonProvenance()
        {
            IsProvenanceAbandoned = true;
            ReportOne();
        }

        // Interlocked.Decrement is a full fence, so whichever report comes last also sees what
        // the other one recorded.
        private void ReportOne()
        {
            if (Interlocked.Decrement(ref pending) == 0)
            {
                onAllReported(this);
            }
        }
    }
}
