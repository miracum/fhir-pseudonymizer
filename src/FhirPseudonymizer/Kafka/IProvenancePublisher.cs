using Confluent.Kafka;
using Hl7.Fhir.Model;

namespace FhirPseudonymizer.Kafka;

/// <summary>
///     Publishes a Bundle of Provenance resources documenting the pseudonymization of a resource
///     (see <see cref="ProvenanceFactory" />) to <see cref="Config.KafkaConfig.ProvenanceTopic" />:
///     best-effort for the REST <c>$de-identify</c> path (<see cref="Publish" />), and with the
///     same delivery guarantees as the pseudonymized message itself for the Kafka consumer
///     (<see cref="PublishAsync" />).
/// </summary>
public interface IProvenancePublisher
{
    /// <summary>
    ///     Takes the snapshot of <paramref name="resource" /> that a later <see cref="Publish" />
    ///     call needs as its <c>original</c>, to be called before the resource is handed to the
    ///     anonymizer. The anonymizer mutates the resource it is given in place and hands back that
    ///     same instance, so without snapshotting first there is no pre-pseudonymization state left
    ///     to describe and the Provenance's <c>entity[role=source]</c> would just point back at the
    ///     pseudonymized resource.
    ///     <para>
    ///         Implementations that do not record provenance return null rather than paying for a
    ///         copy, so callers can keep calling this unconditionally the same way they call
    ///         <see cref="Publish" />.
    ///     </para>
    /// </summary>
    /// <param name="resource">The resource about to be pseudonymized.</param>
    /// <returns>A snapshot to pass as <c>original</c>, or null if provenance is not recorded.</returns>
    Resource CapturePreImage(Resource resource);

    /// <summary>
    ///     Publishes provenance information for a pseudonymization operation, best-effort: a
    ///     failure to record it is only logged, and never fails an otherwise-successful
    ///     pseudonymization, so this must not throw. The produced Kafka message is keyed with the
    ///     (single) Provenance's own id (which also becomes the Bundle's id, see
    ///     <see cref="ProvenanceFactory" />), not any key belonging to the source message - since
    ///     that id is deterministic, this keeps re-publishing provenance for the same input on the
    ///     same partition, e.g. for log compaction.
    /// </summary>
    /// <param name="original">The resource as it was before pseudonymization, used to determine which security labels it gained, see <see cref="ProvenanceFactory" />. Obtain it from <see cref="CapturePreImage" /> before the resource reaches the anonymizer.</param>
    /// <param name="pseudonymized">The resource after pseudonymization, referenced by the produced Provenance(s).</param>
    /// <param name="headers">Headers to copy onto the produced provenance bundle's Kafka message, if applicable (e.g. tracing context forwarded from the source Kafka message).</param>
    void Publish(Resource original, Resource pseudonymized, Headers headers = null);

    /// <summary>
    ///     Like <see cref="Publish" />, but for a caller that must know whether the provenance was
    ///     recorded: the Kafka consumer, which only marks a message as consumed once it was. A full
    ///     local producer queue is waited out rather than given up on, and
    ///     <paramref name="onCompleted" /> is called exactly once - with <c>null</c> once the
    ///     broker acknowledged the provenance message, or right away if there is no provenance to
    ///     record (e.g. since that's disabled), and with a
    ///     <see cref="ProvenancePublishingException" /> otherwise. It may be called from the
    ///     producer's delivery report thread, so must be fast and thread-safe.
    ///
    ///     The returned task completes as soon as the provenance message has been handed to the
    ///     producer. Throws an <see cref="OperationCanceledException" /> - without calling
    ///     <paramref name="onCompleted" /> - if <paramref name="cancellationToken" /> is cancelled
    ///     while waiting for room in the producer's queue, but no other exception.
    /// </summary>
    System.Threading.Tasks.Task PublishAsync(
        Resource original,
        Resource pseudonymized,
        Headers headers,
        Action<ProvenancePublishingException> onCompleted,
        CancellationToken cancellationToken
    );
}
