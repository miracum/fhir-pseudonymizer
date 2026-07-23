using System.Security.Cryptography;
using System.Text;
using FhirPseudonymizer.Config;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using AnonymizerConstants = Microsoft.Health.Fhir.Anonymizer.Core.Constants;

namespace FhirPseudonymizer.Projects;

/// <summary>
///     Serves Project Engines from config files in a mounted directory, caching each built pair
///     until the file's content changes.
/// </summary>
public sealed class FileProjectConfigProvider : IProjectConfigProvider, IDisposable
{
    private static readonly string[] ConfigExtensions = [".yaml", ".yml"];

    private readonly string configsDirectory;
    private readonly IAnonymizerEngineFactory engineFactory;
    private readonly CacheConfig cacheConfig;
    private readonly MemoryCache cache;

    public FileProjectConfigProvider(
        string configsDirectory,
        IAnonymizerEngineFactory engineFactory,
        CacheConfig cacheConfig
    )
    {
        this.configsDirectory = configsDirectory;
        this.engineFactory = engineFactory;
        this.cacheConfig = cacheConfig;

        // A size limit of 0 means "unbounded". The config files are the operator's own, so the
        // number of Projects is already bounded by what they mount — the limit is memory hygiene,
        // not a defence against an unauthenticated caller as the old runtime registry needed.
        cache = new MemoryCache(
            new MemoryCacheOptions
            {
                SizeLimit = cacheConfig.SizeLimit > 0 ? cacheConfig.SizeLimit : null,
            }
        );
    }

    /// <summary>
    ///     A built pair of Engines together with the hash of the config text they were built from,
    ///     so a later read can tell whether the file has changed under the same name.
    /// </summary>
    private sealed record CachedProject(string ContentHash, ProjectEngines Engines);

    public bool TryGetEngines(string name, out ProjectEngines engines)
    {
        engines = null;

        if (!TryReadConfigFile(name, out var yamlConfig))
        {
            return false;
        }

        // Keyed on the name, not the hash, so exactly one entry exists per Project: a changed file
        // rebuilds in place rather than leaving the superseded engines to linger until eviction.
        var contentHash = Hash(yamlConfig);
        if (cache.TryGetValue(name, out CachedProject cached) && cached.ContentHash == contentHash)
        {
            engines = cached.Engines;
            return true;
        }

        // Two concurrent first-time reads of the same name may both build; that is harmless, the
        // last write wins and both callers get a usable, equivalent pair.
        engines = engineFactory.Create(ParseConfig(yamlConfig));
        using (var entry = cache.CreateEntry(name))
        {
            entry.ApplyCacheConfig(cacheConfig);
            entry.Value = new CachedProject(contentHash, engines);
        }

        return true;
    }

    /// <summary>
    ///     Hashes the config as it was read, so the same bytes always key the same cached Engines
    ///     and any edit misses the cache.
    /// </summary>
    private static string Hash(string yamlConfig)
    {
        return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(yamlConfig)));
    }

    /// <summary>
    ///     Reads the config file naming this Project, if one exists. Returns <c>false</c> when the
    ///     directory is unset or holds no <c>&lt;name&gt;.yaml</c> / <c>&lt;name&gt;.yml</c> file.
    /// </summary>
    private bool TryReadConfigFile(string name, out string yamlConfig)
    {
        yamlConfig = null;

        if (string.IsNullOrEmpty(configsDirectory))
        {
            return false;
        }

        var fullDirectory = Path.GetFullPath(configsDirectory);
        foreach (var extension in ConfigExtensions)
        {
            // Path.Join, not Path.Combine: Combine discards the mounted directory outright when
            // name is a rooted path, whereas Join always keeps it. Either way the directory check
            // below is what confines the read to the mount.
            var candidate = Path.GetFullPath(Path.Join(fullDirectory, name + extension));

            // The file must sit directly in the mounted directory: a name carrying '..' or a path
            // separator would otherwise resolve elsewhere and read a config the operator never
            // meant to expose. Request-facing names are already charset-validated, but the
            // resolver refuses to depend on that.
            if (Path.GetDirectoryName(candidate) != fullDirectory)
            {
                continue;
            }

            if (File.Exists(candidate))
            {
                yamlConfig = File.ReadAllText(candidate);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Deserializes and validates a Project's Config, reporting every way it can fail as an
    ///     <see cref="InvalidProjectConfigException" /> so a request naming it can answer with a 400.
    /// </summary>
    private static AnonymizerConfigurationManager ParseConfig(string yamlConfig)
    {
        AnonymizerConfiguration config;
        try
        {
            config = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build()
                .Deserialize<AnonymizerConfiguration>(yamlConfig);
        }
        catch (YamlException exc)
        {
            throw new InvalidProjectConfigException(
                $"The project config is not valid YAML: {exc.Message}",
                exc
            );
        }

        if (config is null)
        {
            throw new InvalidProjectConfigException("The project config is empty.");
        }

        // Valid YAML can still carry an empty list item (a bare '-' or an explicit null), which
        // no downstream consumer of the rules guards against.
        if (config.FhirPathRules?.Any(rule => rule is null) == true)
        {
            throw new InvalidProjectConfigException(
                "The project config contains an empty rule element in 'fhirPathRules'."
            );
        }

        // Must run before the manager is built: its constructor fills absent keys with a
        // process-wide default, which would silently key a Project's data with a shared secret.
        RequireKeysForRules(config);

        try
        {
            // A Project's keys must come from its own Config, so the AnonymizationConfig
            // fallback argument is omitted.
            return new AnonymizerConfigurationManager(config);
        }
        catch (AnonymizerConfigurationErrorsException exc)
        {
            throw new InvalidProjectConfigException(
                $"The project config is invalid: {exc.Message}",
                exc
            );
        }
    }

    /// <summary>
    ///     Rejects a Config that uses a keyed method without carrying the key itself. A Project's
    ///     keys come only from its own Config, so a rule missing its key can never do what its
    ///     author intended.
    /// </summary>
    private static void RequireKeysForRules(AnonymizerConfiguration config)
    {
        var methods =
            config
                .FhirPathRules?.Where(rule => rule.ContainsKey(AnonymizerConstants.MethodKey))
                .Select(rule => rule[AnonymizerConstants.MethodKey]?.ToString())
                .ToHashSet(StringComparer.InvariantCultureIgnoreCase)
            ?? [];

        RequireKey(methods, "encrypt", "encryptKey", config.Parameters?.EncryptKey);
        RequireKey(methods, "cryptoHash", "cryptoHashKey", config.Parameters?.CryptoHashKey);
        RequireKey(methods, "dateShift", "dateShiftKey", config.Parameters?.DateShiftKey);
    }

    private static void RequireKey(
        IReadOnlySet<string> methods,
        string method,
        string parameterName,
        string key
    )
    {
        if (methods.Contains(method) && string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidProjectConfigException(
                $"The project config uses the '{method}' method but carries no 'parameters.{parameterName}'. "
                    + "Projects do not inherit the server's keys, so the key must be part of the config."
            );
        }
    }

    public void Dispose()
    {
        cache.Dispose();
    }
}
