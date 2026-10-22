using System.Net;
using Hl7.Fhir.Rest;
using Polly.Timeout;

namespace FhirPseudonymizer.Pseudonymization;

public class TransientPseudonymizationException(string message, Exception innerException)
    : Exception(message, innerException)
{
    public static bool IsTransientHttpStatus(HttpStatusCode? status) =>
        status is null
        || (int)status >= 500
        || status
            is HttpStatusCode.RequestTimeout
                or HttpStatusCode.TooManyRequests
                or HttpStatusCode.Unauthorized
                or HttpStatusCode.Forbidden;

    // A client error that retrying won't fix, i.e. the backend rejected the request itself.
    public static bool IsRejectedHttpStatus(HttpStatusCode? status) =>
        status is { } code && (int)code is >= 400 and < 500 && !IsTransientHttpStatus(code);

    // Shared by every backend client that talks HTTP/FHIR-REST (gPAS, Entici, Mii): runs their
    // call and reclassifies a transient-looking failure - one already retried, unsuccessfully, by
    // the client's own fast retry policy - as a TransientPseudonymizationException, and any other
    // 4xx as a PseudonymizationRejectedException. Anything else propagates unchanged.
    public static async Task<T> Wrap<T>(string backendName, Func<Task<T>> call)
    {
        try
        {
            return await call();
        }
        // The last attempt timing out, as given by the per-attempt timeout strategy
        catch (TimeoutRejectedException exc)
        {
            throw new TransientPseudonymizationException(
                $"{backendName} pseudonymization call timed out.",
                exc
            );
        }
        // HttpClient.Timeout elapsing - for a client that sets one - surfaces as a
        // TaskCanceledException wrapping a TimeoutException. A cancellation by the caller wraps
        // none, and still propagates unchanged.
        catch (OperationCanceledException exc) when (exc.InnerException is TimeoutException)
        {
            throw new TransientPseudonymizationException(
                $"{backendName} pseudonymization call timed out.",
                exc
            );
        }
        catch (HttpRequestException exc) when (IsTransientHttpStatus(exc.StatusCode))
        {
            throw new TransientPseudonymizationException(
                $"{backendName} pseudonymization call failed with status {exc.StatusCode}.",
                exc
            );
        }
        catch (HttpRequestException exc) when (IsRejectedHttpStatus(exc.StatusCode))
        {
            throw new PseudonymizationRejectedException(
                $"{backendName} rejected the pseudonymization request with status {exc.StatusCode}: {exc.Message}",
                exc
            );
        }
        catch (FhirOperationException exc) when (IsTransientHttpStatus(exc.Status))
        {
            throw new TransientPseudonymizationException(
                $"{backendName} pseudonymization call failed with status {exc.Status}.",
                exc
            );
        }
        catch (FhirOperationException exc) when (IsRejectedHttpStatus(exc.Status))
        {
            throw new PseudonymizationRejectedException(
                $"{backendName} rejected the pseudonymization request with status {exc.Status}: {exc.Message}",
                exc
            );
        }
    }
}
