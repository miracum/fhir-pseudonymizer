using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Configuration;
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

    private static readonly Uri testBaseAddress = new Uri("http://gpas");


    private const string responseContent = @"{
        ""resourceType"": ""Parameters"",
        ""parameter"": [
            {
                ""name"": ""pseudonym"",
                ""part"": [
                    {
                        ""name"": ""pseudonym"",
                        ""valueIdentifier"": {
                            ""system"": ""https://ths-greifswald.de/gpas"",
                            ""value"": ""24""
                        }
                    }
                ]
            },
            {
                ""name"": ""42"",
                ""valueString"": ""24""
            }
        ]
    }";


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
        A.CallTo(messageHandler).Where(_ => _.Method.Name == "SendAsync")
            .WhenArgumentsMatch(((HttpRequestMessage r, CancellationToken _) =>
                r.Method == requestMethod &&
                r.RequestUri == new Uri(testBaseAddress.AbsoluteUri + requestUri)))
            .MustHaveHappenedOnceExactly();
    }

    private IHttpClientFactory CreateHttpClientFactory(HttpMessageHandler httpMessageHandler)
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

    private GPasFhirClient CreateGPasClient(string gPasVersion)
    {
        return new GPasFhirClient(A.Fake<ILogger<GPasFhirClient>>(), clientFactory,
            CreateConfiguration(gPasVersion));
    }

    private HttpMessageHandler CreateHttpMessageHandler()
    {
        var handler = A.Fake<HttpMessageHandler>();
        A.CallTo(handler).Where(_ => _.Method.Name == "SendAsync").WithReturnType<Task<HttpResponseMessage>>()
            .Returns(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent),
            });

        return handler;
    }

    private IConfiguration CreateConfiguration(string gpasVersion)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> {
            {"Cache:SizeLimit", "1"},
            {"gPAS:Version", gpasVersion}
        })
            .Build();
    }
}
