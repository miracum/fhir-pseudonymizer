using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using FluentAssertions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace FhirPseudonymizer.Tests
{
    public class IntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public IntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetMetadata_ReturnsSuccessAndFhirJsonContentType()
        {
            var response = await _client.GetAsync("/fhir/metadata");

            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType.ToString()
                .Should().Be("application/fhir+json; charset=utf-8");
        }

        [Theory]
        [InlineData("/ready")]
        [InlineData("/live")]
        public async Task ReadyAndLiveChecks_ReturnSuccess(string url)
        {
            var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("/fhir/$de-identify")]
        [InlineData("/fhir/$de-pseudonymize")]
        public async Task PostToFhirOperation_WithInvalidContent_ShouldReturnBadRequest(string url)
        {
            var content = new StringContent("asd");
            content.Headers.Add("x-api-key", "dev");
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");

            var response = await _client.PostAsync(url, content);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PostDeIdentify_WithoutApiKeyHeader_ShouldBeAllowed()
        {
            var content = new StringContent(@"{""resourceType"":""Patient""}");
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");
            var response = await _client.PostAsync("/fhir/$de-identify", content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task PostDePseudonymize_WithoutApiKeyHeader_ShouldReturnUnauthorized()
        {
            var content = new StringContent("");
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");
            var response = await _client.PostAsync("/fhir/$de-pseudonymize", content);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task PostDeIdentify_WithDefaultConfig_ShouldEncryptPatientIdentifier()
        {
            var patient = @"{
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
            var response = await _client.PostAsync("/fhir/$de-identify", content);

            response.EnsureSuccessStatusCode();

            var encryptedPatient = new FhirJsonParser().Parse<Patient>(response.Content.ReadAsStringAsync().Result);

            encryptedPatient.Identifier[0].Value.Should().NotBe("123456");
        }

        [Fact]
        public async Task PostDePseudonymize_WithDefaultConfig_ShouldDecryptPatientIdentifier()
        {
            var patient = @"{
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
            var response = await _client.PostAsync("/fhir/$de-pseudonymize", content);

            response.EnsureSuccessStatusCode();

            var decryptedPatient = new FhirJsonParser().Parse<Patient>(response.Content.ReadAsStringAsync().Result);

            decryptedPatient.Identifier[0].Value.Should().Be("123456");
        }
    }
}
