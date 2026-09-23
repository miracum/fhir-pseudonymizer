using System.Net;

namespace FhirPseudonymizer.Pseudonymization;

public class TransientPseudonymizationException(string message, Exception innerException)
    : Exception(message, innerException)
{
    public static bool IsTransientHttpStatus(HttpStatusCode? status) =>
        status is null
        || (int)status >= 500
        || status is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests;
}
