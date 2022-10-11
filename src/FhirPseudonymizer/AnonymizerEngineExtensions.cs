using FhirPseudonymizer;
using FhirPseudonymizer.Config;
using FhirPseudonymizer.Pseudonymization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Health.Fhir.Anonymizer.Core;

namespace FhirPseudonymizer;

public static class AnonymizerEngineExtensions
{
    public static IServiceCollection AddAnonymizerEngine(this IServiceCollection services, AppConfig appConfig)
    {
        AnonymizerEngine.InitializeFhirPathExtensionSymbols();

        var anonConfigManager = AnonymizerConfigurationManager.CreateFromYamlConfigFile(appConfig.AnonymizationEngineConfigPath);

        // add the anon config as an additional service to allow mocking it
        services.AddSingleton(_ => anonConfigManager);

        services.AddSingleton<IAnonymizerEngine>(sp =>
        {
            var anonConfig = sp.GetRequiredService<AnonymizerConfigurationManager>();
            var engine = new AnonymizerEngine(anonConfig);

            var psnClient = sp.GetRequiredService<IPseudonymServiceClient>();
            engine.AddProcessor("pseudonymize", new PseudonymizationProcessor(psnClient));

            return engine;
        });

        services.AddSingleton<IDePseudonymizerEngine>(sp =>
        {
            var anonConfig = sp.GetRequiredService<AnonymizerConfigurationManager>();
            var engine = new DePseudonymizerEngine(anonConfig);

            var psnClient = sp.GetRequiredService<IPseudonymServiceClient>();
            engine.AddProcessor("pseudonymize", new DePseudonymizationProcessor(psnClient));

            engine.AddProcessor("encrypt", new DecryptProcessor(anonConfig.GetParameterConfiguration().EncryptKey));
            return engine;
        });

        return services;
    }
}
