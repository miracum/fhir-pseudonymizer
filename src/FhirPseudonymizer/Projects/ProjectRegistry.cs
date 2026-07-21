using FhirPseudonymizer.Config;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using AnonymizerConstants = Microsoft.Health.Fhir.Anonymizer.Core.Constants;

namespace FhirPseudonymizer.Projects;

public class ProjectRegistry : IProjectRegistry, IDisposable
{
    private readonly MemoryCache cache;
    private readonly CacheConfig cacheConfig;
    private readonly IAnonymizerEngineFactory engineFactory;
    private readonly Lock mutationLock = new();

    public ProjectRegistry(IAnonymizerEngineFactory engineFactory, CacheConfig cacheConfig)
    {
        // A zero size limit would reject every entry, so each registration would answer 503
        // promising a retry that cannot succeed.
        if (cacheConfig.SizeLimit == 0)
        {
            throw new InvalidOperationException(
                "ProjectCache:SizeLimit must be greater than 0: the project registry cannot "
                    + "store anything in a zero-sized cache. Raise the limit or leave it at "
                    + "its default."
            );
        }

        this.engineFactory = engineFactory;
        this.cacheConfig = cacheConfig;

        // One entry's worth, which is what a rejected registration needs and all it should cost:
        // every evicted Project is a caller whose next request 404s and who has to register again.
        // The default 5% would both floor to zero evictions below a limit of 20 — leaving a full
        // registry answering 503 while promising a retry would succeed — and evict far more than
        // one above it (6 at the default limit of 128, 50 at 1000).
        cache = new MemoryCache(
            new MemoryCacheOptions
            {
                SizeLimit = cacheConfig.SizeLimit,
                CompactionPercentage = 1.0 / cacheConfig.SizeLimit,
            }
        );
    }

    public ProjectRegistrationOutcome Register(string name, string yamlConfig)
    {
        // Engines are built eagerly so a broken config is reported at registration, not on the
        // first bundle naming this Project.
        var engines = engineFactory.Create(ParseConfig(yamlConfig));

        // The outcome is derived from three cache operations, so they are serialized: otherwise
        // two concurrent registrations of the same new name could both answer 201, and a removal
        // between the store and the re-check would read as a full registry.
        lock (mutationLock)
        {
            var replacesExisting = cache.TryGetValue(name, out _);

            using (var entry = cache.CreateEntry(name))
            {
                entry.ApplyCacheConfig(cacheConfig);
                entry.Value = engines;
            }

            // MemoryCache does not evict to make room: over the size bound it drops the *new*
            // entry and compacts in the background, so the store has to be confirmed.
            if (!cache.TryGetValue(name, out _))
            {
                return ProjectRegistrationOutcome.NotStored;
            }

            return replacesExisting
                ? ProjectRegistrationOutcome.Replaced
                : ProjectRegistrationOutcome.Created;
        }
    }

    /// <summary>
    ///     Deserializes and validates a Project's Config, reporting every way it can fail as an
    ///     <see cref="InvalidProjectConfigException" /> so registration can answer with a 400.
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

    public bool TryGet(string name, out ProjectEngines engines)
    {
        return cache.TryGetValue(name, out engines);
    }

    public void Remove(string name)
    {
        lock (mutationLock)
        {
            cache.Remove(name);
        }
    }

    public void Dispose()
    {
        cache.Dispose();
        GC.SuppressFinalize(this);
    }
}
