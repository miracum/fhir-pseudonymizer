using System.Net;
using System.Net.Http.Headers;
using FakeItEasy;
using FhirPseudonymizer.Config;
using FhirPseudonymizer.Pseudonymization;
using FhirPseudonymizer.Pseudonymization.GPas;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace FhirPseudonymizer.Tests;

public class GPasFhirClientTests
{
    public static IEnumerable<object[]> GetOrCreatePseudonymFor_Data()
    {
        yield return new object[] { "1.10.1", HttpMethod.Get, "$pseudonymize-allow-create?domain=domain&original=42" };
        yield return new object[] { "1.10.2", HttpMethod.Post, "$pseudonymize-allow-create" };
        yield return new object[] { "1.10.3", HttpMethod.Post, "$pseudonymizeAllowCreate" };
    }

    public static IEnumerable<object[]> GetOriginalValueFor_Data()
    {
        yield return new object[] { "1.10.1", HttpMethod.Get, "$de-pseudonymize?domain=domain&pseudonym=42" };
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
    public async Task GetOrCreatePseudonymFor_ResolvesToApiVersionOperation(string gpasVersion,
        HttpMethod requestMethod, string requestUri)
    {
        // create gpas client
        var gpasClient = CreateGPasClient(gpasVersion);

        // act
        await gpasClient.GetOrCreatePseudonymFor("42", "domain");

        // verify
        VerifyRequest(requestMethod, requestUri);
    }

    private void VerifyRequest(HttpMethod requestMethod, string requestUri)
    {
        A.CallTo(messageHandler)
            .Where(_ => _.Method.Name == "SendAsync")
            .WhenArgumentsMatch((HttpRequestMessage r, CancellationToken _) =>
                r.Method == requestMethod &&
                r.RequestUri == new Uri(testBaseAddress.AbsoluteUri + requestUri))
            .MustHaveHappenedOnceExactly();
    }

    private static IHttpClientFactory CreateHttpClientFactory(HttpMessageHandler httpMessageHandler)
    {
        var client = new HttpClient(httpMessageHandler)
        {
            BaseAddress = testBaseAddress
        };

        var factory = A.Fake<IHttpClientFactory>();
        A.CallTo(() => factory.CreateClient(A<string>._)).Returns(client);
        return factory;
    }

    [Theory]
    [MemberData(nameof(GetOriginalValueFor_Data))]
    public async Task GetOriginalValueFor_ResolvesToApiVersionOperation(string gpasVersion, HttpMethod requestMethod,
        string requestUri)
    {
        // create gpas client
        var gpasClient = CreateGPasClient(gpasVersion);

        // act
        await gpasClient.GetOriginalValueFor("42", "domain");

        // verify request uri and method
        VerifyRequest(requestMethod, requestUri);
    }

    private IPseudonymServiceClient CreateGPasClient(string gPasVersion)
    {
        var config = new GPasConfig
        {
            Version = gPasVersion,
            Cache = new CacheConfig
            {
                AbsoluteExpirationMinutes = 1,
                SizeLimit = 1,
                SlidingExpirationMinutes = 1,
            }
        };

        var cache = new MemoryCache(
            new MemoryCacheOptions
            {
                SizeLimit = 1
            });

        return new GPasFhirClient(
            A.Fake<ILogger<GPasFhirClient>>(),
             clientFactory,
             config,
             cache,
             cache);
    }

    private static HttpMessageHandler CreateHttpMessageHandler()
    {
        var handler = A.Fake<HttpMessageHandler>();
        A.CallTo(handler)
            .Where(_ => _.Method.Name == "SendAsync")
            .WithReturnType<Task<HttpResponseMessage>>()
            .Returns(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                // these values have to be set since they are used by the FhirClient:
                // https://github.com/FirelyTeam/firely-net-common/blob/5899ce463f6cf166520cbbe6322310940942f81c/src/Hl7.Fhir.Support.Poco/Rest/HttpToEntryExtensions.cs#L28 &
                // https://github.com/FirelyTeam/firely-net-sdk/blob/f71543edc34c9edecf0f13af50d35e9e57ca353a/src/Hl7.Fhir.Core/Rest/TypedEntryResponseToBundle.cs#L24
                Content = new StringContent(ResponseContent, new MediaTypeHeaderValue("application/json+fhir")),
                RequestMessage = new HttpRequestMessage(HttpMethod.Post, testBaseAddress),
            });

        return handler;
    }
}
