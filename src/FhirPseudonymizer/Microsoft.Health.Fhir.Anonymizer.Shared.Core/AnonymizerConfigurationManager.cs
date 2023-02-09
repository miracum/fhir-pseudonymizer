using System.IO;
using System.Linq;
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

        public AnonymizerConfigurationManager(AnonymizerConfiguration configuration)
        {
            _validator.Validate(configuration);
            configuration.GenerateDefaultParametersIfNotConfigured();

            _configuration = configuration;

            FhirPathRules = _configuration.FhirPathRules
                .Select(entry => AnonymizationFhirPathRule.CreateAnonymizationFhirPathRule(entry))
                .ToArray();
        }

        public AnonymizationFhirPathRule[] FhirPathRules { get; }

        public static AnonymizerConfigurationManager CreateFromSettingsInJson(string settingsInJson)
        {
            try
            {
                var settings = new JsonLoadSettings
                {
                    DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error
                };
                var token = JToken.Parse(settingsInJson, settings);
                var configuration = token.ToObject<AnonymizerConfiguration>();
                return new AnonymizerConfigurationManager(configuration);
            }
            catch (JsonException innerException)
            {
                throw new JsonException("Failed to parse configuration file", innerException);
            }
        }

        public static AnonymizerConfigurationManager CreateFromConfigurationFile(
            string configFilePath
        )
        {
            try
            {
                var content = File.ReadAllText(configFilePath);

                return CreateFromSettingsInJson(content);
            }
            catch (IOException innerException)
            {
                throw new IOException(
                    $"Failed to read configuration file {configFilePath}",
                    innerException
                );
            }
        }

        public static AnonymizerConfigurationManager CreateFromYamlConfigFile(string configFilePath)
        {
            try
            {
                var content = File.ReadAllText(configFilePath);

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                var config = deserializer.Deserialize<AnonymizerConfiguration>(content);

                return new AnonymizerConfigurationManager(config);
            }
            catch (IOException innerException)
            {
                throw new IOException(
                    $"Failed to read configuration file {configFilePath}",
                    innerException
                );
            }
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
