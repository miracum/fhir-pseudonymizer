using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using FluentAssertions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace FhirPseudonymizer.Tests;

public class SnapshotTests : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly FhirClient fhirClient;

    public SnapshotTests(CustomWebApplicationFactory<Startup> factory)
    {
        var client = factory.CreateClient();

        fhirClient = new FhirClient("http://localhost/fhir", client);
    }

    [Fact]
    public async Task GetMetadata_ReturnsSuccessAndFhirJsonContentType()
    {
        var response = await fhirClient.CapabilityStatementAsync();
    }
}
