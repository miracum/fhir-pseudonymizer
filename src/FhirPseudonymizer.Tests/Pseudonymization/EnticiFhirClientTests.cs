using System.Net;
using System.Net.Http.Headers;
using FhirPseudonymizer.Pseudonymization.Entici;
using Microsoft.Extensions.Logging;

namespace FhirPseudonymizer.Tests.Pseudonymization;

public class EnticiFhirClientTests
{
    private static readonly Uri testBaseAddress = new("http://entici/");

    private readonly HttpMessageHandler messageHandler;
    private readonly IHttpClientFactory clientFactory;

    private const string ResponseContent = $$"""
        {
            "resourceType": "Parameters",
            "parameter": [
                {
                    "name": "pseudonym",
                    "valueIdentifier": {
                        "use": "secondary",
                        "system": "urn:fdc:difuture.de:trustcenter.plain",
                        "value": "a-test-pseudonym"
                    }
                }
            ]
        }
        """;

    public EnticiFhirClientTests()
    {
        messageHandler = CreateHttpMessageHandler();
        clientFactory = CreateHttpClientFactory(messageHandler);
    }

    [Fact]
    public async Task GetOrCreatePseudonymFor_WithNullSettings_ShouldThrow()
    {
        var client = new EnticiFhirClient(A.Fake<ILogger<EnticiFhirClient>>(), clientFactory);

        Func<Task> act = async () => await client.GetOrCreatePseudonymFor("42", "domain", null);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetOrCreatePseudonymFor_WithNoEnticiSettings_ShouldThrow()
    {
        var client = new EnticiFhirClient(A.Fake<ILogger<EnticiFhirClient>>(), clientFactory);

        var settings = new Dictionary<string, object> { ["not-entici"] = new object() };

        Func<Task> act = async () => await client.GetOrCreatePseudonymFor("42", "domain", settings);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GetOrCreatePseudonymFor_WithNullEnticiSettings_ShouldThrow()
    {
        var client = new EnticiFhirClient(A.Fake<ILogger<EnticiFhirClient>>(), clientFactory);

        var settings = new Dictionary<string, object> { ["entici"] = null };

        Func<Task> act = async () => await client.GetOrCreatePseudonymFor("42", "domain", settings);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GetOrCreatePseudonymFor_WithValidEnticiSettings_ShouldReturnExpectedPseudonym()
    {
        var client = new EnticiFhirClient(A.Fake<ILogger<EnticiFhirClient>>(), clientFactory);

        var settings = new Dictionary<string, object>
        {
            ["entici"] = new Dictionary<object, object> { ["resourceType"] = "Encounter" },
        };

        var response = await client.GetOrCreatePseudonymFor("42", "domain", settings);

        response.Should().BeEquivalentTo("a-test-pseudonym");

        VerifyRequest(HttpMethod.Post, "$pseudonymize");
    }

    [Fact]
    public async Task GetOrCreatePseudonymFor_WithValidEnticiSettingsAndTargetSystem_ShouldReturnExpectedPseudonym()
    {
        var client = new EnticiFhirClient(A.Fake<ILogger<EnticiFhirClient>>(), clientFactory);

        var settings = new Dictionary<string, object>
        {
            ["entici"] = new Dictionary<object, object>
            {
                ["resourceType"] = "Patient",
                ["project"] = "https://fhir.example.com/test",
            },
        };

        var response = await client.GetOrCreatePseudonymFor("42", "domain", settings);

        response.Should().BeEquivalentTo("a-test-pseudonym");

        VerifyRequest(HttpMethod.Post, "$pseudonymize");
    }

    [Fact]
    public async Task GetOrCreatePseudonymFor_WithInvalidResourceTypeInEnticiSettings_ShouldThrow()
    {
        var client = new EnticiFhirClient(A.Fake<ILogger<EnticiFhirClient>>(), clientFactory);

        var settings = new Dictionary<string, object>
        {
            ["entici"] = new Dictionary<object, object>
            {
                ["resourceType"] = "Not-A-FHIR-Resource-Type",
            },
        };

        Func<Task> act = async () => await client.GetOrCreatePseudonymFor("42", "domain", settings);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    private void VerifyRequest(HttpMethod requestMethod, string requestUri)
    {
        A.CallTo(messageHandler)
            .Where(_ => _.Method.Name == "SendAsync")
            .WhenArgumentsMatch(
                (HttpRequestMessage r, CancellationToken _) =>
                    r.Method == requestMethod
                    && r.RequestUri == new Uri(testBaseAddress.AbsoluteUri + requestUri)
            )
            .MustHaveHappenedOnceExactly();
    }

    private static IHttpClientFactory CreateHttpClientFactory(HttpMessageHandler httpMessageHandler)
    {
        var client = new HttpClient(httpMessageHandler) { BaseAddress = testBaseAddress };

        var factory = A.Fake<IHttpClientFactory>();
        A.CallTo(() => factory.CreateClient(A<string>._)).Returns(client);
        return factory;
    }

    private static HttpMessageHandler CreateHttpMessageHandler()
    {
        var handler = A.Fake<HttpMessageHandler>();
        A.CallTo(handler)
            .Where(_ => _.Method.Name == "SendAsync")
            .WithReturnType<Task<HttpResponseMessage>>()
            .Returns(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    // these values have to be set since they are used by the FhirClient:
                    // https://github.com/FirelyTeam/firely-net-common/blob/5899ce463f6cf166520cbbe6322310940942f81c/src/Hl7.Fhir.Support.Poco/Rest/HttpToEntryExtensions.cs#L28 &
                    // https://github.com/FirelyTeam/firely-net-sdk/blob/f71543edc34c9edecf0f13af50d35e9e57ca353a/src/Hl7.Fhir.Core/Rest/TypedEntryResponseToBundle.cs#L24
                    Content = new StringContent(
                        ResponseContent,
                        new MediaTypeHeaderValue("application/json+fhir")
                    ),
                    RequestMessage = new HttpRequestMessage(HttpMethod.Post, testBaseAddress),
                }
            );

        return handler;
    }
}
