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
    private readonly FhirJsonSerializer fhirJsonSerializer = new();

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

    public void Publish(
        Resource original,
        Resource pseudonymized,
        byte[] key = null,
        Headers headers = null
    )
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
                    Key = key,
                    Value = fhirJsonSerializer.SerializeToString(bundle),
                    Headers = headers,
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
