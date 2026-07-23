using Microsoft.Health.Fhir.Anonymizer.Core;

namespace FhirPseudonymizer.Projects;

/// <summary>
///     Builds the Engines for one Config. Used both for the startup Config and for every
///     registered Project.
/// </summary>
public interface IAnonymizerEngineFactory
{
    ProjectEngines Create(AnonymizerConfigurationManager configManager);
}
