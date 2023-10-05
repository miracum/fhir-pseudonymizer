using System.IO;
using System.Linq;
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

        public AnonymizerConfigurationManager(AnonymizerConfiguration configuration, AnonymizationConfig anonymizationConfig = null)
        {
            _validator.Validate(configuration);

            if (anonymizationConfig is not null)
            {
                configuration.Parameters ??= new ParameterConfiguration();

                if (string.IsNullOrWhiteSpace(configuration.Parameters.CryptoHashKey))
                {
                    configuration.Parameters.CryptoHashKey = anonymizationConfig.CryptoHashKey;
                }
                if (string.IsNullOrWhiteSpace(configuration.Parameters.EncryptKey))
                {
                    configuration.Parameters.EncryptKey = anonymizationConfig.EncryptKey;
                }
            }

            configuration.GenerateDefaultParametersIfNotConfigured();

            _configuration = configuration;

            FhirPathRules = _configuration.FhirPathRules
                .Select(AnonymizationFhirPathRule.CreateAnonymizationFhirPathRule)
                .ToArray();
        }

        public AnonymizationFhirPathRule[] FhirPathRules { get; }

        public static AnonymizerConfigurationManager CreateFromSettingsInJson(string settingsInJson, AnonymizationConfig anonymizationConfig = null)
        {
            try
            {
                var settings = new JsonLoadSettings
                {
                    DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error
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
            string configFilePath, AnonymizationConfig anonymizationConfig = null
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

        public static AnonymizerConfigurationManager CreateFromYamlConfigFile(string configFilePath, AnonymizationConfig anonymizationConfig = null)
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

        public static AnonymizerConfigurationManager CreateFromYamlConfigString(string yamlConfig, AnonymizationConfig anonymizationConfig = null)
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

        public void SetDateShiftKeyPrefix(string prefix)
        {
            _configuration.Parameters.DateShiftKeyPrefix = prefix;
        }
    }
}
