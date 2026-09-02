using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

namespace FhirPseudonymizer.Pseudonymization.Mii;

/// <summary>
/// A client for the $pseudonymize and $de-pseudonymize operations of the MII
/// Pseudonymization Implementation Guide 2026.1.0.
/// </summary>
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

    /// <summary>
    /// Reads the optional identifier system with the given key from the rule's
    /// 'mii' settings. Returns null if it is not set, so the identifier is sent
    /// with only a value.
    /// </summary>
    private static string GetIdentifierSystem(
        IReadOnlyDictionary<string, object> settings,
        string key
    )
    {
        if (
            settings is not null
            && settings.TryGetValue("mii", out var miiSettingsObject)
            && miiSettingsObject is IReadOnlyDictionary<object, object> miiSettings
            && miiSettings.TryGetValue(key, out var system)
        )
        {
            return system?.ToString();
        }

        return null;
    }

    private FhirClient CreateFhirClient()
    {
        var client = ClientFactory.CreateClient(HttpClientName);

        return new FhirClient(
            client.BaseAddress,
            client,
            settings: new() { PreferredFormat = ResourceFormat.Json }
        );
    }

    public async Task<string> GetOrCreatePseudonymFor(
        string value,
        string domain,
        IReadOnlyDictionary<string, object> settings = null
    )
    {
        var request = new MiiPseudonymizeRequest
        {
            Context = new Identifier(GetIdentifierSystem(settings, "contextSystem"), domain),
            Original = new Identifier(GetIdentifierSystem(settings, "originalSystem"), value),
        };

        using var fhirClient = CreateFhirClient();

        var response = await fhirClient.WholeSystemOperationAsync(
            "pseudonymize",
            request.ToFhirParameters()
        );

        if (response is Parameters responseParameters)
        {
            var pseudonym = MiiPseudonymizeResponse
                .FromFhirParameters(responseParameters)
                .Pseudonym?.Value;
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
        var request = new MiiDePseudonymizeRequest
        {
            Context = new Identifier(GetIdentifierSystem(settings, "contextSystem"), domain),
            Pseudonym = new Identifier(null, pseudonym),
        };

        using var fhirClient = CreateFhirClient();

        var response = await fhirClient.WholeSystemOperationAsync(
            "de-pseudonymize",
            request.ToFhirParameters()
        );

        if (response is Parameters responseParameters)
        {
            var original = MiiDePseudonymizeResponse
                .FromFhirParameters(responseParameters)
                .Original?.FirstOrDefault();

            if (original?.Value?.Value is { Length: > 0 } originalValue)
            {
                return originalValue;
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
