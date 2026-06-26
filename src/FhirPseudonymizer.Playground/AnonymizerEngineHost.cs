using Microsoft.Health.Fhir.Anonymizer.Core;

namespace FhirPseudonymizer.Playground;

// Builds an AnonymizerEngine from a YAML rules string, fully in-memory/in-browser -
// mirrors AnonymizerEngineExtensions.AddAnonymizerEngine in the main server project,
// minus the dependency injection and the real pseudonymization service client.
public static class AnonymizerEngineHost
{
    private static bool symbolsInitialized;

    public static AnonymizerEngine CreateFromYaml(string yamlConfig)
    {
        if (!symbolsInitialized)
        {
            AnonymizerEngine.InitializeFhirPathExtensionSymbols();
            symbolsInitialized = true;
        }

        var configManager = AnonymizerConfigurationManager.CreateFromYamlConfigString(yamlConfig);
        var engine = new AnonymizerEngine(configManager);
        engine.AddProcessor("pseudonymize", new MockPseudonymizationProcessor());

        return engine;
    }
}
