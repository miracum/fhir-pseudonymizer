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
    [InlineData(StatusCode.DeadlineExceeded)]
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

    [Theory]
    [InlineData(StatusCode.InvalidArgument)]
    [InlineData(StatusCode.FailedPrecondition)]
    [InlineData(StatusCode.NotFound)]
    [InlineData(StatusCode.AlreadyExists)]
    [InlineData(StatusCode.OutOfRange)]
    public async Task GetOrCreatePseudonymFor_WhenVfpsRejectsTheInput_ThrowsPseudonymizationRejectedExceptionWithTheDetail(
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
            .Throws(() =>
                throw new RpcException(new Status(statusCode, "doesn't match the required pattern"))
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
        (await act.Should().ThrowAsync<PseudonymizationRejectedException>())
            .WithMessage("*doesn't match the required pattern*")
            .WithInnerException<RpcException>();
    }

    [Theory]
    [InlineData(StatusCode.Unknown)]
    public async Task GetOrCreatePseudonymFor_WhenVfpsFailsOtherwise_ThrowsTheOriginalRpcException(
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
            .Throws(() => throw new RpcException(new Status(statusCode, "something went wrong")));

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
        (await act.Should().ThrowExactlyAsync<RpcException>())
            .Which.StatusCode.Should()
            .Be(statusCode);
    }

    [Fact]
    public async Task GetOrCreatePseudonymFor_WhenTheDeadlineElapses_ThrowsTransientPseudonymizationException()
    {
        // With ThrowOperationCanceledOnCancellation, an exceeded deadline surfaces as an
        // OperationCanceledException although the caller's token was never cancelled.
        var client = A.Fake<PseudonymService.PseudonymServiceClient>();

        A.CallTo(() =>
                client.CreateAsync(
                    A<PseudonymServiceCreateRequest>._,
                    null,
                    null,
                    A<CancellationToken>._
                )
            )
            .Throws(() => new OperationCanceledException("deadline exceeded"));

        var sut = new VfpsPseudonymServiceClient(
            A.Fake<ILogger<VfpsPseudonymServiceClient>>(),
            client
        );

        var act = async () =>
            await sut.GetOrCreatePseudonymFor(
                "test",
                "namespace",
                cancellationToken: TestContext.Current.CancellationToken
            );

        (
            await act.Should().ThrowAsync<TransientPseudonymizationException>()
        ).WithInnerException<OperationCanceledException>();
    }

    [Fact]
    public async Task GetOrCreatePseudonymFor_WhenCancelledByTheCaller_PropagatesTheCancellation()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var client = A.Fake<PseudonymService.PseudonymServiceClient>();

        A.CallTo(() =>
                client.CreateAsync(
                    A<PseudonymServiceCreateRequest>._,
                    null,
                    null,
                    A<CancellationToken>._
                )
            )
            .Throws(() => new OperationCanceledException(cts.Token));

        var sut = new VfpsPseudonymServiceClient(
            A.Fake<ILogger<VfpsPseudonymServiceClient>>(),
            client
        );

        var act = async () =>
            await sut.GetOrCreatePseudonymFor("test", "namespace", cancellationToken: cts.Token);

        await act.Should().ThrowExactlyAsync<OperationCanceledException>();
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
