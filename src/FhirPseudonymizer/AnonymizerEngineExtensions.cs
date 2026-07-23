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
        // Registers the FHIRPath symbols every Engine's rules are written against, so it runs
        // even when this server has no Config of its own.
        AnonymizerEngine.InitializeFhirPathExtensionSymbols();

        if (!HasStartupConfig(appConfig))
        {
            services.AddSingleton(_ => ServerEngines.None);
            return services;
        }

        var anonConfigManager = !string.IsNullOrEmpty(appConfig.AnonymizationEngineConfigInline)
            ? AnonymizerConfigurationManager.CreateFromYamlConfigString(
                appConfig.AnonymizationEngineConfigInline,
                appConfig.Anonymization
            )
            : AnonymizerConfigurationManager.CreateFromYamlConfigFile(
                appConfig.AnonymizationEngineConfigPath,
                appConfig.Anonymization
            );

        // Exposed for mocking, and shared by the singletons below.
        services.AddSingleton(_ => anonConfigManager);

        services.AddSingleton(sp =>
            sp.GetRequiredService<IAnonymizerEngineFactory>()
                .Create(sp.GetRequiredService<AnonymizerConfigurationManager>())
        );

        // The Kafka consumer path only ever runs the server's own rules, so it depends on this
        // single engine rather than resolving a Project.
        services.AddSingleton<IAnonymizerEngine>(sp =>
            sp.GetRequiredService<ProjectEngines>().Anonymizer
        );

        services.AddSingleton(sp => new ServerEngines(sp.GetRequiredService<ProjectEngines>()));

        return services;
    }

    /// <summary>
    ///     Whether the server was given a Config of its own. Both settings left blank asks for a
    ///     Projects-only deployment, in which every request selects a mounted Project's Config.
    /// </summary>
    public static bool HasStartupConfig(AppConfig appConfig)
    {
        return !string.IsNullOrEmpty(appConfig.AnonymizationEngineConfigInline)
            || !string.IsNullOrEmpty(appConfig.AnonymizationEngineConfigPath);
    }
}
