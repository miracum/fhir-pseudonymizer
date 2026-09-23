using System.Diagnostics;
using System.Diagnostics.Metrics;
using FhirPseudonymizer.Config;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.WebUtilities;
using Semver;

namespace FhirPseudonymizer.Pseudonymization.GPas;

public class GPasFhirClient : IPseudonymServiceClient
{
    public static readonly string HttpClientName = "gPAS";

    private static readonly Counter<long> TotalGPasRequests = Program.Meter.CreateCounter<long>(
        "fhirpseudonymizer.gpas.requests",
        description: "Total number of requests against the gPas service."
    );

    private readonly ILogger<GPasFhirClient> logger;

    public GPasFhirClient(
        ILogger<GPasFhirClient> logger,
        IHttpClientFactory clientFactory,
        GPasConfig config
    )
    {
        this.logger = logger;

        ClientFactory = clientFactory;

        var configGPasVersion = config.Version;
        var supportedGPasVersion = SemVersion.Parse(configGPasVersion);

        logger.LogInformation($"Configured gPAS version {supportedGPasVersion}");

        if (supportedGPasVersion.CompareSortOrderTo(SemVersion.Parse("1.10.2")) < 0)
        {
            logger.LogInformation("Using gPAS API version < 1.10.2.");
            GetOrCreatePseudonymForResolver = GetOrCreatePseudonymForV1;
            GetOriginalValueForResolver = GetOriginalValueForV1;
        }
        else if (supportedGPasVersion == SemVersion.Parse("1.10.2"))
        {
            logger.LogInformation("Using gPAS API version == 1.10.2.");
            GetOrCreatePseudonymForResolver = GetOrCreatePseudonymForV2;
            GetOriginalValueForResolver = GetOriginalValueForV2;
        }
        else
        {
            logger.LogInformation("Using gPAS API version > 1.10.2");
            GetOrCreatePseudonymForResolver = GetOrCreatePseudonymForV2x;
            GetOriginalValueForResolver = GetOriginalValueForV2x;
        }
    }

    private IHttpClientFactory ClientFactory { get; }
    private FhirJsonParser FhirParser { get; } = new();
    private PseudonymResolver GetOrCreatePseudonymForResolver { get; }
    private PseudonymResolver GetOriginalValueForResolver { get; }

    /// <summary>
    ///     Resolves a value in a domain against the gPAS API version this client was configured
    ///     for - either an original to its pseudonym, or the other way around.
    /// </summary>
    private delegate Task<string> PseudonymResolver(
        string value,
        string domain,
        CancellationToken cancellationToken
    );

    public async Task<string> GetOrCreatePseudonymFor(
        string value,
        string domain,
        IReadOnlyDictionary<string, object> settings = null,
        CancellationToken cancellationToken = default
    )
    {
        TotalGPasRequests.Add(1, new TagList { { "operation", nameof(GetOrCreatePseudonymFor) } });

        try
        {
            return await GetOrCreatePseudonymForResolver(value, domain, cancellationToken);
        }
        catch (HttpRequestException exc)
            when (TransientPseudonymizationException.IsTransientHttpStatus(exc.StatusCode))
        {
            throw new TransientPseudonymizationException(
                $"gPAS pseudonymization call failed with status {exc.StatusCode}.",
                exc
            );
        }
        catch (FhirOperationException exc)
            when (TransientPseudonymizationException.IsTransientHttpStatus(exc.Status))
        {
            throw new TransientPseudonymizationException(
                $"gPAS pseudonymization call failed with status {exc.Status}.",
                exc
            );
        }
    }

    public async Task<string> GetOriginalValueFor(
        string pseudonym,
        string domain,
        IReadOnlyDictionary<string, object> settings = null,
        CancellationToken cancellationToken = default
    )
    {
        TotalGPasRequests.Add(1, new TagList { { "operation", nameof(GetOriginalValueFor) } });

        return await GetOriginalValueForResolver(pseudonym, domain, cancellationToken);
    }

    private async Task<string> GetOriginalValueForV1(
        string pseudonym,
        string domain,
        CancellationToken cancellationToken
    )
    {
        var client = ClientFactory.CreateClient(HttpClientName);

        var query = new Dictionary<string, string>
        {
            ["domain"] = domain,
            ["pseudonym"] = pseudonym,
        };

        var response = await client.GetAsync(
            QueryHelpers.AddQueryString("$de-pseudonymize", query),
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var parameters = FhirParser.Parse<Parameters>(content);

        var original = parameters.GetSingleValue<FhirString>(pseudonym);
        if (original == null)
        {
            logger.LogWarning("Failed to de-pseudonymize. Returning original value.");
            return pseudonym;
        }

        return original.Value;
    }

    private async Task<string> GetOriginalValueForV2(
        string pseudonym,
        string domain,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var responseParameters = await RequestGetOriginalValueForV2(
                pseudonym,
                domain,
                "de-pseudonymize",
                cancellationToken
            );

            var pseudonymResultSet = responseParameters.Get("pseudonym-result-set").First();
            var originalPart = pseudonymResultSet.Part.Find(component =>
                component.Name == "original"
            );

            return originalPart.Value.ToString();
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

    private async Task<string> GetOriginalValueForV2x(
        string pseudonym,
        string domain,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var responseParameters = await RequestGetOriginalValueForV2(
                pseudonym,
                domain,
                "dePseudonymize",
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

    private async Task<string> GetOrCreatePseudonymForV1(
        string value,
        string domain,
        CancellationToken cancellationToken
    )
    {
        var client = ClientFactory.CreateClient(HttpClientName);

        var query = new Dictionary<string, string> { ["domain"] = domain, ["original"] = value };

        // this currently uses a HttpClient instead of the FhirClient to leverage
        // Polly, tracing, and metrics support. Once FhirClient allows for overriding the HttpClient,
        // we can simplify this code a lot: https://github.com/FirelyTeam/firely-net-sdk/issues/1483
        var response = await client.GetAsync(
            QueryHelpers.AddQueryString("$pseudonymize-allow-create", query),
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var parameters = FhirParser.Parse<Parameters>(content);
        return parameters.GetSingleValue<FhirString>(value).Value;
    }

    private async Task<string> GetOrCreatePseudonymForV2(
        string value,
        string domain,
        CancellationToken cancellationToken
    )
    {
        var responseParameters = await RequestGetOrCreatePseudonymForV2(
            value,
            domain,
            "pseudonymize-allow-create",
            cancellationToken
        );

        var firstResponseParameter = responseParameters.Parameter.FirstOrDefault();
        var pseudonym = firstResponseParameter?.Part.Find(part => part.Name == "pseudonym");
        if (pseudonym?.Value == null)
        {
            throw new InvalidOperationException("No pseudonym included in gPAS response.");
        }

        return pseudonym.Value.ToString();
    }

    private async Task<string> GetOrCreatePseudonymForV2x(
        string value,
        string domain,
        CancellationToken cancellationToken
    )
    {
        var responseParameters = await RequestGetOrCreatePseudonymForV2(
            value,
            domain,
            "pseudonymizeAllowCreate",
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

    private async Task<Parameters> RequestGetOrCreatePseudonymForV2(
        string value,
        string domain,
        string operation,
        CancellationToken cancellationToken
    )
    {
        var client = ClientFactory.CreateClient(HttpClientName);

        using var fhirClient = new FhirClient(
            client.BaseAddress,
            client,
            settings: new() { PreferredFormat = ResourceFormat.Json }
        );

        var parameters = new Parameters()
            .Add("target", new FhirString(domain))
            .Add("original", new FhirString(value));

        var response = await fhirClient.WholeSystemOperationAsync(
            operation,
            parameters,
            ct: cancellationToken
        );

        return response as Parameters;
    }

    private async Task<Parameters> RequestGetOriginalValueForV2(
        string pseudonym,
        string domain,
        string operation,
        CancellationToken cancellationToken
    )
    {
        var client = ClientFactory.CreateClient(HttpClientName);

        using var fhirClient = new FhirClient(
            client.BaseAddress,
            client,
            settings: new() { PreferredFormat = ResourceFormat.Json }
        );

        var parameters = new Parameters()
            .Add("target", new FhirString(domain))
            .Add("pseudonym", new FhirString(pseudonym));

        var response = await fhirClient.WholeSystemOperationAsync(
            operation,
            parameters,
            ct: cancellationToken
        );

        return response as Parameters;
    }
}
