namespace FhirPseudonymizer.Kafka;

/// <summary>
///     The provenance of a pseudonymized message could not be published, see
///     <see cref="IProvenancePublisher.PublishAsync" />. Its own type so that the dead letter topic's
///     x-error-type header tells this apart from a failure to deliver the pseudonymized message
///     itself.
/// </summary>
public class ProvenancePublishingException(string message, Exception innerException)
    : Exception(message, innerException);
