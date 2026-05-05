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
        return cache.GetOrCreateAsync(
            ("GetOrCreatePseudonymFor", value, domain, BuildSettingsCacheKey(settings)),
            async entry =>
            {
                TotalPseudonymizationRequestCacheMisses
                    .WithLabels(nameof(GetOrCreatePseudonymFor))
                    .Inc();
                ApplyCacheConfig(entry);
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
        return cache.GetOrCreateAsync(
            ("GetOriginalValueFor", pseudonym, domain, BuildSettingsCacheKey(settings)),
            async entry =>
            {
                TotalPseudonymizationRequestCacheMisses
                    .WithLabels(nameof(GetOriginalValueFor))
                    .Inc();
                ApplyCacheConfig(entry);
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

        return string.Join(
            "&",
            settings.OrderBy(kvp => kvp.Key, StringComparer.Ordinal)
                    .Select(kvp => $"{kvp.Key}={kvp.Value}")
        );
    }

    private void ApplyCacheConfig(ICacheEntry entry)
    {
        entry.SetSize(1);

        if (cacheConfig.SlidingExpirationMinutes > 0)
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(cacheConfig.SlidingExpirationMinutes));
        }

        if (cacheConfig.AbsoluteExpirationMinutes > 0)
        {
            entry.SetAbsoluteExpiration(
                TimeSpan.FromMinutes(cacheConfig.AbsoluteExpirationMinutes)
            );
        }
    }
}
