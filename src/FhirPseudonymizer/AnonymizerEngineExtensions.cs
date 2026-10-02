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
                "Anonymization config not set. Specify either a path or an inline config."
            );
        }

        // add the anon config as an additional service to allow mocking it
        services.AddSingleton(_ => anonConfigManager);

        services.AddSingleton<IAnonymizerEngine>(sp =>
        {
            var anonConfig = sp.GetRequiredService<AnonymizerConfigurationManager>();
            var engine = new AnonymizerEngine(anonConfig);

            var psnClient = sp.GetRequiredService<IPseudonymServiceClient>();
            engine.AddProcessor(
                "pseudonymize",
                new PseudonymizationProcessor(psnClient, appConfig.Features)
            );

            return engine;
        });

        services.AddSingleton<IDePseudonymizerEngine>(sp =>
        {
            var anonConfig = sp.GetRequiredService<AnonymizerConfigurationManager>();
            var engine = new DePseudonymizerEngine(anonConfig);

            var psnClient = sp.GetRequiredService<IPseudonymServiceClient>();
            engine.AddProcessor(
                "pseudonymize",
                new DePseudonymizationProcessor(psnClient, appConfig.Features)
            );

            engine.AddProcessor("encrypt", new DecryptProcessor(anonConfig.GetEncryptKeyBytes()));
            return engine;
        });

        return services;
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
