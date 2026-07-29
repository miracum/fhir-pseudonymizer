using FhirPseudonymizer.Config;
using FhirPseudonymizer.Pseudonymization;
using Microsoft.Health.Fhir.Anonymizer.Core;

namespace FhirPseudonymizer.Projects;

public class AnonymizerEngineFactory(
    IPseudonymServiceClient pseudonymServiceClient,
    FeatureManagement features
) : IAnonymizerEngineFactory
{
    public ProjectEngines Create(AnonymizerConfigurationManager configManager)
    {
        var anonymizer = new AnonymizerEngine(configManager);
        anonymizer.AddProcessor(
            "pseudonymize",
            new PseudonymizationProcessor(pseudonymServiceClient, features)
        );

        var dePseudonymizer = new DePseudonymizerEngine(configManager);
        dePseudonymizer.AddProcessor(
            "pseudonymize",
            new DePseudonymizationProcessor(pseudonymServiceClient, features)
        );

        // The encrypt key comes from the Config's own parameters, which is what lets each
        // Project decrypt with its own key.
        dePseudonymizer.AddProcessor(
            "encrypt",
            new DecryptProcessor(configManager.GetParameterConfiguration().EncryptKey)
        );

        return new ProjectEngines(anonymizer, dePseudonymizer);
    }
}
