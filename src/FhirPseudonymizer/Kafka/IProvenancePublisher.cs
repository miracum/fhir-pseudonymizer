using Confluent.Kafka;
using Hl7.Fhir.Model;

namespace FhirPseudonymizer.Kafka;

/// <summary>
///     Publishes a Bundle of Provenance resources documenting the pseudonymization of a resource
///     (see <see cref="ProvenanceFactory" />) to <see cref="Config.KafkaConfig.ProvenanceTopic" />,
///     from both the Kafka consumer and the REST <c>$de-identify</c> paths. Implementations must
///     not throw: a failure to record provenance must never fail an otherwise-successful
///     pseudonymization.
/// </summary>
public interface IProvenancePublisher
{
    /// <param name="original">The resource as it was before pseudonymization, used to determine which security labels it gained, see <see cref="ProvenanceFactory" />.</param>
    /// <param name="pseudonymized">The resource after pseudonymization, referenced by the produced Provenance(s).</param>
    /// <param name="key">The Kafka message key to publish the provenance bundle with, if applicable (e.g. the source message's key, to preserve partition affinity); omitted when there is none, e.g. from the REST endpoint.</param>
    /// <param name="headers">Headers to copy onto the produced provenance bundle's Kafka message, if applicable (e.g. tracing context forwarded from the source Kafka message).</param>
    void Publish(
        Resource original,
        Resource pseudonymized,
        byte[] key = null,
        Headers headers = null
    );
}
