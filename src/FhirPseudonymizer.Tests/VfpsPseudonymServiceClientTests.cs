using FhirPseudonymizer.Pseudonymization.Vfps;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Vfps.Protos;

namespace FhirPseudonymizer.Tests;

public class VfpsPseudonymServiceClientTests
{
    [Fact]
    public async Task GetOrCreatePseudonymFor_WithGivenOriginalValue_ShouldReturnPseudonym()
    {
        // Arrange
        var client = A.Fake<PseudonymService.PseudonymServiceClient>();

        var fakeResponse = new PseudonymServiceCreateResponse
        {
            Pseudonym = new() { PseudonymValue = "not test", },
        };

        A.CallTo(() => client.CreateAsync(A<PseudonymServiceCreateRequest>._, null, null, default))
            .Returns(
                new AsyncUnaryCall<PseudonymServiceCreateResponse>(
                    Task.FromResult(fakeResponse),
                    null,
                    null,
                    null,
                    null
                )
            );

        var sut = new VfpsPseudonymServiceClient(
            A.Fake<ILogger<VfpsPseudonymServiceClient>>(),
            client
        );

        // Act
        var result = await sut.GetOrCreatePseudonymFor("test", "namespace");

        // Assert
        result.Should().Be("not test");
    }

    [Fact]
    public async Task GetOriginalValueFor_WithGivenPseudonym_ShouldReturnOriginalValue()
    {
        // Arrange
        var client = A.Fake<PseudonymService.PseudonymServiceClient>();

        var fakeResponse = new PseudonymServiceGetResponse
        {
            Pseudonym = new() { PseudonymValue = "not test", OriginalValue = "test", },
        };

        A.CallTo(() => client.GetAsync(A<PseudonymServiceGetRequest>._, null, null, default))
            .Returns(
                new AsyncUnaryCall<PseudonymServiceGetResponse>(
                    Task.FromResult(fakeResponse),
                    null,
                    null,
                    null,
                    null
                )
            );

        var sut = new VfpsPseudonymServiceClient(
            A.Fake<ILogger<VfpsPseudonymServiceClient>>(),
            client
        );

        // Act
        var result = await sut.GetOriginalValueFor("not test", "namespace");

        // Assert
        result.Should().Be("test");
    }

    [Fact]
    public async Task GetOriginalValueFor_WithNonExistingPseudonym_ShouldReturnPseudonymValueInsteadOfOriginal()
    {
        // Arrange
        var client = A.Fake<PseudonymService.PseudonymServiceClient>();

        A.CallTo(() => client.GetAsync(A<PseudonymServiceGetRequest>._, null, null, default))
            .Throws(() => throw new RpcException(new Status(StatusCode.NotFound, "doesn't exist")));

        var sut = new VfpsPseudonymServiceClient(
            A.Fake<ILogger<VfpsPseudonymServiceClient>>(),
            client
        );

        // Act
        var result = await sut.GetOriginalValueFor("test", "namespace");

        // Assert
        result.Should().Be("test");
    }
}
