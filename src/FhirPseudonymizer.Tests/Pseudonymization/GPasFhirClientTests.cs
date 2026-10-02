using System.Net;
using System.Net.Http.Headers;
using FhirPseudonymizer.Pseudonymization;
using FhirPseudonymizer.Pseudonymization.GPas;
using Hl7.Fhir.Rest;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace FhirPseudonymizer.Tests.Pseudonymization;

public class GPasFhirClientTests
{
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

    [Fact]
    public async Task GetOrCreatePseudonymFor_PostsToPseudonymizeAllowCreateOperation()
    {
        // create gpas client
        var gpasClient = CreateGPasClient();

        // act
        await gpasClient.GetOrCreatePseudonymFor(
            "42",
            "domain",
            cancellationToken: TestContext.Current.CancellationToken
        );

        // verify
        VerifyRequest(HttpMethod.Post, "$pseudonymizeAllowCreate");
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

    [Fact]
    public async Task GetOriginalValueFor_PostsToDePseudonymizeOperation()
    {
        // create gpas client
        var gpasClient = CreateGPasClient();

        // act
        await gpasClient.GetOriginalValueFor(
            "42",
            "domain",
            cancellationToken: TestContext.Current.CancellationToken
        );

        // verify request uri and method
        VerifyRequest(HttpMethod.Post, "$dePseudonymize");
    }

    private IPseudonymServiceClient CreateGPasClient(HttpMessageHandler handler = null)
    {
        var factory = handler is null ? clientFactory : CreateHttpClientFactory(handler);

        return new GPasFhirClient(A.Fake<ILogger<GPasFhirClient>>(), factory);
    }

    // The gPAS de-pseudonymize path deliberately swallows backend failures and returns the
    // pseudonym unchanged. A caller-requested cancellation must not take that path, or a cancelled
    // $de-pseudonymize would quietly emit still-pseudonymized data for the rest of the resource.
    [Fact]
    public async Task GetOriginalValueFor_WhenTheCallerCancels_ThrowsInsteadOfReturningThePseudonym()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var gpasClient = CreateGPasClient(CreateThrowingHttpMessageHandler());

        var act = async () =>
            await gpasClient.GetOriginalValueFor("42", "domain", cancellationToken: cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    // The flip side: an HttpClient timeout also surfaces as an OperationCanceledException, but
    // with the caller's token unsignalled. That case keeps the pre-existing fallback.
    [Fact]
    public async Task GetOriginalValueFor_WhenTheBackendFailsWithoutCancellation_FallsBackToThePseudonym()
    {
        var gpasClient = CreateGPasClient(CreateThrowingHttpMessageHandler());

        var result = await gpasClient.GetOriginalValueFor(
            "42",
            "domain",
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.Should().Be("42");
    }

    [Theory]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    public async Task GetOrCreatePseudonymFor_WhenGPasIsTransientlyUnavailable_ThrowsTransientPseudonymizationException(
        HttpStatusCode statusCode
    )
    {
        var gpasClient = CreateGPasClient(CreateFailingHttpMessageHandler(statusCode));

        var act = async () =>
            await gpasClient.GetOrCreatePseudonymFor(
                "42",
                "domain",
                cancellationToken: TestContext.Current.CancellationToken
            );

        await act.Should().ThrowAsync<TransientPseudonymizationException>();
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.UnprocessableEntity)]
    public async Task GetOrCreatePseudonymFor_WhenGPasRejectsTheInput_ThrowsPseudonymizationRejectedException(
        HttpStatusCode statusCode
    )
    {
        var gpasClient = CreateGPasClient(CreateFailingHttpMessageHandler(statusCode));

        var act = async () =>
            await gpasClient.GetOrCreatePseudonymFor(
                "42",
                "domain",
                cancellationToken: TestContext.Current.CancellationToken
            );

        (
            await act.Should().ThrowAsync<PseudonymizationRejectedException>()
        ).WithInnerException<FhirOperationException>();
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
