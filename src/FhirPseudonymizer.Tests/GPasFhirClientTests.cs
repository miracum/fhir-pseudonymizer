using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
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



    private readonly string responseContent =
        "{\"resourceType\":\"Parameters\",\"parameter\":[{\"name\":\"pseudonym\",\"part\":[{\"name\":\"original\",\"valueIdentifier\":{\"system\":\"https://ths-greifswald.de/gpas\",\"value\":\"1\"}},{\"name\":\"target\",\"valueIdentifier\":{\"system\":\"https://ths-greifswald.de/gpas\",\"value\":\"PATIENT\"}},{\"name\":\"pseudonym\",\"valueIdentifier\":{\"system\":\"https://ths-greifswald.de/gpas\",\"value\":\"751770313\"}}]},{\"name\":\"42\",\"valueString\":\"24\"}]}";

    [Theory]
    [MemberData(nameof(GetOrCreatePseudonymFor_Data))]
    public async Task GetOrCreatePseudonymFor_ResolvesToApiVersionOperation(string gpasVersion, HttpMethod requestMethod, string requestUri)
    {
        // arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {

                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent),
            });

        var baseAddress = new Uri("http://gpas");
        var client = new HttpClient(handlerMock.Object)
        {
            BaseAddress = baseAddress
        };

        var mockFactory = new Mock<IHttpClientFactory>();
        mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

        // configuration
        var inMemorySettings = new Dictionary<string, string> {
            {"Cache:SizeLimit", "1"},
            {"gPAS:Version", gpasVersion}
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var gpasClient = new GPasFhirClient(new Mock<ILogger<GPasFhirClient>>().Object, mockFactory.Object,
            config);

        // act
        await gpasClient.GetOrCreatePseudonymFor("42", "domain");


        // verify
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == requestMethod
                    && req.RequestUri == new Uri(baseAddress.AbsoluteUri + requestUri)
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Theory]
    [MemberData(nameof(GetOriginalValueFor_Data))]
    public async Task GetOriginalValueFor_ResolvesToApiVersionOperation(string gpasVersion, HttpMethod requestMethod, string requestUri)
    {
        // arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {

                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent),
            });

        var baseAddress = new Uri("http://gpas");
        var client = new HttpClient(handlerMock.Object)
        {
            BaseAddress = baseAddress
        };

        var mockFactory = new Mock<IHttpClientFactory>();
        mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

        // configuration
        var inMemorySettings = new Dictionary<string, string> {
            {"Cache:SizeLimit", "1"},
            {"gPAS:Version", gpasVersion}
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var gpasClient = new GPasFhirClient(new Mock<ILogger<GPasFhirClient>>().Object, mockFactory.Object,
            config);

        // act
        await gpasClient.GetOriginalValueFor("42", "domain");


        // verify
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == requestMethod
                && req.RequestUri == new Uri(baseAddress.AbsoluteUri + requestUri)
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }
}
