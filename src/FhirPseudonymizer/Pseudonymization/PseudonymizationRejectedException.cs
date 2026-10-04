namespace FhirPseudonymizer.Pseudonymization;

/// <summary>
///     The pseudonymization backend rejected a request - e.g. Vfps because the value doesn't
///     match its namespace's required pattern, or gPAS with a 4xx status - so, unlike after a
///     <see cref="TransientPseudonymizationException" />, retrying the same input won't help.
///     Its own type so callers can tell a rejected input apart from an unexpected failure: the
///     REST API responds with 422 instead of 500.
/// </summary>
public class PseudonymizationRejectedException(string message, Exception innerException)
    : Exception(message, innerException);
