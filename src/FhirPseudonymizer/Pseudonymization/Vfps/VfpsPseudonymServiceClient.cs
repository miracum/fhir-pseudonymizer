using Grpc.Core;
using Vfps.Protos;

namespace FhirPseudonymizer.Pseudonymization.Vfps;

public class VfpsPseudonymServiceClient(
    ILogger<VfpsPseudonymServiceClient> logger,
    PseudonymService.PseudonymServiceClient client
) : IPseudonymServiceClient
{
    // Also used by VfpsExtensions to configure the gRPC channel's own fast retry policy, so a
    // status code only needs to be added here once to be treated as transient by both.
    internal static readonly HashSet<StatusCode> TransientStatusCodes =
    [
        StatusCode.Unavailable,
        StatusCode.Internal,
        StatusCode.Unauthenticated,
        StatusCode.PermissionDenied,
    ];

    // The request itself was rejected, e.g. InvalidArgument for a value that doesn't match the
    // namespace's required pattern, or NotFound for a namespace that doesn't exist.
    internal static readonly HashSet<StatusCode> RejectedStatusCodes =
    [
        StatusCode.InvalidArgument,
        StatusCode.FailedPrecondition,
        StatusCode.NotFound,
        StatusCode.AlreadyExists,
        StatusCode.OutOfRange,
    ];

    private readonly ILogger<VfpsPseudonymServiceClient> logger = logger;

    private PseudonymService.PseudonymServiceClient Client { get; } = client;

    public async Task<string> GetOrCreatePseudonymFor(
        string value,
        string domain,
        IReadOnlyDictionary<string, object> settings = null,
        CancellationToken cancellationToken = default
    )
    {
        var request = new PseudonymServiceCreateRequest
        {
            OriginalValue = value,
            Namespace = domain,
        };

        try
        {
            var response = await Client.CreateAsync(request, cancellationToken: cancellationToken);
            return response.Pseudonym.PseudonymValue;
        }
        // The call's deadline elapsing (see DefaultDeadlineInterceptor). With
        // ThrowOperationCanceledOnCancellation set this surfaces as an OperationCanceledException
        // while the caller's own token is not cancelled; the RpcException is handled too in case
        // the server reports the deadline itself. Not part of TransientStatusCodes, which also
        // configures the channel's retries: there's no time left to retry once it has elapsed.
        catch (OperationCanceledException exc) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TransientPseudonymizationException(
                "Vfps pseudonymization call exceeded its deadline.",
                exc
            );
        }
        catch (RpcException exc) when (exc.StatusCode == StatusCode.DeadlineExceeded)
        {
            throw new TransientPseudonymizationException(
                "Vfps pseudonymization call exceeded its deadline.",
                exc
            );
        }
        catch (RpcException exc) when (TransientStatusCodes.Contains(exc.StatusCode))
        {
            throw new TransientPseudonymizationException(
                $"Vfps pseudonymization call failed with status {exc.StatusCode}.",
                exc
            );
        }
        catch (RpcException exc) when (RejectedStatusCodes.Contains(exc.StatusCode))
        {
            throw new PseudonymizationRejectedException(
                $"Vfps rejected the pseudonymization request with status {exc.StatusCode}: {exc.Status.Detail}",
                exc
            );
        }
    }

    public async Task<string> GetOriginalValueFor(
        string pseudonym,
        string domain,
        IReadOnlyDictionary<string, object> settings = null,
        CancellationToken cancellationToken = default
    )
    {
        var request = new PseudonymServiceGetRequest
        {
            PseudonymValue = pseudonym,
            Namespace = domain,
        };

        try
        {
            var response = await Client.GetAsync(request, cancellationToken: cancellationToken);
            return response.Pseudonym.OriginalValue;
        }
        // See GPasFhirClient: a caller-requested cancellation must not be swallowed into the
        // "return the pseudonym unchanged" fallback, or a cancelled $de-pseudonymize would
        // silently produce still-pseudonymized output instead of aborting.
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exc)
        {
            logger.LogWarning(
                exc,
                "Failed to de-pseudonymize {Pseudonym}. Returning pseudonymized value.",
                pseudonym
            );
            return pseudonym;
        }
    }
}
