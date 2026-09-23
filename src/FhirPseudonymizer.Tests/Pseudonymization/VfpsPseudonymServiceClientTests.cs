using FhirPseudonymizer.Pseudonymization;
using FhirPseudonymizer.Pseudonymization.Vfps;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Vfps.Protos;

namespace FhirPseudonymizer.Tests.Pseudonymization;

public class VfpsPseudonymServiceClientTests
{
    [Theory]
    [InlineData(StatusCode.Unavailable)]
    [InlineData(StatusCode.Internal)]
    [InlineData(StatusCode.Unauthenticated)]
    [InlineData(StatusCode.PermissionDenied)]
    public async Task GetOrCreatePseudonymFor_WhenVfpsIsTransientlyUnavailable_ThrowsTransientPseudonymizationException(
        StatusCode statusCode
    )
    {
        // Arrange
        var client = A.Fake<PseudonymService.PseudonymServiceClient>();

        A.CallTo(() =>
                client.CreateAsync(
                    A<PseudonymServiceCreateRequest>._,
                    null,
                    null,
                    A<CancellationToken>._
                )
            )
            .Throws(() => throw new RpcException(new Status(statusCode, "backend unavailable")));

        var sut = new VfpsPseudonymServiceClient(
            A.Fake<ILogger<VfpsPseudonymServiceClient>>(),
            client
        );

        // Act
        var act = async () =>
            await sut.GetOrCreatePseudonymFor(
                "test",
                "namespace",
                cancellationToken: TestContext.Current.CancellationToken
            );

        // Assert
        (
            await act.Should().ThrowAsync<TransientPseudonymizationException>()
        ).WithInnerException<RpcException>();
    }

    [Fact]
    public async Task GetOrCreatePseudonymFor_WhenVfpsRejectsTheInput_ThrowsTheOriginalRpcException()
    {
        // Arrange
        var client = A.Fake<PseudonymService.PseudonymServiceClient>();

        A.CallTo(() =>
                client.CreateAsync(
                    A<PseudonymServiceCreateRequest>._,
                    null,
                    null,
                    A<CancellationToken>._
                )
            )
            .Throws(() =>
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, "doesn't match the required pattern")
                )
            );

        var sut = new VfpsPseudonymServiceClient(
            A.Fake<ILogger<VfpsPseudonymServiceClient>>(),
            client
        );

        // Act
        var act = async () =>
            await sut.GetOrCreatePseudonymFor(
                "test",
                "namespace",
                cancellationToken: TestContext.Current.CancellationToken
            );

        // Assert
        await act.Should().ThrowAsync<RpcException>();
    }

    [Fact]
    public async Task GetOrCreatePseudonymFor_WithGivenOriginalValue_ShouldReturnPseudonym()
    {
        // Arrange
        var client = A.Fake<PseudonymService.PseudonymServiceClient>();

        var fakeResponse = new PseudonymServiceCreateResponse
        {
            Pseudonym = new() { PseudonymValue = "not test" },
        };

        A.CallTo(() =>
                client.CreateAsync(
                    A<PseudonymServiceCreateRequest>._,
                    null,
                    null,
                    A<CancellationToken>._
                )
            )
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
        var result = await sut.GetOrCreatePseudonymFor(
            "test",
            "namespace",
            cancellationToken: TestContext.Current.CancellationToken
        );

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
            Pseudonym = new() { PseudonymValue = "not test", OriginalValue = "test" },
        };

        A.CallTo(() =>
                client.GetAsync(A<PseudonymServiceGetRequest>._, null, null, A<CancellationToken>._)
            )
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
        var result = await sut.GetOriginalValueFor(
            "not test",
            "namespace",
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Assert
        result.Should().Be("test");
    }

    [Fact]
    public async Task GetOriginalValueFor_WithNonExistingPseudonym_ShouldReturnPseudonymValueInsteadOfOriginal()
    {
        // Arrange
        var client = A.Fake<PseudonymService.PseudonymServiceClient>();

        A.CallTo(() =>
                client.GetAsync(A<PseudonymServiceGetRequest>._, null, null, A<CancellationToken>._)
            )
            .Throws(() => throw new RpcException(new Status(StatusCode.NotFound, "doesn't exist")));

        var sut = new VfpsPseudonymServiceClient(
            A.Fake<ILogger<VfpsPseudonymServiceClient>>(),
            client
        );

        // Act
        var result = await sut.GetOriginalValueFor(
            "test",
            "namespace",
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Assert
        result.Should().Be("test");
    }

    [Fact]
    public async Task GetOriginalValueFor_WhenTheCallerCancels_ThrowsInsteadOfReturningThePseudonym()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var client = A.Fake<PseudonymService.PseudonymServiceClient>();
        A.CallTo(() =>
                client.GetAsync(A<PseudonymServiceGetRequest>._, null, null, A<CancellationToken>._)
            )
            .Throws(() => new OperationCanceledException(cts.Token));

        var sut = new VfpsPseudonymServiceClient(
            A.Fake<ILogger<VfpsPseudonymServiceClient>>(),
            client
        );

        // Act
        var act = async () =>
            await sut.GetOriginalValueFor("test", "namespace", cancellationToken: cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
