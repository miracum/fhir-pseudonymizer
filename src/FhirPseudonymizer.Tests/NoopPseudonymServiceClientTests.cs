using FhirPseudonymizer.Pseudonymization;
using FluentAssertions;

namespace FhirPseudonymizer.Tests;

public class NoopPseudonymServiceClientTests
{
    [Fact]
    public void GetOrCreatePseudonymFor_WithAnyOriginalValue_ShouldAlwaysThrow()
    {
        var client = new NoopPseudonymServiceClient();

        Action act = () => client.GetOrCreatePseudonymFor("something", "somewhere");

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void GetOriginalValueFor_WithAnyPseudonym_ShouldAlwaysThrow()
    {
        var client = new NoopPseudonymServiceClient();

        Action act = () => client.GetOriginalValueFor("something", "somewhere");

        act.Should().Throw<InvalidOperationException>();
    }
}
