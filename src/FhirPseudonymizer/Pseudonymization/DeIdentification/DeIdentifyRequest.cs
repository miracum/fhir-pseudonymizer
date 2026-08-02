using FhirParametersGenerator;
using Hl7.Fhir.Model;

namespace FhirPseudonymizer.Pseudonymization.DeIdentification;

/// <summary>
///     Request body for the /v3alpha1/fhir/$de-identify operation. Bundles the same de-identification
///     rules that are otherwise supplied via an anonymization.yaml config file (see e.g.
///     hipaa-anonymization.yaml) together with the FHIR resource they should be applied to, all
///     carried as parts of a single FHIR Parameters resource.
/// </summary>
[GenerateFhirParameters]
public partial class DeIdentifyRequest
{
    public string FhirVersion { get; set; }
    public List<FhirPathRuleParameter> FhirPathRules { get; set; } = new();
    public DeIdentifyParametersConfig Parameters { get; set; }
    public Resource Resource { get; set; }
}

/// <summary>
///     A single fhirPathRules entry, e.g. { path: "Patient.name", method: "redact" }.
/// </summary>
public class FhirPathRuleParameter
{
    public string Path { get; set; }
    public string Method { get; set; }
}

/// <summary>
///     Mirrors Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations.ParameterConfiguration,
///     i.e. the "parameters" section of an anonymization.yaml config file.
/// </summary>
public class DeIdentifyParametersConfig
{
    public string DateShiftKey { get; set; }
    public string DateShiftScope { get; set; }

    // Note: the source generator maps CLR int to valueDecimal, not valueInteger, since its
    // ClrTypeToFhirType table has no dedicated Integer entry. Accepted here for consistency with
    // the rest of this class; DeIdentifyRequestParser reads whichever primitive was actually sent.
    public int? DateShiftFixedOffsetInDays { get; set; }
    public string CryptoHashKey { get; set; }
    public string EncryptKey { get; set; }
    public bool? EnablePartialAgesForRedact { get; set; }
    public bool? EnablePartialDatesForRedact { get; set; }
    public bool? EnablePartialZipCodesForRedact { get; set; }
    public List<string> RestrictedZipCodeTabulationAreas { get; set; } = [];
}
