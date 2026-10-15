using System.Runtime.Serialization;

namespace Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations
{
    [DataContract]
    public class AnonymizerConfiguration
    {
        // Static default keys to provide the same default keys for all engine instances - one
        // each, so that HMAC and AES never share a key. 32 hex characters are exactly the 32 bytes
        // a statically-set encryptKey has to be.
        private static readonly Lazy<string> s_defaultCryptoHashKey = new Lazy<string>(() =>
            Guid.NewGuid().ToString("N")
        );
        private static readonly Lazy<string> s_defaultEncryptKey = new Lazy<string>(() =>
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
                    CryptoHashKey = s_defaultCryptoHashKey.Value,
                    EncryptKey = s_defaultEncryptKey.Value,
                };
                return;
            }

            if (string.IsNullOrEmpty(Parameters.DateShiftKey))
            {
                Parameters.DateShiftKey = Guid.NewGuid().ToString("N");
            }

            if (string.IsNullOrEmpty(Parameters.CryptoHashKey))
            {
                Parameters.CryptoHashKey = s_defaultCryptoHashKey.Value;
            }

            if (string.IsNullOrEmpty(Parameters.EncryptKey))
            {
                Parameters.EncryptKey = s_defaultEncryptKey.Value;
            }
        }
    }
}
