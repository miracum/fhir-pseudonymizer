using System.Diagnostics.Metrics;
using System.Text.Json;
using FhirPseudonymizer.Config;
using Microsoft.Extensions.Caching.Memory;

namespace FhirPseudonymizer.Pseudonymization;

public class CachedPseudonymServiceClient(
    IPseudonymServiceClient innerClient,
    IMemoryCache cache,
    CacheConfig cacheConfig
) : IPseudonymServiceClient
{
    // Dotted names are the OpenTelemetry convention; the Prometheus exporter renders them with
    // underscores and a "_total" suffix on export, matching the metric names this client exposed
    // under prometheus-net.
    private static readonly Counter<long> TotalPseudonymizationRequests =
        Program.Meter.CreateCounter<long>(
            "fhirpseudonymizer.pseudonymization.requests",
            description: "Total number of requests against the pseudonymization service cache, "
                + "regardless of whether they were resolved via the cache or forwarded to the underlying service."
        );

    private static readonly Counter<long> TotalPseudonymizationRequestCacheMisses =
        Program.Meter.CreateCounter<long>(
            "fhirpseudonymizer.pseudonymization.requests.cache_misses",
            description: "Total number of requests against the pseudonymization service that could not be resolved via the internal cache."
        );

    public Task<string> GetOrCreatePseudonymFor(
        string value,
        string domain,
        IReadOnlyDictionary<string, object> settings = null,
        CancellationToken cancellationToken = default
    )
    {
        TotalPseudonymizationRequests.Add(
            1,
            new KeyValuePair<string, object>("operation", nameof(GetOrCreatePseudonymFor))
        );

        return cache.GetOrCreateAsync(
            ("GetOrCreatePseudonymFor", value, domain, BuildSettingsCacheKey(settings)),
            async entry =>
            {
                TotalPseudonymizationRequestCacheMisses.Add(
                    1,
                    new KeyValuePair<string, object>("operation", nameof(GetOrCreatePseudonymFor))
                );
                ApplyCacheConfig(entry);
                return await innerClient.GetOrCreatePseudonymFor(
                    value,
                    domain,
                    settings,
                    cancellationToken
                );
            }
        );
    }

    public Task<string> GetOriginalValueFor(
        string pseudonym,
        string domain,
        IReadOnlyDictionary<string, object> settings = null,
        CancellationToken cancellationToken = default
    )
    {
        TotalPseudonymizationRequests.Add(
            1,
            new KeyValuePair<string, object>("operation", nameof(GetOriginalValueFor))
        );

        return cache.GetOrCreateAsync(
            ("GetOriginalValueFor", pseudonym, domain, BuildSettingsCacheKey(settings)),
            async entry =>
            {
                TotalPseudonymizationRequestCacheMisses.Add(
                    1,
                    new KeyValuePair<string, object>("operation", nameof(GetOriginalValueFor))
                );
                ApplyCacheConfig(entry);
                return await innerClient.GetOriginalValueFor(
                    pseudonym,
                    domain,
                    settings,
                    cancellationToken
                );
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
