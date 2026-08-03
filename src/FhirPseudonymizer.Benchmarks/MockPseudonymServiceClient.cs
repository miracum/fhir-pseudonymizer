using FhirPseudonymizer.Pseudonymization;

namespace FhirPseudonymizer.Benchmarks;

/// <summary>
///     A stand-in for a real gPAS/vfps/Entici/MII pseudonymization service client used so the
///     benchmarks can exercise the "pseudonymize" processor without any network I/O.
/// </summary>
public class MockPseudonymServiceClient : IPseudonymServiceClient
{
    public Task<string> GetOrCreatePseudonymFor(
        string value,
        string domain,
        IReadOnlyDictionary<string, object> settings = null
    ) => Task.FromResult($"pseudonym-for-{value}-in-{domain}");

    public Task<string> GetOriginalValueFor(
        string pseudonym,
        string domain,
        IReadOnlyDictionary<string, object> settings = null
    ) => Task.FromResult($"original-for-{pseudonym}-in-{domain}");
}
