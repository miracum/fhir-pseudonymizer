using System.Net;
using System.Net.Http.Headers;
using FhirPseudonymizer.Pseudonymization.Mii;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Logging;

namespace FhirPseudonymizer.Tests.Pseudonymization;

public class MiiFhirClientTests
{
    private static readonly Uri testBaseAddress = new("http://mii-backend/");

    private const string TestContextSystem = "https://sample/context-system";

    private const string TestOriginalSystem = "https://sample/original-system";

    private static readonly Dictionary<string, object> testSettings = new()
    {
        ["mii"] = new Dictionary<object, object>
        {
            ["contextSystem"] = TestContextSystem,
            ["originalSystem"] = TestOriginalSystem,
        },
    };

    private const string PseudonymizeResponseContent = """
        {
            "resourceType": "Parameters",
            "id": "PseudonymizeIdentifierResponseExample",
            "parameter": [
                {
                    "name": "context",
                    "valueIdentifier": {
                        "system": "https://sample/psn-system",
                        "value": "Transfer1"
                    }
                },
                {
                    "name": "original",
                    "valueIdentifier": {
                        "system": "https://sample/psn-system",
                        "value": "D1CL0CAL1"
                    }
                },
                {
                    "name": "pseudonym",
                    "valueIdentifier": {
                        "system": "https://sample/psn-system",
                        "value": "H3RAU56A8E"
                    }
                }
            ]
        }
        """;

    private const string DePseudonymizeResponseContent = """
        {
            "resourceType": "Parameters",
            "id": "DePseudonymizeResponseWithIdentifierExample",
            "parameter": [
                {
                    "name": "original",
                    "part": [
                        {
                            "name": "context",
                            "valueIdentifier": {
                                "system": "https://sample/psn-system",
                                "value": "Transfer1"
                            }
                        },
                        {
                            "name": "value",
                            "valueIdentifier": {
                                "system": "https://sample/psn-system",
                                "value": "D1CL0CAL1"
                            }
                        },
                        {
                            "name": "pseudonym",
                            "valueIdentifier": {
                                "system": "https://sample/psn-system",
                                "value": "H3RAU56A8E"
                            }
                        }
                    ]
                }
            ]
        }
        """;

    [Fact]
    public async Task GetOrCreatePseudonymFor_WithValidInput_ShouldReturnPseudonym()
    {
        var handler = CreateHttpMessageHandler(PseudonymizeResponseContent);
        var factory = CreateHttpClientFactory(handler);
        var client = new MiiFhirClient(A.Fake<ILogger<MiiFhirClient>>(), factory);

        var result = await client.GetOrCreatePseudonymFor("D1CL0CAL1", "Transfer1", testSettings);

        result.Should().Be("H3RAU56A8E");

        VerifyRequest(handler, HttpMethod.Post, "$pseudonymize");
    }

    [Fact]
    public async Task GetOrCreatePseudonymFor_ShouldSendContextAndOriginalAsIdentifiers()
    {
        var requests = new List<string>();
        var handler = CreateHttpMessageHandler(PseudonymizeResponseContent, requests);
        var factory = CreateHttpClientFactory(handler);
        var client = new MiiFhirClient(A.Fake<ILogger<MiiFhirClient>>(), factory);

        await client.GetOrCreatePseudonymFor("D1CL0CAL1", "Transfer1", testSettings);

        var sent = new FhirJsonParser().Parse<Parameters>(requests.Single());

        sent.GetSingleValue<Identifier>("context")
            .Should()
            .BeEquivalentTo(new Identifier(TestContextSystem, "Transfer1"));
        sent.GetSingleValue<Identifier>("original")
            .Should()
            .BeEquivalentTo(new Identifier(TestOriginalSystem, "D1CL0CAL1"));
        sent.Parameter.Should().NotContain(p => p.Name == "allowCreate");
    }

    [Fact]
    public async Task GetOriginalValueFor_WithValidInput_ShouldReturnOriginalValue()
    {
        var handler = CreateHttpMessageHandler(DePseudonymizeResponseContent);
        var factory = CreateHttpClientFactory(handler);
        var client = new MiiFhirClient(A.Fake<ILogger<MiiFhirClient>>(), factory);

        var result = await client.GetOriginalValueFor("H3RAU56A8E", "Transfer1", testSettings);

        result.Should().Be("D1CL0CAL1");

        VerifyRequest(handler, HttpMethod.Post, "$de-pseudonymize");
    }

    [Fact]
    public async Task GetOriginalValueFor_ShouldSendContextAndPseudonymAsIdentifiers()
    {
        var requests = new List<string>();
        var handler = CreateHttpMessageHandler(DePseudonymizeResponseContent, requests);
        var factory = CreateHttpClientFactory(handler);
        var client = new MiiFhirClient(A.Fake<ILogger<MiiFhirClient>>(), factory);

        await client.GetOriginalValueFor("H3RAU56A8E", "Transfer1", testSettings);

        var sent = new FhirJsonParser().Parse<Parameters>(requests.Single());

        sent.GetSingleValue<Identifier>("context")
            .Should()
            .BeEquivalentTo(new Identifier(TestContextSystem, "Transfer1"));
        sent.GetSingleValue<Identifier>("pseudonym")
            .Should()
            .BeEquivalentTo(new Identifier(null, "H3RAU56A8E"));
    }

    [Fact]
    public async Task GetOrCreatePseudonymFor_WithoutSystemSettings_ShouldSendIdentifierValuesOnly()
    {
        var requests = new List<string>();
        var handler = CreateHttpMessageHandler(PseudonymizeResponseContent, requests);
        var factory = CreateHttpClientFactory(handler);
        var client = new MiiFhirClient(A.Fake<ILogger<MiiFhirClient>>(), factory);

        await client.GetOrCreatePseudonymFor("D1CL0CAL1", "Transfer1", settings: null);

        var sent = new FhirJsonParser().Parse<Parameters>(requests.Single());

        sent.GetSingleValue<Identifier>("context")
            .Should()
            .BeEquivalentTo(new Identifier(null, "Transfer1"));
        sent.GetSingleValue<Identifier>("original")
            .Should()
            .BeEquivalentTo(new Identifier(null, "D1CL0CAL1"));
    }

    [Fact]
    public async Task GetOriginalValueFor_WithEmptyResponse_ShouldThrow()
    {
        const string emptyResponse = """
            {
                "resourceType": "Parameters",
                "parameter": []
            }
            """;

        var handler = CreateHttpMessageHandler(emptyResponse);
        var factory = CreateHttpClientFactory(handler);
        var client = new MiiFhirClient(A.Fake<ILogger<MiiFhirClient>>(), factory);

        Func<Task> act = async () =>
            await client.GetOriginalValueFor("pseudonym", "domain", testSettings);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    private static void VerifyRequest(HttpMessageHandler handler, HttpMethod method, string path)
    {
        A.CallTo(handler)
            .Where(_ => _.Method.Name == "SendAsync")
            .WhenArgumentsMatch(
                (HttpRequestMessage r, CancellationToken _) =>
                    r.Method == method
                    && r.RequestUri == new Uri(testBaseAddress.AbsoluteUri + path)
            )
            .MustHaveHappenedOnceExactly();
    }

    private static IHttpClientFactory CreateHttpClientFactory(HttpMessageHandler handler)
    {
        var client = new HttpClient(handler) { BaseAddress = testBaseAddress };

        var factory = A.Fake<IHttpClientFactory>();
        A.CallTo(() => factory.CreateClient(A<string>._)).Returns(client);
        return factory;
    }

    /// <summary>
    /// Creates a fake handler that always answers with <paramref name="responseContent" />.
    /// If <paramref name="sentRequestBodies" /> is set, the handler adds the body of each
    /// request to that list.
    /// </summary>
    private static HttpMessageHandler CreateHttpMessageHandler(
        string responseContent,
        List<string> sentRequestBodies = null
    )
    {
        var handler = A.Fake<HttpMessageHandler>();
        A.CallTo(handler)
            .Where(_ => _.Method.Name == "SendAsync")
            .WithReturnType<Task<HttpResponseMessage>>()
            .Invokes(call =>
                sentRequestBodies?.Add(
                    ((HttpRequestMessage)call.Arguments[0])
                        .Content.ReadAsStringAsync()
                        .GetAwaiter()
                        .GetResult()
                )
            )
            .Returns(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        responseContent,
                        new MediaTypeHeaderValue("application/json+fhir")
                    ),
                    RequestMessage = new HttpRequestMessage(HttpMethod.Post, testBaseAddress),
                }
            );

        return handler;
    }
}
