using System.Net;
using System.Net.Http.Headers;
using FhirPseudonymizer.Pseudonymization.Mii;
using Microsoft.Extensions.Logging;

namespace FhirPseudonymizer.Tests.Pseudonymization;

public class MiiFhirClientTests
{
    private static readonly Uri testBaseAddress = new("http://mii-backend/");

    private const string PseudonymizeResponseContent = """
        {
            "resourceType": "Parameters",
            "parameter": [
                {
                    "name": "target",
                    "valueString": "test-domain"
                },
                {
                    "name": "original",
                    "valueString": "original-value"
                },
                {
                    "name": "pseudonym",
                    "valueString": "a-test-pseudonym"
                }
            ]
        }
        """;

    private const string DePseudonymizeResponseContent = """
        {
            "resourceType": "Parameters",
            "parameter": [
                {
                    "name": "original",
                    "part": [
                        {
                            "name": "target",
                            "valueString": "test-domain"
                        },
                        {
                            "name": "value",
                            "valueString": "the-original-value"
                        },
                        {
                            "name": "pseudonym",
                            "valueString": "a-test-pseudonym"
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

        var result = await client.GetOrCreatePseudonymFor("original-value", "test-domain");

        result.Should().Be("a-test-pseudonym");

        VerifyRequest(handler, HttpMethod.Post, "$pseudonymize");
    }

    [Fact]
    public async Task GetOriginalValueFor_WithValidInput_ShouldReturnOriginalValue()
    {
        var handler = CreateHttpMessageHandler(DePseudonymizeResponseContent);
        var factory = CreateHttpClientFactory(handler);
        var client = new MiiFhirClient(A.Fake<ILogger<MiiFhirClient>>(), factory);

        var result = await client.GetOriginalValueFor("a-test-pseudonym", "test-domain");

        result.Should().Be("the-original-value");

        VerifyRequest(handler, HttpMethod.Post, "$de-pseudonymize");
    }

    [Fact]
    public async Task GetOriginalValueFor_WithIdentifierValue_ShouldReturnIdentifierValue()
    {
        const string responseWithIdentifier = """
            {
                "resourceType": "Parameters",
                "parameter": [
                    {
                        "name": "original",
                        "part": [
                            {
                                "name": "target",
                                "valueString": "test-domain"
                            },
                            {
                                "name": "value",
                                "valueIdentifier": {
                                    "system": "http://example.com",
                                    "value": "identifier-original"
                                }
                            }
                        ]
                    }
                ]
            }
            """;

        var handler = CreateHttpMessageHandler(responseWithIdentifier);
        var factory = CreateHttpClientFactory(handler);
        var client = new MiiFhirClient(A.Fake<ILogger<MiiFhirClient>>(), factory);

        var result = await client.GetOriginalValueFor("a-pseudonym", "test-domain");

        result.Should().Be("identifier-original");
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

        Func<Task> act = async () => await client.GetOriginalValueFor("pseudonym", "domain");

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

    private static HttpMessageHandler CreateHttpMessageHandler(string responseContent)
    {
        var handler = A.Fake<HttpMessageHandler>();
        A.CallTo(handler)
            .Where(_ => _.Method.Name == "SendAsync")
            .WithReturnType<Task<HttpResponseMessage>>()
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
