using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vfps.Protos;

namespace FhirPseudonymizer.Pseudonymization.Vfps;

public class VfpsPseudonymServiceClient : IPseudonymServiceClient
{
    private readonly ILogger<VfpsPseudonymServiceClient> logger;

    public VfpsPseudonymServiceClient(
        ILogger<VfpsPseudonymServiceClient> logger,
        PseudonymService.PseudonymServiceClient client
    )
    {
        Client = client;
        this.logger = logger;
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

        try
        {
            var response = await Client.GetAsync(request);
            return response.Pseudonym.OriginalValue;
        }
        catch (Exception exc)
        {
            logger.LogWarning(
                exc,
                "Failed to de-pseudonymize. Returning pseudonymized value.",
                pseudonym
            );
            return pseudonym;
        }
    }
}
