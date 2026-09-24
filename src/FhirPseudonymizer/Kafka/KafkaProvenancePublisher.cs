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
                new Message<byte[], string>
                {
                    Key = Encoding.UTF8.GetBytes(bundle.Id),
                    Value = JsonSerializer.Serialize(bundle, FhirJsonOptions),
                    Headers = headers,
                },
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
}
