using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Models
{
    public static class SecurityLabels
    {
        public static readonly Coding REDACT = new()
        {
            System = "http://terminology.hl7.org/CodeSystem/v3-ObservationValue",
            Code = "REDACTED",
            Display = "redacted",
        };

        public static readonly Coding ABSTRED = new()
        {
            System = "http://terminology.hl7.org/CodeSystem/v3-ObservationValue",
            Code = "ABSTRED",
            Display = "abstracted",
        };

        public static readonly Coding CRYTOHASH = new()
        {
            System = "http://terminology.hl7.org/CodeSystem/v3-ObservationValue",
            Code = "CRYTOHASH",
            Display = "cryptographic hash function",
        };

        public static readonly Coding ENCRYPT = new()
        {
            System = "http://terminology.hl7.org/CodeSystem/v3-ObservationValue",
            Code = "MASKED",
            Display = "masked",
        };

        public static readonly Coding PERTURBED = new()
        {
            Code = "PERTURBED",
            Display = "exact value is replaced with another exact value",
        };

        public static readonly Coding SUBSTITUTED = new()
        {
            Code = "SUBSTITUTED",
            Display = "exact value is replaced with a predefined value",
        };

        public static readonly Coding GENERALIZED = new()
        {
            Code = "GENERALIZED",
            Display = "exact value is replaced with a general value",
        };

        public static readonly Coding PSEUDED = new()
        {
            System = "http://terminology.hl7.org/CodeSystem/v3-ObservationValue",
            Code = "PSEUDED",
            Display = "pseudonymized",
        };
    }
}
