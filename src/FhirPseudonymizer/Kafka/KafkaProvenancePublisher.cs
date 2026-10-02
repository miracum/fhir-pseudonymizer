using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using FhirPseudonymizer.Config;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace FhirPseudonymizer.Kafka;

public class KafkaProvenancePublisher : IProvenancePublisher
{
    private readonly IProducer<byte[], string> producer;
    private readonly KafkaConfig kafkaConfig;
    private readonly ILogger<KafkaProvenancePublisher> logger;
    private static readonly JsonSerializerOptions FhirJsonOptions =
        new JsonSerializerOptions().ForFhir(ModelInfo.ModelInspector);

    public KafkaProvenancePublisher(
        IProducer<byte[], string> producer,
        KafkaConfig kafkaConfig,
        ILogger<KafkaProvenancePublisher> logger
    )
    {
        this.producer = producer;
        this.kafkaConfig = kafkaConfig;
        this.logger = logger;
    }

    public Resource CapturePreImage(Resource resource) => (Resource)resource?.DeepCopy();

    public void Publish(Resource original, Resource pseudonymized, Headers headers = null)
    {
        var bundle = ProvenanceFactory.CreateBundle(original, pseudonymized, DateTimeOffset.UtcNow);
        if (bundle is null)
        {
            return;
        }

        try
        {
            producer.Produce(
                kafkaConfig.ProvenanceTopic,
                CreateMessage(bundle, headers),
                report =>
                {
                    if (report.Error.IsError)
                    {
                        logger.LogError(
                            "Failed to deliver provenance bundle {BundleId} to topic {Topic}: {Reason}",
                            bundle.Id,
                            kafkaConfig.ProvenanceTopic,
                            report.Error.Reason
                        );
                    }
                }
            );
        }
        catch (KafkaException exc)
        {
            logger.LogError(
                exc,
                "Failed to produce provenance bundle to topic {Topic}",
                kafkaConfig.ProvenanceTopic
            );
        }
    }

    public async System.Threading.Tasks.Task PublishAsync(
        Resource original,
        Resource pseudonymized,
        Headers headers,
        Action<ProvenancePublishingException> onCompleted,
        CancellationToken cancellationToken
    )
    {
        Bundle bundle;
        Message<byte[], string> message;
        try
        {
            bundle = ProvenanceFactory.CreateBundle(original, pseudonymized, DateTimeOffset.UtcNow);
            message = bundle is null ? null : CreateMessage(bundle, headers);
        }
        catch (Exception exc)
        {
            onCompleted(
                new ProvenancePublishingException(
                    $"Failed to create the provenance bundle: {exc.Message}",
                    exc
                )
            );
            return;
        }

        if (message is null)
        {
            // there is nothing to document, see ProvenanceFactory.CreateBundle
            onCompleted(null);
            return;
        }

        try
        {
            await producer.ProduceWaitingForRoomAsync(
                kafkaConfig.ProvenanceTopic,
                message,
                report =>
                    onCompleted(
                        report.Error.IsError
                            ? new ProvenancePublishingException(
                                $"Failed to deliver provenance bundle {bundle.Id} to topic {kafkaConfig.ProvenanceTopic}: {report.Error.Reason}",
                                new ProduceException<byte[], string>(report.Error, report)
                            )
                            : null
                    ),
                cancellationToken
            );
        }
        catch (Exception exc) when (exc is not OperationCanceledException)
        {
            // e.g. the bundle exceeds message.max.bytes
            onCompleted(
                new ProvenancePublishingException(
                    $"Failed to produce provenance bundle {bundle.Id} to topic {kafkaConfig.ProvenanceTopic}: {exc.Message}",
                    exc
                )
            );
        }
    }

    private static Message<byte[], string> CreateMessage(Bundle bundle, Headers headers) =>
        new()
        {
            Key = Encoding.UTF8.GetBytes(bundle.Id),
            Value = JsonSerializer.Serialize(bundle, FhirJsonOptions),
            Headers = headers,
        };
}
