using System.Threading.Tasks;

namespace FhirPseudonymizer.Pseudonymization;

public interface IPseudonymServiceClient
{
    Task<string> GetOriginalValueFor(string pseudonym, string domain);
    Task<string> GetOrCreatePseudonymFor(string value, string domain);
}
