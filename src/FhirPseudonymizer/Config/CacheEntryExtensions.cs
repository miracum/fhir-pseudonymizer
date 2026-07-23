using Microsoft.Extensions.Caching.Memory;

namespace FhirPseudonymizer.Config;

public static class CacheEntryExtensions
{
    /// <summary>
    ///     Sizes and expires a cache entry according to a <see cref="CacheConfig" />: every entry
    ///     counts as 1 towards the cache's size limit, and an expiration left at 0 stays disabled.
    /// </summary>
    public static void ApplyCacheConfig(this ICacheEntry entry, CacheConfig cacheConfig)
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
