using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

namespace FhirPseudonymizer.Pseudonymization.Mii;

public class MiiFhirClient : IPseudonymServiceClient
{
    public static readonly string HttpClientName = "mii";
    private readonly ILogger<MiiFhirClient> logger;

    public MiiFhirClient(ILogger<MiiFhirClient> logger, IHttpClientFactory clientFactory)
    {
        this.logger = logger;

        ClientFactory = clientFactory;
    }

    private IHttpClientFactory ClientFactory { get; }

    public async Task<string> GetOrCreatePseudonymFor(
        string value,
        string domain,
        IReadOnlyDictionary<string, object> settings = null
    )
    {
        var parameters = new Parameters();
        parameters.Add("target", new FhirString(domain));
        parameters.Add("original", new FhirString(value));
        parameters.Add("allowCreate", new FhirBoolean(true));

        var client = ClientFactory.CreateClient(HttpClientName);

        using var fhirClient = new FhirClient(
            client.BaseAddress,
            client,
            settings: new() { PreferredFormat = ResourceFormat.Json }
        );

        var response = await fhirClient.WholeSystemOperationAsync(
            "pseudonymize",
            parameters
        );

        if (response is Parameters responseParameters)
        {
            var pseudonym = responseParameters.GetSingleValue<FhirString>("pseudonym")?.Value;
            ArgumentException.ThrowIfNullOrEmpty(pseudonym);
            return pseudonym;
        }

        throw new InvalidOperationException(
            "Pseudonymization failed. MII backend did not return a response of type 'Parameters'."
        );
    }

    public async Task<string> GetOriginalValueFor(
        string pseudonym,
        string domain,
        IReadOnlyDictionary<string, object> settings = null
    )
    {
        var parameters = new Parameters();
        parameters.Add("target", new FhirString(domain));
        parameters.Add("pseudonym", new FhirString(pseudonym));

        var client = ClientFactory.CreateClient(HttpClientName);

        using var fhirClient = new FhirClient(
            client.BaseAddress,
            client,
            settings: new() { PreferredFormat = ResourceFormat.Json }
        );

        var response = await fhirClient.WholeSystemOperationAsync(
            "de-pseudonymize",
            parameters
        );

        if (response is Parameters responseParameters)
        {
            var originalParam = responseParameters.Get("original").FirstOrDefault();
            if (originalParam is not null)
            {
                var valuePart = originalParam.Part?.FirstOrDefault(p => p.Name == "value");
                if (valuePart?.Value is Identifier identifier)
                {
                    return identifier.Value;
                }

                if (valuePart?.Value is FhirString fhirString)
                {
                    return fhirString.Value;
                }
            }

            throw new InvalidOperationException(
                "De-pseudonymization failed. Could not extract 'original.value' from the MII backend response."
            );
        }

        throw new InvalidOperationException(
            "De-pseudonymization failed. MII backend did not return a response of type 'Parameters'."
        );
    }
}
