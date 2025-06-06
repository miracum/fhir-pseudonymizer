using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations
{
    [DataContract]
    public class AnonymizerConfiguration
    {
        // Static default crypto hash key to provide a same default key for all engine instances
        private static readonly Lazy<string> s_defaultCryptoKey = new Lazy<string>(() =>
            Guid.NewGuid().ToString("N")
        );

        [DataMember(Name = "fhirVersion")]
        public string FhirVersion { get; set; }

        [DataMember(Name = "fhirPathRules")]
        public Dictionary<string, object>[] FhirPathRules { get; set; }

        [DataMember(Name = "parameters")]
        // renamed to "Parameters" due to missing support for DataMember attributes
        // https://github.com/aaubry/YamlDotNet/issues/461
        public ParameterConfiguration Parameters { get; set; }

        public void GenerateDefaultParametersIfNotConfigured()
        {
            // if not configured, a random string will be generated as date shift key, others will keep their default values
            if (Parameters == null)
            {
                Parameters = new ParameterConfiguration
                {
                    DateShiftKey = Guid.NewGuid().ToString("N"),
                    CryptoHashKey = s_defaultCryptoKey.Value,
                    EncryptKey = s_defaultCryptoKey.Value,
                };
                return;
            }

            if (string.IsNullOrEmpty(Parameters.DateShiftKey))
            {
                Parameters.DateShiftKey = Guid.NewGuid().ToString("N");
            }

            if (string.IsNullOrEmpty(Parameters.CryptoHashKey))
            {
                Parameters.CryptoHashKey = s_defaultCryptoKey.Value;
            }

            if (string.IsNullOrEmpty(Parameters.EncryptKey))
            {
                Parameters.EncryptKey = s_defaultCryptoKey.Value;
            }
        }
    }
}
