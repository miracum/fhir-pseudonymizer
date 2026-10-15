using FhirPseudonymizer.Config;
using FhirPseudonymizer.Pseudonymization;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;

namespace FhirPseudonymizer;

public static class AnonymizerEngineExtensions
{
    public static IServiceCollection AddAnonymizerEngine(
        this IServiceCollection services,
        AppConfig appConfig
    )
    {
        AnonymizerEngine.InitializeFhirPathExtensionSymbols();

        var configFilePath = appConfig.AnonymizationEngineConfigPath;

        AnonymizerConfigurationManager anonConfigManager = null;
        if (!string.IsNullOrEmpty(appConfig.AnonymizationEngineConfigInline))
        {
            anonConfigManager = AnonymizerConfigurationManager.CreateFromYamlConfigString(
                appConfig.AnonymizationEngineConfigInline,
                appConfig.Anonymization
            );
        }
        else if (!string.IsNullOrEmpty(configFilePath))
        {
            anonConfigManager = AnonymizerConfigurationManager.CreateFromYamlConfigFile(
                configFilePath,
                appConfig.Anonymization
            );
        }
        else
        {
            throw new InvalidOperationException(
                "Anonymization config not set. Set either AnonymizationEngineConfigPath or AnonymizationEngineConfigInline."
            );
        }

        // add the anon config as an additional service to allow mocking it
        services.AddSingleton(_ => anonConfigManager);

        services.AddSingleton<IAnonymizerEngine>(sp =>
            CreateAnonymizerEngine(
                sp.GetRequiredService<AnonymizerConfigurationManager>(),
                sp.GetRequiredService<IPseudonymServiceClient>(),
                appConfig.Features
            )
        );

        services.AddSingleton<IDePseudonymizerEngine>(sp =>
            CreateDePseudonymizerEngine(
                sp.GetRequiredService<AnonymizerConfigurationManager>(),
                sp.GetRequiredService<IPseudonymServiceClient>(),
                appConfig.Features
            )
        );

        return services;
    }

    /// <summary>
    ///     The engine behind <c>$de-identify</c>: the built-in processors, plus pseudonymization
    ///     through the configured pseudonymization service.
    /// </summary>
    public static AnonymizerEngine CreateAnonymizerEngine(
        AnonymizerConfigurationManager configurationManager,
        IPseudonymServiceClient psnClient,
        FeatureManagement features
    )
    {
        var engine = new AnonymizerEngine(configurationManager);
        engine.AddProcessor("pseudonymize", new PseudonymizationProcessor(psnClient, features));
        return engine;
    }

    /// <summary>
    ///     The engine behind <c>$re-identify</c>: only the reversible methods, each replaced by
    ///     its inverse. Rules using any other method are left alone.
    /// </summary>
    public static DePseudonymizerEngine CreateDePseudonymizerEngine(
        AnonymizerConfigurationManager configurationManager,
        IPseudonymServiceClient psnClient,
        FeatureManagement features
    )
    {
        var engine = new DePseudonymizerEngine(configurationManager);
        engine.AddProcessor("pseudonymize", new DePseudonymizationProcessor(psnClient, features));
        engine.AddProcessor(
            "encrypt",
            new DecryptProcessor(configurationManager.GetEncryptKeyBytes())
        );
        return engine;
    }

    public static void LogErrorIfPseudonymizationServiceIsMissing(
        AnonymizerConfigurationManager anonConfig,
        PseudonymizationServiceType pseudonymizationService,
        ILogger logger
    )
    {
        if (pseudonymizationService != PseudonymizationServiceType.None)
        {
            return;
        }

        // Matched case-insensitively, like the config validator and the engine do.
        var pseudonymizeRulePaths = anonConfig
            .FhirPathRules.Where(rule =>
                string.Equals(
                    rule.Method,
                    nameof(AnonymizerMethod.Pseudonymize),
                    StringComparison.OrdinalIgnoreCase
                )
            )
            .Select(rule => rule.Path)
            .ToList();

        if (pseudonymizeRulePaths.Count == 0)
        {
            return;
        }

        logger.LogError(
            "The anonymization config uses the pseudonymize method for {PseudonymizeRulePaths}, "
                + "but PseudonymizationService is set to None, so resources matching these rules "
                + "will fail to be processed. Set PseudonymizationService to one of gPAS, Vfps, "
                + "entici or Mii.",
            pseudonymizeRulePaths
        );
    }
}
