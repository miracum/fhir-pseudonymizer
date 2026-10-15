using System.Diagnostics;
using System.Diagnostics.Metrics;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

namespace FhirPseudonymizer.Pseudonymization.GPas;

public class GPasFhirClient(ILogger<GPasFhirClient> logger, IHttpClientFactory clientFactory)
    : IPseudonymServiceClient
{
    public static readonly string HttpClientName = "gPAS";

    private static readonly Counter<long> TotalGPasRequests = Program.Meter.CreateCounter<long>(
        "fhirpseudonymizer.gpas.requests",
        description: "Total number of requests against the gPas service."
    );

    public async Task<string> GetOrCreatePseudonymFor(
        string value,
        string domain,
        IReadOnlyDictionary<string, object> settings = null,
        CancellationToken cancellationToken = default
    )
    {
        TotalGPasRequests.Add(1, new TagList { { "operation", nameof(GetOrCreatePseudonymFor) } });

        return await TransientPseudonymizationException.Wrap(
            "gPAS",
            async () =>
            {
                var responseParameters = await RequestOperation(
                    "pseudonymizeAllowCreate",
                    new Parameters()
                        .Add("target", new FhirString(domain))
                        .Add("original", new FhirString(value)),
                    cancellationToken
                );

                var firstResponseParameter = responseParameters.Parameter.FirstOrDefault();
                var pseudonym = firstResponseParameter?.Part.Find(part => part.Name == "pseudonym");
                if (pseudonym?.Value is not Identifier pseudonymIdentifier)
                {
                    throw new InvalidOperationException("No pseudonym included in gPAS response.");
                }

                return pseudonymIdentifier.Value;
            }
        );
    }

    public async Task<string> GetOriginalValueFor(
        string pseudonym,
        string domain,
        IReadOnlyDictionary<string, object> settings = null,
        CancellationToken cancellationToken = default
    )
    {
        TotalGPasRequests.Add(1, new TagList { { "operation", nameof(GetOriginalValueFor) } });

        try
        {
            var responseParameters = await RequestOperation(
                "dePseudonymize",
                new Parameters()
                    .Add("target", new FhirString(domain))
                    .Add("pseudonym", new FhirString(pseudonym)),
                cancellationToken
            );

            var firstResponseParameter = responseParameters.Parameter.FirstOrDefault();
            var original = firstResponseParameter?.Part.Find(part => part.Name == "original");
            if (original?.Value is Identifier originalIdentifier)
            {
                return originalIdentifier.Value;
            }

            logger.LogError("Failed to de-pseudonymize. Returning original value.");
            return pseudonym;
        }
        // A caller-requested cancellation has to propagate: falling through to the fallback
        // below would silently return the pseudonym as if de-pseudonymization had failed, for
        // this and every remaining field of the resource. An HttpClient *timeout* also surfaces
        // as OperationCanceledException, but with the token unsignalled - that case still takes
        // the fallback, as before.
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exc)
        {
            logger.LogError(exc, "Failed to de-pseudonymize. Returning original value.");
            return pseudonym;
        }
    }

    private async Task<Parameters> RequestOperation(
        string operation,
        Parameters parameters,
        CancellationToken cancellationToken
    )
    {
        var client = clientFactory.CreateClient(HttpClientName);

        using var fhirClient = new FhirClient(
            client.BaseAddress,
            client,
            settings: new() { PreferredFormat = ResourceFormat.Json }
        );

        var response = await fhirClient.WholeSystemOperationAsync(
            operation,
            parameters,
            ct: cancellationToken
        );

        return response as Parameters;
    }
}
