using System.Net;
using System.Net.Http.Headers;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;

namespace FhirPseudonymizer.Tests;

[UsesVerify]
public class IntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly HttpClient client;

    public IntegrationTests(CustomWebApplicationFactory<Startup> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMetadata_ReturnsSuccessAndFhirJsonContentType()
    {
        var response = await client.GetAsync("/fhir/metadata");

        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType
            .ToString()
            .Should()
            .Be("application/fhir+json; charset=utf-8");
    }

    [Theory]
    [InlineData("/ready")]
    [InlineData("/live")]
    public async Task ReadyAndLiveChecks_ReturnSuccess(string url)
    {
        var response = await client.GetAsync(url);

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

        var response = await client.PostAsync(url, content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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
        var response = await client.PostAsync("/fhir/$de-identify", content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostDePseudonymize_WithoutApiKeyHeader_ShouldReturnUnauthorized()
    {
        var content = new StringContent("");
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");
        var response = await client.PostAsync("/fhir/$de-pseudonymize", content);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostDePseudonymize_WithWrongApiKey_ShouldReturnUnauthorized()
    {
        var content = new StringContent("");
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");
        content.Headers.Add("x-api-key", "wrong-key");
        var response = await client.PostAsync("/fhir/$de-pseudonymize", content);

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
        var response = await client.PostAsync("/fhir/$de-identify", content);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        var encryptedPatient = new FhirJsonParser().Parse<Patient>(responseContent);

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
        var response = await client.PostAsync("/fhir/$de-pseudonymize", content);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var decryptedPatient = await new FhirJsonParser().ParseAsync<Patient>(responseContent);

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

        var inputJson =
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
        }
        ";

        var factory = new CustomWebApplicationFactory<Startup>
        {
            CustomInMemorySettings = new Dictionary<string, string>
            {
                ["AnonymizationEngineConfigInline"] = inlineConfig,
                ["EnableMetrics"] = "false",
                ["Anonymization:CryptoHashKey"] = "test",
            }
        };

        var client = factory.CreateClient();

        var fhirClient = new FhirClient(
            "http://localhost/fhir",
            client,
            settings: new() { PreferredFormat = ResourceFormat.Json }
        );

        var fhirParser = new FhirJsonParser();
        var input = await fhirParser.ParseAsync<Resource>(inputJson);
        var parameters = new Parameters().Add("resource", input);
        var response = await fhirClient.WholeSystemOperationAsync("de-identify", parameters);

        await Verify(response.ToJson(new() { Pretty = true }), "fhir.json")
            .UseDirectory("Snapshots");
    }
}
