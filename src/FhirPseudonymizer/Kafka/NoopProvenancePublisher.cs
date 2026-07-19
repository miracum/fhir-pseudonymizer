using Confluent.Kafka;
using Hl7.Fhir.Model;

namespace FhirPseudonymizer.Kafka;

/// <summary>
///     Used when <see cref="Config.KafkaConfig.ProvenanceTopic" /> is not configured, so that
///     callers can unconditionally publish provenance without checking whether it is enabled.
/// </summary>
public class NoopProvenancePublisher : IProvenancePublisher
{
    public void Publish(
        Resource original,
        Resource pseudonymized,
        byte[] key = null,
        Headers headers = null
    ) { }
}
