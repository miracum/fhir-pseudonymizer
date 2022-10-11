using System.Threading.Tasks;
using Vfps.Protos;

namespace FhirPseudonymizer.Pseudonymization.Vfps;

public class VfpsPseudonymServiceClient : IPseudonymServiceClient
{
    public VfpsPseudonymServiceClient(PseudonymService.PseudonymServiceClient client)
    {
        Client = client;
    }

    private PseudonymService.PseudonymServiceClient Client { get; }

    public async Task<string> GetOrCreatePseudonymFor(string value, string domain)
    {
        var request = new PseudonymServiceCreateRequest
        {
            OriginalValue = value,
            Namespace = domain,
        };

        var response = await Client.CreateAsync(request);

        return response.Pseudonym.PseudonymValue;
    }

    public async Task<string> GetOriginalValueFor(string pseudonym, string domain)
    {
        var request = new PseudonymServiceGetRequest
        {
            PseudonymValue = pseudonym,
            Namespace = domain,
        };

        var response = await Client.GetAsync(request);

        return response.Pseudonym.OriginalValue;
    }
}
