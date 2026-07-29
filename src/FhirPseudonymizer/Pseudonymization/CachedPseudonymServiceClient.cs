using System.Text.Json;
using FhirPseudonymizer.Config;
using Microsoft.Extensions.Caching.Memory;
using Prometheus;

namespace FhirPseudonymizer.Pseudonymization;

public class CachedPseudonymServiceClient(
    IPseudonymServiceClient innerClient,
    IMemoryCache cache,
    CacheConfig cacheConfig
) : IPseudonymServiceClient
{
    private static readonly Counter TotalPseudonymizationRequests = Metrics.CreateCounter(
        "fhirpseudonymizer_pseudonymization_requests_total",
        "Total number of requests against the pseudonymization service cache, "
            + "regardless of whether they were resolved via the cache or forwarded to the underlying service.",
        new CounterConfiguration() { LabelNames = ["operation"] }
    );

    private static readonly Counter TotalPseudonymizationRequestCacheMisses = Metrics.CreateCounter(
        "fhirpseudonymizer_pseudonymization_requests_cache_misses_total",
        "Total number of requests against the pseudonymization service that could not be resolved via the internal cache.",
        new CounterConfiguration() { LabelNames = ["operation"] }
    );

    public Task<string> GetOrCreatePseudonymFor(
        string value,
        string domain,
        IReadOnlyDictionary<string, object> settings = null
    )
    {
        TotalPseudonymizationRequests.WithLabels(nameof(GetOrCreatePseudonymFor)).Inc();

        return cache.GetOrCreateAsync(
            ("GetOrCreatePseudonymFor", value, domain, BuildSettingsCacheKey(settings)),
            async entry =>
            {
                TotalPseudonymizationRequestCacheMisses
                    .WithLabels(nameof(GetOrCreatePseudonymFor))
                    .Inc();
                entry.ApplyCacheConfig(cacheConfig);
                return await innerClient.GetOrCreatePseudonymFor(value, domain, settings);
            }
        );
    }

    public Task<string> GetOriginalValueFor(
        string pseudonym,
        string domain,
        IReadOnlyDictionary<string, object> settings = null
    )
    {
        TotalPseudonymizationRequests.WithLabels(nameof(GetOriginalValueFor)).Inc();

        return cache.GetOrCreateAsync(
            ("GetOriginalValueFor", pseudonym, domain, BuildSettingsCacheKey(settings)),
            async entry =>
            {
                TotalPseudonymizationRequestCacheMisses
                    .WithLabels(nameof(GetOriginalValueFor))
                    .Inc();
                entry.ApplyCacheConfig(cacheConfig);
                return await innerClient.GetOriginalValueFor(pseudonym, domain, settings);
            }
        );
    }

    private static string BuildSettingsCacheKey(IReadOnlyDictionary<string, object> settings)
    {
        if (settings == null || settings.Count == 0)
        {
            return string.Empty;
        }

        return JsonSerializer.Serialize(settings);
    }
}
