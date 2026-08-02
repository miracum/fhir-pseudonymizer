using System.Text;
using FhirPseudonymizer.Controllers;
using Hl7.Fhir.Model;

namespace FhirPseudonymizer.Tests.Pseudonymization.DeIdentification;

public class DeIdentifyRequestTests
{
    private const string SampleYamlConfig = """
        fhirVersion: R4
        fhirPathRules:
          - path: Patient.name
            method: redact
        """;

    [Fact]
    public void FromFhirParameters_WithFullRequest_ParsesAllParts()
    {
        var configBytes = Encoding.UTF8.GetBytes(SampleYamlConfig);
        var parameters = new Parameters()
            .Add("config", new Attachment { ContentType = "application/yaml", Data = configBytes })
            .Add("resource", new Patient { Id = "example" });

        var request = DeIdentifyRequest.FromFhirParameters(parameters);

        request.Config.Should().NotBeNull();
        request.Config.ContentType.Should().Be("application/yaml");
        request.Config.Data.Should().Equal(configBytes);

        request.Resource.Should().BeOfType<Patient>();
        ((Patient)request.Resource).Id.Should().Be("example");
    }

    [Fact]
    public void FromFhirParameters_WithoutConfig_LeavesConfigNull()
    {
        var parameters = new Parameters().Add("resource", new Patient());

        var request = DeIdentifyRequest.FromFhirParameters(parameters);

        request.Config.Should().BeNull();
        request.Resource.Should().NotBeNull();
    }

    [Fact]
    public void FromFhirParameters_WithoutResourcePart_LeavesResourceNull()
    {
        var parameters = new Parameters().Add(
            "config",
            new Attachment { ContentType = "application/yaml" }
        );

        var request = DeIdentifyRequest.FromFhirParameters(parameters);

        request.Resource.Should().BeNull();
    }

    [Fact]
    public void FromFhirParameters_WithNullParameters_Throws()
    {
        Action act = () => DeIdentifyRequest.FromFhirParameters(null);

        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void ToFhirParameters_ThenFromFhirParameters_RoundTrips()
    {
        var original = new DeIdentifyRequest
        {
            Config = new Attachment
            {
                ContentType = "application/yaml",
                Data = Encoding.UTF8.GetBytes(SampleYamlConfig),
            },
            Resource = new Patient { Id = "example" },
        };

        var roundTripped = DeIdentifyRequest.FromFhirParameters(original.ToFhirParameters());

        roundTripped.Should().BeEquivalentTo(original);
    }
}
