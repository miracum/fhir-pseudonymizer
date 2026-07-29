using Microsoft.Health.Fhir.Anonymizer.Core;

namespace FhirPseudonymizer.Projects;

/// <summary>
///     The pair of Engines built from one Project's Config: one to anonymize and one to
///     de-pseudonymize. They share the Config, and therefore the Project's crypto keys.
/// </summary>
public sealed record ProjectEngines(
    IAnonymizerEngine Anonymizer,
    IDePseudonymizerEngine DePseudonymizer
);
