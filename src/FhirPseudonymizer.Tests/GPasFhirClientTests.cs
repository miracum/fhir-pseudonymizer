using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq.Protected;
using Task = System.Threading.Tasks.Task;

namespace FhirPseudonymizer.Tests;

public class GPasFhirClientTests
{
    public static IEnumerable<object[]> GetProcessData()
    {
        yield return new object[] { "1.10.1", HttpMethod.Get, "$pseudonymize-allow-create?domain=domain&original=42" };
        yield return new object[] { "1.10.2", HttpMethod.Post, "$pseudonymize-allow-create" };
        yield return new object[] { "1.10.3", HttpMethod.Post, "$pseudonymizeAllowCreate" };
    }

    private readonly string responseContent =
        "{\"resourceType\":\"Parameters\",\"parameter\":[{\"name\":\"pseudonym\",\"part\":[{\"name\":\"original\",\"valueIdentifier\":{\"system\":\"https://ths-greifswald.de/gpas\",\"value\":\"1\"}},{\"name\":\"target\",\"valueIdentifier\":{\"system\":\"https://ths-greifswald.de/gpas\",\"value\":\"PATIENT\"}},{\"name\":\"pseudonym\",\"valueIdentifier\":{\"system\":\"https://ths-greifswald.de/gpas\",\"value\":\"751770313\"}}]},{\"name\":\"42\",\"valueString\":\"24\"}]}";

    [Theory]
    [MemberData(nameof(GetProcessData))]
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
}
