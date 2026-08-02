using System.Text;
using FhirPseudonymizer.Config;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Microsoft.Health.Fhir.Anonymizer.Core
{
    public sealed class AnonymizerConfigurationManager
    {
        private readonly AnonymizerConfiguration _configuration;
        private readonly AnonymizerConfigurationValidator _validator =
            new AnonymizerConfigurationValidator();
        private readonly byte[] _derivedEncryptKey;

        public AnonymizerConfigurationManager(
            AnonymizerConfiguration configuration,
            AnonymizationConfig anonymizationConfig = null
        )
        {
            _validator.Validate(configuration);

            if (anonymizationConfig is not null)
            {
                configuration.Parameters ??= new ParameterConfiguration();

                // Static keys win as-is regardless of source (YAML `parameters:` or
                // Anonymization__* env/config) - only once resolved does KeyDerivationContext
                // decide whether that resolved value is used directly or as an HKDF master key.
                if (string.IsNullOrWhiteSpace(configuration.Parameters.CryptoHashKey))
                {
                    configuration.Parameters.CryptoHashKey = anonymizationConfig.CryptoHashKey;
                }
                if (string.IsNullOrWhiteSpace(configuration.Parameters.EncryptKey))
                {
                    configuration.Parameters.EncryptKey = anonymizationConfig.EncryptKey;
                }

                var keyDerivationContext = anonymizationConfig.KeyDerivationContext;
                if (!string.IsNullOrWhiteSpace(keyDerivationContext))
                {
                    // Each key derives from its own resolved value as its own master key - not
                    // from one another - so CryptoHashKey and EncryptKey stay independent
                    // secrets even when both are being stretched via the same context.
                    if (!string.IsNullOrWhiteSpace(configuration.Parameters.CryptoHashKey))
                    {
                        configuration.Parameters.CryptoHashKey = KeyDerivation.DeriveCryptoHashKey(
                            configuration.Parameters.CryptoHashKey,
                            keyDerivationContext
                        );
                    }

                    if (!string.IsNullOrWhiteSpace(configuration.Parameters.EncryptKey))
                    {
                        _derivedEncryptKey = KeyDerivation.DeriveEncryptKey(
                            configuration.Parameters.EncryptKey,
                            keyDerivationContext
                        );
                    }
                }
            }

            configuration.GenerateDefaultParametersIfNotConfigured();

            _configuration = configuration;

            FhirPathRules = _configuration
                .FhirPathRules.Select(AnonymizationFhirPathRule.CreateAnonymizationFhirPathRule)
                .ToArray();
        }

        public AnonymizationFhirPathRule[] FhirPathRules { get; }

        public static AnonymizerConfigurationManager CreateFromSettingsInJson(
            string settingsInJson,
            AnonymizationConfig anonymizationConfig = null
        )
        {
            try
            {
                var settings = new JsonLoadSettings
                {
                    DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error,
                };
                var token = JToken.Parse(settingsInJson, settings);
                var configuration = token.ToObject<AnonymizerConfiguration>();
                return new AnonymizerConfigurationManager(configuration, anonymizationConfig);
            }
            catch (JsonException innerException)
            {
                throw new JsonException("Failed to parse configuration file", innerException);
            }
        }

        public static AnonymizerConfigurationManager CreateFromConfigurationFile(
            string configFilePath,
            AnonymizationConfig anonymizationConfig = null
        )
        {
            try
            {
                var content = File.ReadAllText(configFilePath);

                return CreateFromSettingsInJson(content, anonymizationConfig);
            }
            catch (IOException innerException)
            {
                throw new IOException(
                    $"Failed to read configuration file {configFilePath}",
                    innerException
                );
            }
        }

        public static AnonymizerConfigurationManager CreateFromYamlConfigFile(
            string configFilePath,
            AnonymizationConfig anonymizationConfig = null
        )
        {
            try
            {
                var content = File.ReadAllText(configFilePath);
                return CreateFromYamlConfigString(content, anonymizationConfig);
            }
            catch (IOException innerException)
            {
                throw new IOException(
                    $"Failed to read configuration file {configFilePath}",
                    innerException
                );
            }
        }

        public static AnonymizerConfigurationManager CreateFromYamlConfigString(
            string yamlConfig,
            AnonymizationConfig anonymizationConfig = null
        )
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var config = deserializer.Deserialize<AnonymizerConfiguration>(yamlConfig);

            return new AnonymizerConfigurationManager(config, anonymizationConfig);
        }

        public ParameterConfiguration GetParameterConfiguration()
        {
            return _configuration.Parameters;
        }

        /// <summary>
        ///     The AES key for the encrypt/decrypt methods, as raw bytes. Returns the derived
        ///     key (from KeyDerivationContext) if one was computed, otherwise falls back to the
        ///     UTF-8 bytes of the static <see cref="ParameterConfiguration.EncryptKey" /> string -
        ///     matching how the vendored EncryptProcessor/DecryptProcessor convert it today.
        /// </summary>
        public byte[] GetEncryptKeyBytes()
        {
            return _derivedEncryptKey
                ?? Encoding.UTF8.GetBytes(_configuration.Parameters.EncryptKey);
        }

        public void SetDateShiftKeyPrefix(string prefix)
        {
            _configuration.Parameters.DateShiftKeyPrefix = prefix;
        }
    }
}
