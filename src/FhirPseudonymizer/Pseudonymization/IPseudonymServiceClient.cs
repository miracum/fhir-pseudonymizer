namespace FhirPseudonymizer.Pseudonymization;

public interface IPseudonymServiceClient
{
    Task<string> GetOriginalValueFor(
        string pseudonym,
        string domain,
        IReadOnlyDictionary<string, object> settings = null
    );
    Task<string> GetOrCreatePseudonymFor(
        string value,
        string domain,
        IReadOnlyDictionary<string, object> settings = null
    );
}
