using System.Text;
using FhirPseudonymizer.Config;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Prometheus;
using Semver;

namespace FhirPseudonymizer.Pseudonymization.GPas;

public class GPasFhirClient : IPseudonymServiceClient
{
    private static readonly Counter TotalGPasRequests = Metrics.CreateCounter(
        "fhirpseudonymizer_gpas_requests_total",
        "Total number of requests against the gPas service.",
        new CounterConfiguration() { LabelNames = new[] { "operation" }, }
    );

    private static readonly Counter TotalGPasRequestCacheMisses = Metrics.CreateCounter(
        "fhirpseudonymizer_gpas_requests_cache_misses_total",
        "Total number of requests against gPas that could not be resolved via the internal cache.",
        new CounterConfiguration() { LabelNames = new[] { "operation" }, }
    );

    private readonly ILogger<GPasFhirClient> logger;

    public GPasFhirClient(
        ILogger<GPasFhirClient> logger,
        IHttpClientFactory clientFactory,
        GPasConfig config,
        IMemoryCache pseudonymCache,
        IMemoryCache originalValueCache
    )
    {
        this.logger = logger;

        Client = clientFactory.CreateClient("gPAS");

        FhirClient = new FhirClient(Client.BaseAddress, Client);

        PseudonymCache = pseudonymCache;
        OriginalValueCache = originalValueCache;

        SlidingExpiration = TimeSpan.FromMinutes(config.Cache.SlidingExpirationMinutes);
        AbsoluteExpiration = TimeSpan.FromMinutes(config.Cache.AbsoluteExpirationMinutes);

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

    private IMemoryCache PseudonymCache { get; }
    private IMemoryCache OriginalValueCache { get; }
    private HttpClient Client { get; }
    private FhirJsonParser FhirParser { get; } = new();
    private FhirJsonSerializer FhirSerializer { get; } = new();
    private TimeSpan SlidingExpiration { get; }
    private TimeSpan AbsoluteExpiration { get; }
    private Func<string, string, Task<string>> GetOrCreatePseudonymForResolver { get; }
    private Func<string, string, Task<string>> GetOriginalValueForResolver { get; }
    private FhirClient FhirClient { get; }

    public async Task<string> GetOrCreatePseudonymFor(string value, string domain)
    {
        TotalGPasRequests.WithLabels(nameof(GetOrCreatePseudonymFor)).Inc();

        return await PseudonymCache.GetOrCreateAsync(
            (value, domain),
            async entry =>
            {
                TotalGPasRequestCacheMisses.WithLabels(nameof(GetOrCreatePseudonymFor)).Inc();

                entry
                    .SetSize(1)
                    .SetSlidingExpiration(SlidingExpiration)
                    .SetAbsoluteExpiration(AbsoluteExpiration);

                logger.LogDebug(
                    "Getting or creating pseudonym for {value} in {domain}",
                    value,
                    domain
                );

                return await GetOrCreatePseudonymForResolver(value, domain);
            }
        );
    }

    public async Task<string> GetOriginalValueFor(string pseudonym, string domain)
    {
        TotalGPasRequests.WithLabels(nameof(GetOriginalValueFor)).Inc();

        return await OriginalValueCache.GetOrCreateAsync(
            (pseudonym, domain),
            async entry =>
            {
                TotalGPasRequestCacheMisses.WithLabels(nameof(GetOriginalValueFor)).Inc();

                entry
                    .SetSize(1)
                    .SetSlidingExpiration(SlidingExpiration)
                    .SetAbsoluteExpiration(AbsoluteExpiration);

                logger.LogDebug(
                    "Getting original value for pseudonym {Pseudonym} from {Domain}",
                    pseudonym,
                    domain
                );

                return await GetOriginalValueForResolver(pseudonym, domain);
            }
        );
    }

    private async Task<string> GetOriginalValueForV1(string pseudonym, string domain)
    {
        var query = new Dictionary<string, string>
        {
            ["domain"] = domain,
            ["pseudonym"] = pseudonym
        };

        var response = await Client.GetAsync(
            QueryHelpers.AddQueryString("$de-pseudonymize", query)
        );
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var parameters = FhirParser.Parse<Parameters>(content);

        var original = parameters.GetSingleValue<FhirString>(pseudonym);
        if (original == null)
        {
            logger.LogWarning("Failed to de-pseudonymize. Returning original value.", pseudonym);
            return pseudonym;
        }

        return original.Value;
    }

    private async Task<string> GetOriginalValueForV2(string pseudonym, string domain)
    {
        try
        {
            var responseParameters = await RequestGetOriginalValueForV2(
                pseudonym,
                domain,
                "$de-pseudonymize"
            );

            var pseudonymResultSet = responseParameters.Get("pseudonym-result-set").First();
            var originalPart = pseudonymResultSet.Part.Find(
                component => component.Name == "original"
            );

            return originalPart.Value.ToString();
        }
        catch (Exception exc)
        {
            logger.LogError(exc, "Failed to de-pseudonymize. Returning original value.");
            return pseudonym;
        }
    }

    private async Task<string> GetOriginalValueForV2x(string pseudonym, string domain)
    {
        try
        {
            var responseParameters = await RequestGetOriginalValueForV2(
                pseudonym,
                domain,
                "$dePseudonymize"
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
        catch (Exception exc)
        {
            logger.LogError(exc, "Failed to de-pseudonymize. Returning original value.");
            return pseudonym;
        }
    }

    private async Task<string> GetOrCreatePseudonymForV1(string value, string domain)
    {
        var query = new Dictionary<string, string> { ["domain"] = domain, ["original"] = value };

        // this currently uses a HttpClient instead of the FhirClient to leverage
        // Polly, tracing, and metrics support. Once FhirClient allows for overriding the HttpClient,
        // we can simplify this code a lot: https://github.com/FirelyTeam/firely-net-sdk/issues/1483
        var response = await Client.GetAsync(
            QueryHelpers.AddQueryString("$pseudonymize-allow-create", query)
        );
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var parameters = FhirParser.Parse<Parameters>(content);
        return parameters.GetSingleValue<FhirString>(value).Value;
    }

    private async Task<string> GetOrCreatePseudonymForV2(string value, string domain)
    {
        var responseParameters = await RequestGetOrCreatePseudonymForV2(
            value,
            domain,
            "pseudonymize-allow-create"
        );

        var firstResponseParameter = responseParameters.Parameter.FirstOrDefault();
        var pseudonym = firstResponseParameter?.Part.Find(part => part.Name == "pseudonym");
        if (pseudonym?.Value == null)
        {
            throw new InvalidOperationException("No pseudonym included in gPAS response.");
        }

        return pseudonym.Value.ToString();
    }

    private async Task<string> GetOrCreatePseudonymForV2x(string value, string domain)
    {
        var responseParameters = await RequestGetOrCreatePseudonymForV2(
            value,
            domain,
            "pseudonymizeAllowCreate"
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
        string operation
    )
    {
        var parameters = new Parameters()
            .Add("target", new FhirString(domain))
            .Add("original", new FhirString(value));

        var response = await FhirClient.WholeSystemOperationAsync(operation, parameters);

        return response as Parameters;
    }

    private async Task<Parameters> RequestGetOriginalValueForV2(
        string pseudonym,
        string domain,
        string operation
    )
    {
        var parameters = new Parameters()
            .Add("target", new FhirString(domain))
            .Add("pseudonym", new FhirString(pseudonym));

        var parametersBody = await FhirSerializer.SerializeToStringAsync(parameters);
        using var content = new StringContent(
            parametersBody,
            Encoding.UTF8,
            "application/fhir+json"
        );

        var response = await Client.PostAsync(operation, content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        return FhirParser.Parse<Parameters>(responseContent);
    }
}
