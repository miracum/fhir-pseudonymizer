using System.Net;
using System.Net.Http.Headers;
using FhirPseudonymizer.Config;
using FhirPseudonymizer.Pseudonymization;
using FhirPseudonymizer.Pseudonymization.GPas;
using Hl7.Fhir.Rest;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace FhirPseudonymizer.Tests.Pseudonymization;

public class GPasFhirClientTests
{
    public static IEnumerable<object[]> GetOrCreatePseudonymFor_Data()
    {
        yield return new object[]
        {
            "1.10.1",
            HttpMethod.Get,
            "$pseudonymize-allow-create?domain=domain&original=42",
        };
        yield return new object[] { "1.10.2", HttpMethod.Post, "$pseudonymize-allow-create" };
        yield return new object[] { "1.10.3", HttpMethod.Post, "$pseudonymizeAllowCreate" };
    }

    public static IEnumerable<object[]> GetOriginalValueFor_Data()
    {
        yield return new object[]
        {
            "1.10.1",
            HttpMethod.Get,
            "$de-pseudonymize?domain=domain&pseudonym=42",
        };
        yield return new object[] { "1.10.2", HttpMethod.Post, "$de-pseudonymize" };
        yield return new object[] { "1.10.3", HttpMethod.Post, "$dePseudonymize" };
    }

    private static readonly Uri testBaseAddress = new("http://gpas");

    private const string ResponseContent = $$"""
        {
            "resourceType": "Parameters",
            "parameter": [
                {
                    "name": "pseudonym",
                    "part": [
                        {
                            "name": "pseudonym",
                            "valueIdentifier": {
                                "system": "https://ths-greifswald.de/gpas",
                                "value": "24"
                            }
                        }
                    ]
                },
                {
                    "name": "42",
                    "valueString": "24"
                }
            ]
        }
        """;

    private readonly HttpMessageHandler messageHandler;
    private readonly IHttpClientFactory clientFactory;

    public GPasFhirClientTests()
    {
        messageHandler = CreateHttpMessageHandler();
        clientFactory = CreateHttpClientFactory(messageHandler);
    }

    [Theory]
    [MemberData(nameof(GetOrCreatePseudonymFor_Data))]
    public async Task GetOrCreatePseudonymFor_ResolvesToApiVersionOperation(
        string gpasVersion,
        HttpMethod requestMethod,
        string requestUri
    )
    {
        // create gpas client
        var gpasClient = CreateGPasClient(gpasVersion);

        // act
        await gpasClient.GetOrCreatePseudonymFor(
            "42",
            "domain",
            cancellationToken: TestContext.Current.CancellationToken
        );

        // verify
        VerifyRequest(requestMethod, requestUri);
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

    [Theory]
    [MemberData(nameof(GetOriginalValueFor_Data))]
    public async Task GetOriginalValueFor_ResolvesToApiVersionOperation(
        string gpasVersion,
        HttpMethod requestMethod,
        string requestUri
    )
    {
        // create gpas client
        var gpasClient = CreateGPasClient(gpasVersion);

        // act
        await gpasClient.GetOriginalValueFor(
            "42",
            "domain",
            cancellationToken: TestContext.Current.CancellationToken
        );

        // verify request uri and method
        VerifyRequest(requestMethod, requestUri);
    }

    private IPseudonymServiceClient CreateGPasClient(
        string gPasVersion,
        HttpMessageHandler handler = null
    )
    {
        var config = new GPasConfig { Version = gPasVersion };
        var factory = handler is null ? clientFactory : CreateHttpClientFactory(handler);

        return new GPasFhirClient(A.Fake<ILogger<GPasFhirClient>>(), factory, config);
    }

    // The gPAS V2/V2x de-pseudonymize paths deliberately swallow backend failures and return the
    // pseudonym unchanged. A caller-requested cancellation must not take that path, or a cancelled
    // $de-pseudonymize would quietly emit still-pseudonymized data for the rest of the resource.
    [Theory]
    [InlineData("1.10.2")]
    [InlineData("1.10.3")]
    public async Task GetOriginalValueFor_WhenTheCallerCancels_ThrowsInsteadOfReturningThePseudonym(
        string gpasVersion
    )
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var gpasClient = CreateGPasClient(gpasVersion, CreateThrowingHttpMessageHandler());

        var act = async () =>
            await gpasClient.GetOriginalValueFor("42", "domain", cancellationToken: cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    // The flip side: an HttpClient timeout also surfaces as an OperationCanceledException, but
    // with the caller's token unsignalled. That case keeps the pre-existing fallback.
    [Theory]
    [InlineData("1.10.2")]
    [InlineData("1.10.3")]
    public async Task GetOriginalValueFor_WhenTheBackendFailsWithoutCancellation_FallsBackToThePseudonym(
        string gpasVersion
    )
    {
        var gpasClient = CreateGPasClient(gpasVersion, CreateThrowingHttpMessageHandler());

        var result = await gpasClient.GetOriginalValueFor(
            "42",
            "domain",
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.Should().Be("42");
    }

    [Theory]
    [InlineData("1.10.2", HttpStatusCode.ServiceUnavailable)]
    [InlineData("1.10.3", HttpStatusCode.ServiceUnavailable)]
    [InlineData("1.10.2", HttpStatusCode.Unauthorized)]
    [InlineData("1.10.3", HttpStatusCode.Unauthorized)]
    [InlineData("1.10.2", HttpStatusCode.Forbidden)]
    [InlineData("1.10.3", HttpStatusCode.Forbidden)]
    public async Task GetOrCreatePseudonymFor_WhenGPasIsTransientlyUnavailable_ThrowsTransientPseudonymizationException(
        string gpasVersion,
        HttpStatusCode statusCode
    )
    {
        var gpasClient = CreateGPasClient(gpasVersion, CreateFailingHttpMessageHandler(statusCode));

        var act = async () =>
            await gpasClient.GetOrCreatePseudonymFor(
                "42",
                "domain",
                cancellationToken: TestContext.Current.CancellationToken
            );

        await act.Should().ThrowAsync<TransientPseudonymizationException>();
    }

    [Theory]
    [InlineData("1.10.2")]
    [InlineData("1.10.3")]
    public async Task GetOrCreatePseudonymFor_WhenGPasRejectsTheInput_ThrowsTheOriginalException(
        string gpasVersion
    )
    {
        var gpasClient = CreateGPasClient(
            gpasVersion,
            CreateFailingHttpMessageHandler(HttpStatusCode.BadRequest)
        );

        var act = async () =>
            await gpasClient.GetOrCreatePseudonymFor(
                "42",
                "domain",
                cancellationToken: TestContext.Current.CancellationToken
            );

        await act.Should()
            .ThrowAsync<Exception>()
            .Where(exc => exc.GetType() != typeof(TransientPseudonymizationException));
    }

    private static HttpMessageHandler CreateFailingHttpMessageHandler(HttpStatusCode statusCode)
    {
        var handler = A.Fake<HttpMessageHandler>();
        A.CallTo(handler)
            .Where(_ => _.Method.Name == "SendAsync")
            .WithReturnType<Task<HttpResponseMessage>>()
            .Returns(
                new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(
                        """
                        {"resourceType":"OperationOutcome","issue":[{"severity":"error","code":"exception","diagnostics":"backend error"}]}
                        """,
                        new MediaTypeHeaderValue("application/json+fhir")
                    ),
                    RequestMessage = new HttpRequestMessage(HttpMethod.Post, testBaseAddress),
                }
            );

        return handler;
    }

    private static HttpMessageHandler CreateThrowingHttpMessageHandler()
    {
        var handler = A.Fake<HttpMessageHandler>();
        A.CallTo(handler)
            .Where(_ => _.Method.Name == "SendAsync")
            .WithReturnType<Task<HttpResponseMessage>>()
            .Throws(new TaskCanceledException());

        return handler;
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
