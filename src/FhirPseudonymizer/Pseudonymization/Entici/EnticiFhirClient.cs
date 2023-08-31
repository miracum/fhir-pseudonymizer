using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

namespace FhirPseudonymizer.Pseudonymization.Entici;

public class EnticiFhirClient : IPseudonymServiceClient
{
    private readonly ILogger<EnticiFhirClient> logger;

    public EnticiFhirClient(ILogger<EnticiFhirClient> logger, IHttpClientFactory clientFactory)
    {
        this.logger = logger;

        var client = clientFactory.CreateClient("Entici");

        FhirClient = new FhirClient(
            client.BaseAddress,
            client,
            settings: new() { PreferredFormat = ResourceFormat.Json }
        );
    }

    private FhirClient FhirClient { get; }

    public async Task<string> GetOrCreatePseudonymFor(
        string value,
        string domain,
        IReadOnlyDictionary<string, object> settings = null
    )
    {
        ArgumentNullException.ThrowIfNull(settings);

        var hasEnticiSettings = settings.TryGetValue("entici", out var enticiSettingsObject);
        if (!hasEnticiSettings || enticiSettingsObject is null)
        {
            throw new InvalidOperationException(
                "Pseudonymization using Entici requires a special settings object as part of the rule definition."
            );
        }

        var enticiSettings = enticiSettingsObject as IReadOnlyDictionary<object, object>;

        // this should throw if resourceType is unset.
        var resourceType = Enum.Parse<ResourceType>(enticiSettings["resourceType"].ToString());

        var request = new EnticiPseudonymizationRequest
        {
            Identifier = new Identifier(domain, value),
            ResourceType = new Code(resourceType.ToString()),
        };

        var hasTargetSystem = enticiSettings.TryGetValue("project", out var targetSystemObject);
        if (hasTargetSystem)
        {
            request.Project = new FhirString(targetSystemObject.ToString());
        }

        var response = await FhirClient.WholeSystemOperationAsync(
            "pseudonymize",
            request.ToFhirParameters()
        );
        if (response is Parameters responseParameters)
        {
            var pseudonym = responseParameters.GetSingleValue<Identifier>("pseudonym");
            ArgumentException.ThrowIfNullOrEmpty(pseudonym?.Value, nameof(pseudonym));
            return pseudonym.Value;
        }
        else
        {
            throw new InvalidOperationException(
                "Pseudonymization failed. Entici backend did not return a response of type 'Parameters'."
            );
        }
    }

    public Task<string> GetOriginalValueFor(
        string pseudonym,
        string domain,
        IReadOnlyDictionary<string, object> settings = null
    )
    {
        throw new NotImplementedException(
            "De-Pseudonymization is not yet implemented for the Entici backend."
        );
    }
}
