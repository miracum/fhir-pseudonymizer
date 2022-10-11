using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FhirPseudonymizer.Pseudonymization.Vfps;
using FluentAssertions;
using Grpc.Core;
using Vfps.Protos;
using Xunit;

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
            Pseudonym = new()
            {
                PseudonymValue = "not test",
            },
        };

        A.CallTo(() => client.CreateAsync(A<PseudonymServiceCreateRequest>._, null, null, default))
            .Returns(new AsyncUnaryCall<PseudonymServiceCreateResponse>(Task.FromResult(fakeResponse), null, null, null, null));

        var sut = new VfpsPseudonymServiceClient(client);

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
            Pseudonym = new()
            {
                PseudonymValue = "not test",
                OriginalValue = "test",
            },
        };

        A.CallTo(() => client.GetAsync(A<PseudonymServiceGetRequest>._, null, null, default))
            .Returns(new AsyncUnaryCall<PseudonymServiceGetResponse>(Task.FromResult(fakeResponse), null, null, null, null));

        var sut = new VfpsPseudonymServiceClient(client);

        // Act
        var result = await sut.GetOriginalValueFor("not test", "namespace");

        // Assert
        result.Should().Be("test");
    }
}
