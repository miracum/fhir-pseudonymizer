using System.Net;
using System.Net.Http.Headers;
using System.Text;
using FhirPseudonymizer.Config;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;

namespace FhirPseudonymizer.Tests;

public class IntegrationTests(CustomWebApplicationFactory<Startup> factory)
    : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly HttpClient client = factory.CreateClient();

    private readonly string fhirBundleJson =
        @"
        {
          ""resourceType"": ""Bundle"",
          ""type"": ""batch"",
          ""id"": ""test"",
          ""entry"": [
            {
              ""request"": {
                ""method"": ""PUT"",
                ""url"": ""Patient/example0""
              },
              ""resource"": {
                ""resourceType"": ""Patient"",
                ""id"": ""example0"",
                ""gender"": ""female"",
                ""birthDate"": ""1985-10-14""
              }
            }
          ]
        }";

    [Fact]
    public async Task GetMetadata_ReturnsSuccessAndFhirJsonContentType()
    {
        var response = await client.GetAsync(
            "/fhir/metadata",
            TestContext.Current.CancellationToken
        );

        response.EnsureSuccessStatusCode();
        response
            .Content.Headers.ContentType.ToString()
            .Should()
            .Be("application/fhir+json; charset=utf-8");
    }

    [Theory]
    [InlineData("/ready")]
    [InlineData("/live")]
    public async Task ReadyAndLiveChecks_ReturnSuccess(string url)
    {
        var response = await client.GetAsync(url, TestContext.Current.CancellationToken);

        Action act = () => response.EnsureSuccessStatusCode();

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("/fhir/$de-identify")]
    [InlineData("/fhir/$de-pseudonymize")]
    public async Task PostToFhirOperation_WithInvalidContent_ShouldReturnBadRequest(string url)
    {
        var content = new StringContent("asd");
        content.Headers.Add("x-api-key", "dev");
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");

        var response = await client.PostAsync(url, content, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostV3AlphaDeIdentify_WithInvalidContent_ShouldReturnBadRequest()
    {
        using var content = new StringContent("asd");
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");

        var response = await client.PostAsync(
            "/v3alpha1/fhir/$de-identify",
            content,
            TestContext.Current.CancellationToken
        );

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostV3AlphaDeIdentify_WithParametersButNoResource_ShouldReturnBadRequest()
    {
        var parameters = new Parameters().Add(
            "config",
            new Attachment
            {
                ContentType = "application/yaml",
                Data = Encoding.UTF8.GetBytes("fhirVersion: R4\nfhirPathRules: []\n"),
            }
        );

        using var content = new StringContent(parameters.ToJson());
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");

        var response = await client.PostAsync(
            "/v3alpha1/fhir/$de-identify",
            content,
            TestContext.Current.CancellationToken
        );

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostV3AlphaDeIdentify_WithResourceButNoConfig_ShouldReturnBadRequest()
    {
        var parameters = new Parameters().Add("resource", new Patient { Id = "example" });

        var content = new StringContent(parameters.ToJson());
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");

        var response = await client.PostAsync(
            "/v3alpha1/fhir/$de-identify",
            content,
            TestContext.Current.CancellationToken
        );

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostV3AlphaDeIdentify_WithInlineConfigAndResource_ShouldReturnDeIdentifiedResource()
    {
        var parameters = new Parameters()
            .Add(
                "config",
                new Attachment
                {
                    ContentType = "application/yaml",
                    Data = Encoding.UTF8.GetBytes(
                        "fhirVersion: R4\nfhirPathRules:\n  - path: Patient.name\n    method: redact\n"
                    ),
                }
            )
            .Add(
                "resource",
                new Patient
                {
                    Id = "example",
                    Name = [new HumanName { Family = "Doe", Given = ["John"] }],
                }
            );

        var content = new StringContent(parameters.ToJson());
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");

        var response = await client.PostAsync(
            "/v3alpha1/fhir/$de-identify",
            content,
            TestContext.Current.CancellationToken
        );

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );
        var deIdentified = new FhirJsonParser().Parse<Patient>(responseContent);

        deIdentified.Name.Should().BeEmpty();
    }

    [Fact]
    public async Task PostV3AlphaDeIdentify_WithBundleContainingMultipleResources_ShouldDeIdentifyAllEntries()
    {
        var bundleJson = """
            {
              "resourceType": "Bundle",
              "type": "collection",
              "entry": [
                {
                  "resource": {
                    "resourceType": "Patient",
                    "id": "patient-1",
                    "name": [{ "family": "Doe", "given": ["John"] }]
                  }
                },
                {
                  "resource": {
                    "resourceType": "Patient",
                    "id": "patient-2",
                    "name": [{ "family": "Smith", "given": ["Jane"] }]
                  }
                },
                {
                  "resource": {
                    "resourceType": "Observation",
                    "id": "observation-1",
                    "status": "final",
                    "code": { "text": "Body Weight" },
                    "subject": { "reference": "Patient/patient-1" }
                  }
                }
              ]
            }
            """;

        var bundle = await new FhirJsonParser().ParseAsync<Bundle>(bundleJson);

        var parameters = new Parameters()
            .Add(
                "config",
                new Attachment
                {
                    ContentType = "application/yaml",
                    Data = Encoding.UTF8.GetBytes(
                        "fhirVersion: R4\nfhirPathRules:\n  - path: Patient.name\n    method: redact\n"
                    ),
                }
            )
            .Add("resource", bundle);

        var content = new StringContent(parameters.ToJson());
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");

        var response = await client.PostAsync(
            "/v3alpha1/fhir/$de-identify",
            content,
            TestContext.Current.CancellationToken
        );

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );
        var deIdentified = new FhirJsonParser().Parse<Bundle>(responseContent);

        deIdentified.Entry.Should().HaveCount(3);
        deIdentified
            .Entry.Select(e => e.Resource)
            .OfType<Patient>()
            .Should()
            .HaveCount(2)
            .And.AllSatisfy(patient => patient.Name.Should().BeEmpty());
        deIdentified
            .Entry.Select(e => e.Resource)
            .OfType<Observation>()
            .Should()
            .ContainSingle()
            .Which.Subject.Reference.Should()
            .Be("Patient/patient-1");
    }

    [Fact]
    public async Task PostV3AlphaDeIdentify_WithKeyDerivationContextInRequestConfig_ShouldUseDerivedCryptoHashKeyInsteadOfStaticKey()
    {
        const string staticCryptoHashKey = "static-master-key";
        const string keyDerivationContext = "project-a";
        const string patientId = "example";

        var factory = new CustomWebApplicationFactory<Startup>
        {
            CustomInMemorySettings = new Dictionary<string, string>
            {
                ["Anonymization:CryptoHashKey"] = staticCryptoHashKey,
                ["EnableMetrics"] = "false",
            },
        };

        var client = factory.CreateClient();

        var inlineConfig = $"""
            fhirVersion: R4
            fhirPathRules:
              - path: Resource.id
                method: cryptoHash
            parameters:
              keyDerivationContext: {keyDerivationContext}
            """;

        var parameters = new Parameters()
            .Add(
                "config",
                new Attachment
                {
                    ContentType = "application/yaml",
                    Data = Encoding.UTF8.GetBytes(inlineConfig),
                }
            )
            .Add("resource", new Patient { Id = patientId });

        var content = new StringContent(parameters.ToJson());
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");

        var response = await client.PostAsync(
            "/v3alpha1/fhir/$de-identify",
            content,
            TestContext.Current.CancellationToken
        );

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );
        var deIdentified = new FhirJsonParser().Parse<Patient>(responseContent);

        var derivedKey = KeyDerivation.DeriveCryptoHashKey(
            staticCryptoHashKey,
            keyDerivationContext
        );
        var expectedHashWithDerivedKey = CryptoHashUtility.ComputeHmacSHA256Hash(
            patientId,
            derivedKey
        );
        var hashWithStaticKeyDirectly = CryptoHashUtility.ComputeHmacSHA256Hash(
            patientId,
            staticCryptoHashKey
        );

        expectedHashWithDerivedKey.Should().NotBe(hashWithStaticKeyDirectly);

        deIdentified.Id.Should().Be(expectedHashWithDerivedKey);
        deIdentified.Id.Should().NotBe(hashWithStaticKeyDirectly);
    }

    [Fact]
    public async Task PostDeIdentify_WithoutApiKeyHeader_ShouldBeAllowed()
    {
        var patient =
            @"{
                ""resourceType"": ""Patient"",
                ""id"": ""glossy""
            }";

        var content = new StringContent(patient);
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");
        var response = await client.PostAsync(
            "/fhir/$de-identify",
            content,
            TestContext.Current.CancellationToken
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostDePseudonymize_WithoutApiKeyHeader_ShouldReturnUnauthorized()
    {
        var content = new StringContent("");
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");
        var response = await client.PostAsync(
            "/fhir/$de-pseudonymize",
            content,
            TestContext.Current.CancellationToken
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostDePseudonymize_WithWrongApiKey_ShouldReturnUnauthorized()
    {
        var content = new StringContent("");
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");
        content.Headers.Add("x-api-key", "wrong-key");
        var response = await client.PostAsync(
            "/fhir/$de-pseudonymize",
            content,
            TestContext.Current.CancellationToken
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostDeIdentify_WithDefaultConfig_ShouldEncryptPatientIdentifier()
    {
        var patient =
            @"{
                ""resourceType"": ""Patient"",
                ""id"": ""glossy"",
                ""identifier"": [
                    {
                        ""use"": ""usual"",
                        ""type"": {
                        ""coding"": [
                            {
                                ""system"": ""http://terminology.hl7.org/CodeSystem/v2-0203"",
                                ""code"": ""MR""
                            }
                        ]
                        },
                        ""system"": ""http://www.goodhealth.org/identifiers/mrn"",
                        ""value"": ""123456""
                    }
                ]
            }";

        var content = new StringContent(patient);
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");
        var response = await client.PostAsync(
            "/fhir/$de-identify",
            content,
            TestContext.Current.CancellationToken
        );

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );

        var encryptedPatient = new FhirJsonDeserializer().Deserialize<Patient>(responseContent);

        encryptedPatient.Identifier[0].Value.Should().NotBe("123456");
    }

    [Fact]
    public async Task PostDePseudonymize_WithDefaultConfig_ShouldDecryptPatientIdentifier()
    {
        var patient =
            @"{
                ""resourceType"": ""Patient"",
                ""id"": ""glossy"",
                ""identifier"": [
                    {
                        ""use"": ""usual"",
                        ""type"": {
                        ""coding"": [
                            {
                                ""system"": ""http://terminology.hl7.org/CodeSystem/v2-0203"",
                                ""code"": ""MR""
                            }
                        ]
                        },
                        ""system"": ""http://www.goodhealth.org/identifiers/mrn"",
                        ""value"": ""F36B23C5E72E3503D6C9659DDDEB7B5D61F6B90D5E5BE65FE08726315EF67CF3""
                    }
                ]
            }";

        var content = new StringContent(patient);
        content.Headers.Add("x-api-key", "dev");
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");
        var response = await client.PostAsync(
            "/fhir/$de-pseudonymize",
            content,
            TestContext.Current.CancellationToken
        );

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );
        var decryptedPatient = new FhirJsonDeserializer().Deserialize<Patient>(responseContent);

        decryptedPatient.Identifier[0].Value.Should().Be("123456");
    }

    [Fact]
    public async Task PostDeIdentify_WithCryptoHashKeySetViaAppSettingsConfig_ShouldCryptoHashValue()
    {
        var inlineConfig =
            @"
            fhirVersion: R4
            fhirPathRules:
              - path: Resource.id
                method: cryptoHash
              - path: Bundle.entry.fullUrl
                method: cryptoHash
              - path: Bundle.entry.request.url
                method: cryptoHash
        ";

        var factory = new CustomWebApplicationFactory<Startup>
        {
            CustomInMemorySettings = new Dictionary<string, string>
            {
                ["AnonymizationEngineConfigInline"] = inlineConfig,
                ["EnableMetrics"] = "false",
                ["Anonymization:CryptoHashKey"] = "test",
            },
        };

        var client = factory.CreateClient();

        var fhirClient = new FhirClient(
            "http://localhost/fhir",
            client,
            settings: new() { PreferredFormat = ResourceFormat.Json }
        );

        var fhirParser = new FhirJsonDeserializer();
        var input = fhirParser.Deserialize<Resource>(fhirBundleJson);
        var parameters = new Parameters().Add("resource", input);
        var response = await fhirClient.WholeSystemOperationAsync("de-identify", parameters);

        await Verify(response.ToJson(pretty: true), "json").UseDirectory("Snapshots");
    }

    [Fact]
    public async Task PostDeIdentify_WithKeyDerivationContextSet_ShouldDeriveDifferentCryptoHashKeyPerContext()
    {
        var inlineConfig =
            @"
            fhirVersion: R4
            fhirPathRules:
              - path: Resource.id
                method: cryptoHash
        ";

        async Task<string> DeIdentifyAndGetHashedIdAsync(string keyDerivationContext)
        {
            var settings = new Dictionary<string, string>
            {
                ["AnonymizationEngineConfigInline"] = inlineConfig,
                ["EnableMetrics"] = "false",
                ["Anonymization:CryptoHashKey"] = "test",
            };

            if (keyDerivationContext is not null)
            {
                settings["Anonymization:KeyDerivationContext"] = keyDerivationContext;
            }

            var factory = new CustomWebApplicationFactory<Startup>
            {
                CustomInMemorySettings = settings,
            };

            using var fhirClient = new FhirClient(
                "http://localhost/fhir",
                factory.CreateClient(),
                settings: new() { PreferredFormat = ResourceFormat.Json }
            );

            var fhirParser = new FhirJsonDeserializer();
            var input = fhirParser.Deserialize<Resource>(fhirBundleJson);
            var parameters = new Parameters().Add("resource", input);
            var response = await fhirClient.WholeSystemOperationAsync("de-identify", parameters);

            return ((Bundle)response).Entry[0].Resource.Id;
        }

        var withoutContext = await DeIdentifyAndGetHashedIdAsync(null);
        var withContextA = await DeIdentifyAndGetHashedIdAsync("project-a");
        var withContextARepeated = await DeIdentifyAndGetHashedIdAsync("project-a");
        var withContextB = await DeIdentifyAndGetHashedIdAsync("project-b");

        withContextA.Should().NotBe(withoutContext);
        withContextA.Should().NotBe(withContextB);
        withContextA.Should().Be(withContextARepeated);
    }

    [Fact]
    public async Task PostDeIdentifyThenDePseudonymize_WithKeyDerivationContextAndNoStaticEncryptKey_ShouldRoundTripEncryptedValue()
    {
        var inlineConfig =
            @"
            fhirVersion: R4
            fhirPathRules:
              - path: Patient.identifier.value
                method: encrypt
        ";

        var patient =
            @"{
                ""resourceType"": ""Patient"",
                ""id"": ""glossy"",
                ""identifier"": [
                    { ""value"": ""123456"" }
                ]
            }";

        using var factory = new CustomWebApplicationFactory<Startup>
        {
            CustomInMemorySettings = new Dictionary<string, string>
            {
                ["AnonymizationEngineConfigInline"] = inlineConfig,
                ["EnableMetrics"] = "false",
                ["Anonymization:CryptoHashKey"] = "test-crypto-hash-master",
                ["Anonymization:EncryptKey"] = "test-encrypt-master",
                ["Anonymization:KeyDerivationContext"] = "project-a",
            },
        };

        using var factoryClient = factory.CreateClient();

        var encryptContent = new StringContent(patient);
        encryptContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");
        var encryptResponse = await factoryClient.PostAsync(
            "/fhir/$de-identify",
            encryptContent,
            TestContext.Current.CancellationToken
        );
        encryptResponse.EnsureSuccessStatusCode();

        var encryptedPatientJson = await encryptResponse.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );
        var encryptedPatient = new FhirJsonDeserializer().Deserialize<Patient>(
            encryptedPatientJson
        );

        encryptedPatient.Identifier[0].Value.Should().NotBe("123456");

        var decryptContent = new StringContent(encryptedPatientJson);
        decryptContent.Headers.Add("x-api-key", "dev");
        decryptContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");
        var decryptResponse = await factoryClient.PostAsync(
            "/fhir/$de-pseudonymize",
            decryptContent,
            TestContext.Current.CancellationToken
        );
        decryptResponse.EnsureSuccessStatusCode();

        var decryptedPatientJson = await decryptResponse.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );
        var decryptedPatient = new FhirJsonDeserializer().Deserialize<Patient>(
            decryptedPatientJson
        );

        decryptedPatient.Identifier[0].Value.Should().Be("123456");
    }

    [Fact]
    public async Task PostDeIdentifyThenDePseudonymize_WithSameKeyDerivationContextButDifferentEncryptMasterKey_ShouldNotDecryptWithEachOthersKey()
    {
        var inlineConfig =
            @"
            fhirVersion: R4
            fhirPathRules:
              - path: Patient.identifier.value
                method: encrypt
        ";

        var patient =
            @"{
                ""resourceType"": ""Patient"",
                ""id"": ""glossy"",
                ""identifier"": [
                    { ""value"": ""123456"" }
                ]
            }";

        // Same CryptoHashKey and KeyDerivationContext for both - only EncryptKey differs - to
        // prove EncryptKey derives from itself as master, not from CryptoHashKey.
        using var encryptingFactory = new CustomWebApplicationFactory<Startup>
        {
            CustomInMemorySettings = new Dictionary<string, string>
            {
                ["AnonymizationEngineConfigInline"] = inlineConfig,
                ["EnableMetrics"] = "false",
                ["Anonymization:CryptoHashKey"] = "shared-crypto-hash-master",
                ["Anonymization:EncryptKey"] = "encrypt-master-one",
                ["Anonymization:KeyDerivationContext"] = "project-a",
            },
        };

        using var decryptingFactory = new CustomWebApplicationFactory<Startup>
        {
            CustomInMemorySettings = new Dictionary<string, string>
            {
                ["AnonymizationEngineConfigInline"] = inlineConfig,
                ["EnableMetrics"] = "false",
                ["Anonymization:CryptoHashKey"] = "shared-crypto-hash-master",
                ["Anonymization:EncryptKey"] = "encrypt-master-two",
                ["Anonymization:KeyDerivationContext"] = "project-a",
            },
        };

        using var encryptingClient = encryptingFactory.CreateClient();
        using var decryptingClient = decryptingFactory.CreateClient();

        var encryptContent = new StringContent(patient);
        encryptContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");
        var encryptResponse = await encryptingClient.PostAsync(
            "/fhir/$de-identify",
            encryptContent,
            TestContext.Current.CancellationToken
        );
        encryptResponse.EnsureSuccessStatusCode();

        var encryptedPatientJson = await encryptResponse.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );

        var decryptContent = new StringContent(encryptedPatientJson);
        decryptContent.Headers.Add("x-api-key", "dev");
        decryptContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");
        var decryptResponse = await decryptingClient.PostAsync(
            "/fhir/$de-pseudonymize",
            decryptContent,
            TestContext.Current.CancellationToken
        );
        decryptResponse.EnsureSuccessStatusCode();

        var decryptedPatientJson = await decryptResponse.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );
        var decryptedPatient = new FhirJsonDeserializer().Deserialize<Patient>(
            decryptedPatientJson
        );

        // DecryptProcessor swallows AES/padding errors and returns the (still encrypted) input
        // unchanged, so a mismatched key surfaces as "didn't decrypt back to the original".
        decryptedPatient.Identifier[0].Value.Should().NotBe("123456");
    }

    [Fact]
    public async Task PostDeIdentify_WithShouldAddSecurityTagSetToFalse_ShouldNotAddSecurityMetaDataToResult()
    {
        var inlineConfig =
            @"
            fhirVersion: R4
            fhirPathRules:
              - path: Resource.id
                method: redact
              - path: Bundle.entry.fullUrl
                method: cryptoHash
              - path: Bundle.entry.request.url
                method: cryptoHash
        ";

        var factory = new CustomWebApplicationFactory<Startup>
        {
            CustomInMemorySettings = new Dictionary<string, string>
            {
                ["AnonymizationEngineConfigInline"] = inlineConfig,
                ["EnableMetrics"] = "false",
                ["Anonymization:CryptoHashKey"] = "test",
                ["Anonymization:ShouldAddSecurityTag"] = "false",
            },
        };

        var client = factory.CreateClient();

        using var fhirClient = new FhirClient(
            "http://localhost/fhir",
            client,
            settings: new() { PreferredFormat = ResourceFormat.Json }
        );

        var fhirParser = new FhirJsonDeserializer();
        var input = fhirParser.Deserialize<Resource>(fhirBundleJson);
        var parameters = new Parameters().Add("resource", input);
        var response = await fhirClient.WholeSystemOperationAsync("de-identify", parameters);

        await Verify(response.ToJson(pretty: true), "json").UseDirectory("Snapshots");
    }

    [Fact]
    public async Task PostDeIdentify_WithRemoveMethodTargetingWholeBundleEntries_RemovesThoseEntries()
    {
        var bundleJson = """
            {
              "resourceType": "Bundle",
              "type": "collection",
              "entry": [
                {
                  "resource": {
                    "resourceType": "Patient",
                    "id": "patient-1",
                    "name": [{ "family": "Doe", "given": ["John"] }]
                  }
                },
                {
                  "resource": {
                    "resourceType": "Patient",
                    "id": "patient-2",
                    "name": [{ "family": "Smith", "given": ["Jane"] }]
                  }
                },
                {
                  "resource": {
                    "resourceType": "Observation",
                    "id": "observation-1",
                    "status": "final",
                    "code": { "text": "Body Weight" }
                  }
                }
              ]
            }
            """;

        var inlineConfig =
            @"
            fhirVersion: R4
            fhirPathRules:
              - path: Bundle.entry.where(resource is Patient)
                method: remove
        ";

        var factory = new CustomWebApplicationFactory<Startup>
        {
            CustomInMemorySettings = new Dictionary<string, string>
            {
                ["AnonymizationEngineConfigInline"] = inlineConfig,
                ["EnableMetrics"] = "false",
            },
        };

        var client = factory.CreateClient();

        var content = new StringContent(bundleJson);
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");

        var response = await client.PostAsync(
            "/fhir/$de-identify",
            content,
            TestContext.Current.CancellationToken
        );

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );
        var deIdentified = new FhirJsonDeserializer().Deserialize<Bundle>(responseContent);

        deIdentified.Entry.Should().ContainSingle();
        deIdentified.Entry[0].Resource.Should().BeOfType<Observation>();
        deIdentified.Meta.Security.Should().ContainSingle(coding => coding.Code == "REDACTED");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task PostDeIdentify_WithRemoveCombinedWithRedactAndCryptoHashOnSameResource_RemovesResourceRegardlessOfRuleOrder(
        bool removeRuleFirst
    )
    {
        const string cryptoHashKey = "test";

        // Patient.name (redact) and Resource.id (cryptoHash, a general rule that also matches
        // the Patient) both target fields on the very same Patient the other rule removes
        // wholesale. Since the removed entry - and everything nested in it - is excised from the
        // Bundle before the traversal descends into it, these should never actually run against
        // the Patient, regardless of which rule the config lists first.
        var inlineConfig = removeRuleFirst
            ? @"
            fhirVersion: R4
            fhirPathRules:
              - path: Bundle.entry.where(resource is Patient)
                method: remove
              - path: Patient.name
                method: redact
              - path: Resource.id
                method: cryptoHash
        "
            : @"
            fhirVersion: R4
            fhirPathRules:
              - path: Patient.name
                method: redact
              - path: Resource.id
                method: cryptoHash
              - path: Bundle.entry.where(resource is Patient)
                method: remove
        ";

        var bundleJson = """
            {
              "resourceType": "Bundle",
              "type": "collection",
              "entry": [
                {
                  "resource": {
                    "resourceType": "Patient",
                    "id": "patient-1",
                    "name": [{ "family": "Doe", "given": ["John"] }]
                  }
                },
                {
                  "resource": {
                    "resourceType": "Observation",
                    "id": "observation-1",
                    "status": "final",
                    "code": { "text": "Body Weight" }
                  }
                }
              ]
            }
            """;

        var factory = new CustomWebApplicationFactory<Startup>
        {
            CustomInMemorySettings = new Dictionary<string, string>
            {
                ["AnonymizationEngineConfigInline"] = inlineConfig,
                ["EnableMetrics"] = "false",
                ["Anonymization:CryptoHashKey"] = cryptoHashKey,
            },
        };

        var client = factory.CreateClient();

        var content = new StringContent(bundleJson);
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");

        var response = await client.PostAsync(
            "/fhir/$de-identify",
            content,
            TestContext.Current.CancellationToken
        );

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );
        var deIdentified = new FhirJsonDeserializer().Deserialize<Bundle>(responseContent);

        // the Patient - and the redact/cryptoHash rules that would have applied to it - are gone
        deIdentified.Entry.Should().ContainSingle();
        deIdentified.Entry[0].Resource.Should().BeOfType<Observation>();

        // the surviving Observation is still cryptoHashed normally, unaffected by the remove rule
        var remaining = (Observation)deIdentified.Entry[0].Resource;
        remaining
            .Id.Should()
            .Be(CryptoHashUtility.ComputeHmacSHA256Hash("observation-1", cryptoHashKey));
    }

    [Theory]
    [InlineData("redact")]
    [InlineData("remove")]
    public async Task PostDeIdentify_WithRedactOrRemoveTargetingComplexElement_BothRemoveTheWholeElement(
        string method
    )
    {
        var observationJson = """
            {
              "resourceType": "Observation",
              "id": "observation-1",
              "status": "final",
              "code": {
                "coding": [
                  { "system": "http://loinc.org", "code": "29463-7", "display": "Body Weight" }
                ],
                "text": "Body Weight"
              },
              "valueQuantity": {
                "value": 72.5,
                "unit": "kg"
              }
            }
            """;

        var inlineConfig =
            $@"
            fhirVersion: R4
            fhirPathRules:
              - path: Observation.code
                method: {method}
        ";

        var factory = new CustomWebApplicationFactory<Startup>
        {
            CustomInMemorySettings = new Dictionary<string, string>
            {
                ["AnonymizationEngineConfigInline"] = inlineConfig,
                ["EnableMetrics"] = "false",
            },
        };

        var client = factory.CreateClient();

        var content = new StringContent(observationJson);
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");

        var response = await client.PostAsync(
            "/fhir/$de-identify",
            content,
            TestContext.Current.CancellationToken
        );

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );
        // Observation.code is required (1..1) by the base spec, so the strict default
        // deserialization mode would refuse to re-parse a response that - as intended by this
        // test - no longer has one; SYNTAXONLY skips that content-rule validation (checks only
        // that the JSON itself is well-formed), matching the leniency the old FhirJsonParser had
        // here.
        var deIdentified = FhirJsonDeserializer.SYNTAXONLY.Deserialize<Observation>(
            responseContent
        );

        // the whole code element is gone - not just cleared - while sibling elements survive,
        // and both methods tag the resource the same way ("REDACTED" - remove reuses that code
        // rather than a dedicated one)
        deIdentified.Code.Should().BeNull();
        deIdentified.Status.Should().Be(ObservationStatus.Final);
        deIdentified.Value.Should().NotBeNull();
        deIdentified.Meta.Security.Should().ContainSingle(coding => coding.Code == "REDACTED");
    }
}
