using FhirParametersGenerator;
using Hl7.Fhir.Model;

namespace FhirPseudonymizer.Pseudonymization.Entici;

[GenerateFhirParameters]
public class EnticiPseudonymizationRequest
{
    public Identifier Identifier { get; set; }
    public Code ResourceType { get; set; }
    public FhirString Project { get; set; }
}
