using System.Text;
using Hl7.FhirPath;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors.Settings;

namespace Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations
{
    public class AnonymizerConfigurationValidator
    {
        // The HMAC-SHA256 output length RFC 2104 discourages shorter keys than, and the AES-256
        // key size.
        public const int MinimumKeyLengthInBytes = 32;

        // AES-256 - AES would also take 16 or 24 bytes, but those fall below the minimum above.
        public const int StaticEncryptKeyLengthInBytes = 32;

        private readonly ILogger _logger =
            AnonymizerLogging.CreateLogger<AnonymizerConfigurationValidator>();

        public void Validate(AnonymizerConfiguration config)
        {
            if (string.IsNullOrEmpty(config.FhirVersion))
            {
                _logger.LogWarning("Version is not specified in configuration file.");
            }
            else if (
                !string.Equals(
                    Constants.SupportedVersion,
                    config.FhirVersion,
                    StringComparison.InvariantCultureIgnoreCase
                )
            )
            {
                throw new AnonymizerConfigurationErrorsException(
                    $"Configuration of fhirVersion {config.FhirVersion} is not supported. Expected fhirVersion: {Constants.SupportedVersion}"
                );
            }

            if (config.FhirPathRules == null)
            {
                throw new AnonymizerConfigurationErrorsException(
                    "The configuration is invalid, please specify any fhirPathRules"
                );
            }

            var compiler = new FhirPathCompiler();
            var supportedMethods = Enum.GetNames(typeof(AnonymizerMethod))
                .ToHashSet(StringComparer.InvariantCultureIgnoreCase);
            foreach (var rule in config.FhirPathRules)
            {
                if (!rule.ContainsKey(Constants.PathKey) || !rule.ContainsKey(Constants.MethodKey))
                {
                    throw new AnonymizerConfigurationErrorsException(
                        "Missing path or method in Fhir path rule config."
                    );
                }

                // Grammar check on FHIR path
                try
                {
                    compiler.Compile(rule[Constants.PathKey].ToString());
                }
                catch (Exception ex)
                {
                    throw new AnonymizerConfigurationErrorsException(
                        $"Invalid FHIR path {rule[Constants.PathKey]}",
                        ex
                    );
                }

                // Method validate
                var method = rule[Constants.MethodKey].ToString();
                if (!supportedMethods.Contains(method))
                {
                    throw new AnonymizerConfigurationErrorsException(
                        $"Anonymization method {method} not supported."
                    );
                }

                // Should provide replacement value for substitute rule
                if (
                    string.Equals(
                        method,
                        AnonymizerMethod.Substitute.ToString(),
                        StringComparison.InvariantCultureIgnoreCase
                    )
                )
                {
                    SubstituteSetting.ValidateRuleSettings(rule);
                }

                if (
                    string.Equals(
                        method,
                        AnonymizerMethod.Perturb.ToString(),
                        StringComparison.InvariantCultureIgnoreCase
                    )
                )
                {
                    PerturbSetting.ValidateRuleSettings(rule);
                }

                if (
                    string.Equals(
                        method,
                        AnonymizerMethod.Generalize.ToString(),
                        StringComparison.InvariantCultureIgnoreCase
                    )
                )
                {
                    GeneralizeSetting.ValidateRuleSettings(rule);
                }
            }
        }

        /// <summary>
        ///     Rejects an encryptKey that isn't exactly <see cref="StaticEncryptKeyLengthInBytes" />
        ///     (as UTF-8). Only meant for the fully resolved key, and only if it's used as the AES
        ///     key as-is - not if it's the master key a key derivation context derives it from.
        /// </summary>
        public void ValidateStaticEncryptKeySize(string encryptKey)
        {
            if (string.IsNullOrEmpty(encryptKey))
            {
                return;
            }

            var keyLengthInBytes = Encoding.UTF8.GetByteCount(encryptKey);
            if (keyLengthInBytes != StaticEncryptKeyLengthInBytes)
            {
                throw new AnonymizerConfigurationErrorsException(
                    $"The configured encryptKey is {keyLengthInBytes} bytes long, but is used as the AES-256 key as-is, "
                        + $"so it must be exactly {StaticEncryptKeyLengthInBytes} bytes. Use a randomly generated key "
                        + "instead, e.g. from `openssl rand -base64 24`, or set a key derivation context to derive "
                        + "the AES key from it."
                );
            }
        }

        /// <summary>
        ///     Rejects a configured cryptoHashKey or encryptKey shorter than
        ///     <see cref="MinimumKeyLengthInBytes" /> (as UTF-8). Meant for the fully resolved
        ///     keys (wherever they were set), and before they're used as HKDF master keys -
        ///     deriving from a short master key doesn't make up for its length.
        /// </summary>
        public void ValidateKeyLengths(ParameterConfiguration parameters)
        {
            ValidateKeyLength(parameters?.CryptoHashKey, "cryptoHashKey", "openssl rand -hex 32");
            // 32 characters, the exact length a statically-set encryptKey has to be.
            ValidateKeyLength(parameters?.EncryptKey, "encryptKey", "openssl rand -base64 24");
        }

        private static void ValidateKeyLength(string key, string keyName, string generateCommand)
        {
            // An unset key is replaced by a random one instead, see
            // AnonymizerConfiguration.GenerateDefaultParametersIfNotConfigured.
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var keyLengthInBytes = Encoding.UTF8.GetByteCount(key);
            if (keyLengthInBytes < MinimumKeyLengthInBytes)
            {
                // Never include the key itself - this message ends up in logs and, for configs
                // sent along with a request, in the response.
                throw new AnonymizerConfigurationErrorsException(
                    $"The configured {keyName} is only {keyLengthInBytes} bytes long, "
                        + $"but must be at least {MinimumKeyLengthInBytes} bytes. "
                        + $"Use a randomly generated key instead, e.g. from `{generateCommand}`."
                );
            }
        }
    }
}
