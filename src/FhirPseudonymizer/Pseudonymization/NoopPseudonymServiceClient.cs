namespace FhirPseudonymizer.Pseudonymization;

public class NoopPseudonymServiceClient : IPseudonymServiceClient
{
    public Task<string> GetOrCreatePseudonymFor(
        string value,
        string domain,
        IReadOnlyDictionary<string, object> settings = null
    )
    {
        throw new InvalidOperationException(
            "PseudonymizationService config is set to 'None' but configuration used pseudonymization method."
        );
    }

    public Task<string> GetOriginalValueFor(
        string pseudonym,
        string domain,
        IReadOnlyDictionary<string, object> settings = null
    )
    {
        throw new InvalidOperationException(
            "PseudonymizationService config is set to 'None' but configuration used pseudonymization method."
        );
    }
}
