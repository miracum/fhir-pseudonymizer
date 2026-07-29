using FhirPseudonymizer.Config;
using FhirPseudonymizer.Projects;
using Microsoft.Health.Fhir.Anonymizer.Core;

namespace FhirPseudonymizer;

public static class AnonymizerEngineExtensions
{
    public static IServiceCollection AddAnonymizerEngine(
        this IServiceCollection services,
        AppConfig appConfig
    )
    {
        // Registers the FHIRPath symbols every Engine's rules are written against, the Projects'
        // as much as the server's own.
        AnonymizerEngine.InitializeFhirPathExtensionSymbols();

        AnonymizerConfigurationManager anonConfigManager;
        if (!string.IsNullOrEmpty(appConfig.AnonymizationEngineConfigInline))
        {
            anonConfigManager = AnonymizerConfigurationManager.CreateFromYamlConfigString(
                appConfig.AnonymizationEngineConfigInline,
                appConfig.Anonymization
            );
        }
        else if (!string.IsNullOrEmpty(appConfig.AnonymizationEngineConfigPath))
        {
            anonConfigManager = AnonymizerConfigurationManager.CreateFromYamlConfigFile(
                appConfig.AnonymizationEngineConfigPath,
                appConfig.Anonymization
            );
        }
        else
        {
            throw new InvalidOperationException(
                "Anonymization config not set. Specify either a path or an inline config."
            );
        }

        // Exposed for mocking, and shared by the singletons below.
        services.AddSingleton(_ => anonConfigManager);

        // The Engines built from the server's own Config, which every request that names no
        // Project is served with. A Project's are built the same way, by the same factory.
        services.AddSingleton(sp =>
            sp.GetRequiredService<IAnonymizerEngineFactory>()
                .Create(sp.GetRequiredService<AnonymizerConfigurationManager>())
        );

        // The Kafka consumer path only ever runs the server's own rules, so it depends on this
        // single engine rather than resolving a Project.
        services.AddSingleton<IAnonymizerEngine>(sp =>
            sp.GetRequiredService<ProjectEngines>().Anonymizer
        );

        return services;
    }
}
