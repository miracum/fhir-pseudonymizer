using System.Collections;
using System.Globalization;
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
        var settingsKey = BuildSettingsCacheKey(settings);
        return cache.GetOrCreateAsync(
            ("GetOrCreatePseudonymFor", value, domain, settingsKey),
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
        var settingsKey = BuildSettingsCacheKey(settings);
        return cache.GetOrCreateAsync(
            ("GetOriginalValueFor", pseudonym, domain, settingsKey),
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

    private static string BuildSettingsCacheKey(IReadOnlyDictionary<string, object> settings)
    {
        if (settings is null || settings.Count == 0)
        {
            return string.Empty;
        }

        var key = string.Join(
            ";",
            settings
                .OrderBy(static pair => pair.Key, StringComparer.Ordinal)
                .Select(pair => $"{pair.Key}={BuildValueCacheKey(pair.Value)}")
        );
        return $"{{{key}}}";
    }

    private static string BuildValueCacheKey(object value)
    {
        if (value is null)
        {
            return "<null>";
        }

        if (value is IReadOnlyDictionary<string, object> stringDictionary)
        {
            return BuildSettingsCacheKey(stringDictionary);
        }

        if (value is IReadOnlyDictionary<object, object> objectDictionary)
        {
            var key = string.Join(
                ";",
                objectDictionary
                    .OrderBy(pair => pair.Key?.ToString(), StringComparer.Ordinal)
                    .Select(pair => $"{pair.Key}={BuildValueCacheKey(pair.Value)}")
            );
            return $"{{{key}}}";
        }

        if (value is IEnumerable sequence && value is not string)
        {
            var key = string.Join(
                ",",
                sequence.Cast<object>().Select(BuildValueCacheKey)
            );
            return $"[{key}]";
        }

        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? "<null>";
    }
}
