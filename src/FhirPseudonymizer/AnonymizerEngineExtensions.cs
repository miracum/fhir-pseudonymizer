using FhirPseudonymizer.Config;
using FhirPseudonymizer.Pseudonymization;
using Microsoft.Health.Fhir.Anonymizer.Core;

namespace FhirPseudonymizer;

public static class AnonymizerEngineExtensions
{
    public static IServiceCollection AddAnonymizerEngine(
        this IServiceCollection services,
        AppConfig appConfig
    )
    {
        AnonymizerEngine.InitializeFhirPathExtensionSymbols();

        var configFilePath =
            appConfig.AnonymizationConfig.Path ?? appConfig.AnonymizationEngineConfigPath;

        AnonymizerConfigurationManager anonConfigManager = null;
        if (!string.IsNullOrEmpty(appConfig.AnonymizationConfig.Inline))
        {
            anonConfigManager = AnonymizerConfigurationManager.CreateFromYamlConfigString(
                appConfig.AnonymizationConfig.Inline
            );
        }
        else if (!string.IsNullOrEmpty(configFilePath))
        {
            anonConfigManager = AnonymizerConfigurationManager.CreateFromYamlConfigFile(
                appConfig.AnonymizationEngineConfigPath
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

            engine.AddProcessor(
                "encrypt",
                new DecryptProcessor(anonConfig.GetParameterConfiguration().EncryptKey)
            );
            return engine;
        });

        return services;
    }
}
