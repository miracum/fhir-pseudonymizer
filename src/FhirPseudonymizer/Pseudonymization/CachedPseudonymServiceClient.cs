using FhirPseudonymizer.Config;
using Microsoft.Extensions.Caching.Memory;

namespace FhirPseudonymizer.Pseudonymization;

public class CachedPseudonymServiceClient(
    IPseudonymServiceClient innerClient,
    IMemoryCache cache,
    CacheConfig cacheConfig
) : IPseudonymServiceClient
{
    public Task<string> GetOrCreatePseudonymFor(
        string value,
        string domain,
        IReadOnlyDictionary<string, object> settings = null
    )
    {
        return cache.GetOrCreateAsync(
            ("GetOrCreatePseudonymFor", value, domain),
            async entry =>
            {
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
            ("GetOriginalValueFor", pseudonym, domain),
            async entry =>
            {
                ApplyCacheConfig(entry);
                return await innerClient.GetOriginalValueFor(pseudonym, domain, settings);
            }
        );
    }

    private void ApplyCacheConfig(ICacheEntry entry)
    {
        entry.SetSize(1);

        if (cacheConfig.SlidingExpirationMinutes > 0)
        {
            entry.SetSlidingExpiration(
                TimeSpan.FromMinutes(cacheConfig.SlidingExpirationMinutes)
            );
        }

        if (cacheConfig.AbsoluteExpirationMinutes > 0)
        {
            entry.SetAbsoluteExpiration(
                TimeSpan.FromMinutes(cacheConfig.AbsoluteExpirationMinutes)
            );
        }
    }
}
