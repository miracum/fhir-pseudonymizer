using System.Text;
using FhirPseudonymizer.Config;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;

namespace FhirPseudonymizer.Tests.Config;

public class KeyConfigurationTests
{
    private const string LongEncryptKey = TestKeys.EncryptKey + TestKeys.OtherEncryptKey;

    private const string ConfigWithoutKeys = """
        fhirVersion: R4
        fhirPathRules:
          - path: Resource.id
            method: cryptoHash
          - path: Patient.identifier.value
            method: encrypt
        """;

    private static string ConfigWithKey(string keyName, string key) =>
        $"""
            {ConfigWithoutKeys}
            parameters:
              {keyName}: "{key}"
            """;

    [Theory]
    [InlineData("cryptoHashKey", "fhir-pseudonymizer")]
    [InlineData("cryptoHashKey", "secret")]
    [InlineData("cryptoHashKey", "   ")]
    [InlineData("cryptoHashKey", "0123456789012345678901234567890")]
    // valid AES-128 and AES-192 key sizes, but shorter than 32 bytes
    [InlineData("encryptKey", "0123456789012345")]
    [InlineData("encryptKey", "fhir-pseudonymizer000000")]
    public void CreateFromYamlConfigString_WithKeyShorterThan32Bytes_ShouldThrow(
        string keyName,
        string key
    )
    {
        var act = () =>
            AnonymizerConfigurationManager.CreateFromYamlConfigString(ConfigWithKey(keyName, key));

        act.Should().Throw<AnonymizerConfigurationErrorsException>();
    }

    [Theory]
    [InlineData("cryptoHashKey", "01234567890123456789012345678901")]
    // 16 characters, but 32 bytes as UTF-8
    [InlineData("cryptoHashKey", "ääääääääääääääää")]
    [InlineData("cryptoHashKey", TestKeys.CryptoHashKey)]
    // `openssl rand -base64 32`
    [InlineData("cryptoHashKey", "NeuidQPrByE7Fh2SXhxrg/Q1yDyDaZkmQW4Q1RXVrHY=")]
    [InlineData("encryptKey", TestKeys.EncryptKey)]
    public void CreateFromYamlConfigString_WithKeyOfAtLeast32Bytes_ShouldNotThrow(
        string keyName,
        string key
    )
    {
        var act = () =>
            AnonymizerConfigurationManager.CreateFromYamlConfigString(ConfigWithKey(keyName, key));

        act.Should().NotThrow();
    }

    [Fact]
    public void CreateFromYamlConfigString_WithoutKeys_ShouldGenerateADifferentRandomKeyForEach()
    {
        var configManager = AnonymizerConfigurationManager.CreateFromYamlConfigString(
            ConfigWithoutKeys,
            new AnonymizationConfig()
        );

        var encryptKey = configManager.GetEncryptKeyBytes();
        encryptKey.Should().HaveCount(32);
        Encoding
            .UTF8.GetString(encryptKey)
            .Should()
            .NotBe(configManager.GetParameterConfiguration().CryptoHashKey);
    }

    [Theory]
    [InlineData("fhir-pseudonymizer", null, null)]
    [InlineData(null, "fhir-pseudonymizer000000", null)]
    // derived keys are always 32 bytes, but can't be any stronger than their master key
    [InlineData("fhir-pseudonymizer", null, "project-a")]
    [InlineData(null, "fhir-pseudonymizer000000", "project-a")]
    public void CreateFromYamlConfigString_WithTooShortKeyFromAnonymizationConfig_ShouldThrow(
        string cryptoHashKey,
        string encryptKey,
        string keyDerivationContext
    )
    {
        var act = () =>
            AnonymizerConfigurationManager.CreateFromYamlConfigString(
                ConfigWithoutKeys,
                new AnonymizationConfig
                {
                    CryptoHashKey = cryptoHashKey,
                    EncryptKey = encryptKey,
                    KeyDerivationContext = keyDerivationContext,
                }
            );

        act.Should().Throw<AnonymizerConfigurationErrorsException>();
    }

    [Theory]
    // set in the YAML config
    [InlineData(LongEncryptKey, null)]
    // set via Anonymization__EncryptKey
    [InlineData(null, LongEncryptKey)]
    public void CreateFromYamlConfigString_WithStaticEncryptKeyLongerThan32Bytes_ShouldThrow(
        string yamlEncryptKey,
        string appSettingsEncryptKey
    )
    {
        var act = () =>
            AnonymizerConfigurationManager.CreateFromYamlConfigString(
                yamlEncryptKey is null
                    ? ConfigWithoutKeys
                    : ConfigWithKey("encryptKey", yamlEncryptKey),
                new AnonymizationConfig { EncryptKey = appSettingsEncryptKey }
            );

        act.Should().Throw<AnonymizerConfigurationErrorsException>();
    }

    [Theory]
    // set in the YAML config
    [InlineData(LongEncryptKey, null)]
    // set via Anonymization__EncryptKey
    [InlineData(null, LongEncryptKey)]
    public void CreateFromYamlConfigString_WithEncryptMasterKeyLongerThan32BytesAndKeyDerivationContext_ShouldNotThrow(
        string yamlEncryptKey,
        string appSettingsEncryptKey
    )
    {
        // only an encryptKey used as-is has to be exactly 32 bytes, a master key may be longer
        var act = () =>
            AnonymizerConfigurationManager.CreateFromYamlConfigString(
                yamlEncryptKey is null
                    ? ConfigWithoutKeys
                    : ConfigWithKey("encryptKey", yamlEncryptKey),
                new AnonymizationConfig
                {
                    EncryptKey = appSettingsEncryptKey,
                    KeyDerivationContext = "project-a",
                }
            );

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("cryptoHashKey", "a-shared-secret-key")]
    [InlineData("encryptKey", "fhir-pseudonymizer000000")]
    public void CreateFromYamlConfigString_WithTooShortKey_ShouldNotIncludeTheKeyInTheError(
        string keyName,
        string key
    )
    {
        var act = () =>
            AnonymizerConfigurationManager.CreateFromYamlConfigString(ConfigWithKey(keyName, key));

        act.Should()
            .Throw<AnonymizerConfigurationErrorsException>()
            .Which.Message.Should()
            .NotContain(key);
    }
}
