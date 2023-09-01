using System.Net;
using System.Net.Http.Headers;
using FhirPseudonymizer.Pseudonymization;

namespace FhirPseudonymizer.Tests;

public class PseudonymizationServiceConfigurationTests
    : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> factory;

    public PseudonymizationServiceConfigurationTests(CustomWebApplicationFactory<Startup> factory)
    {
        this.factory = factory;
    }

    public class PseudonymizationServiceTestData : TheoryData<PseudonymizationServiceType>
    {
        public PseudonymizationServiceTestData()
        {
            foreach (var backend in Enum.GetValues<PseudonymizationServiceType>())
            {
                Add(backend);
            }
        }
    }

    [Theory]
    [ClassData(typeof(PseudonymizationServiceTestData))]
    public async Task PostDeIdentify_WithConfiguredPseudonymizationService_ShouldSucceed(
        PseudonymizationServiceType serviceType
    )
    {
        factory.CustomInMemorySettings = new Dictionary<string, string>
        {
            ["PseudonymizationService"] = serviceType.ToString(),
            ["EnableMetrics"] = "false",
        };

        var client = factory.CreateClient();

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
}
