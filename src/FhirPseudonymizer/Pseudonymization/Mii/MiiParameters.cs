using FhirParametersGenerator;
using Hl7.Fhir.Model;

namespace FhirPseudonymizer.Pseudonymization.Mii;

/// <summary>
/// The input of the $pseudonymize operation of the MII Pseudonymization
/// Implementation Guide 2026.1.0.
/// </summary>
[GenerateFhirParameters]
public partial class MiiPseudonymizeRequest
{
    public Identifier Context { get; set; }
    public Identifier Original { get; set; }
}

/// <summary>
/// The output of the $pseudonymize operation of the MII Pseudonymization
/// Implementation Guide 2026.1.0.
/// </summary>
[GenerateFhirParameters]
public partial class MiiPseudonymizeResponse
{
    public Identifier Context { get; set; }
    public Identifier Original { get; set; }
    public Identifier Pseudonym { get; set; }
}

/// <summary>
/// The input of the $de-pseudonymize operation of the MII Pseudonymization
/// Implementation Guide 2026.1.0.
/// </summary>
[GenerateFhirParameters]
public partial class MiiDePseudonymizeRequest
{
    public Identifier Context { get; set; }
    public Identifier Pseudonym { get; set; }
}

/// <summary>
/// The output of the $de-pseudonymize operation of the MII Pseudonymization
/// Implementation Guide 2026.1.0. The cardinality of "original" is 1..*.
/// </summary>
[GenerateFhirParameters]
public partial class MiiDePseudonymizeResponse
{
    public List<MiiOriginal> Original { get; set; } = [];
}

/// <summary>
/// One "original" output of the $de-pseudonymize operation. The generator maps the
/// properties to the parts of the parameter.
/// </summary>
public class MiiOriginal
{
    public Identifier Context { get; set; }
    public Identifier Value { get; set; }
    public Identifier Pseudonym { get; set; }
}
