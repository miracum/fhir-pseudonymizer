using FhirPseudonymizer.Pseudonymization;

namespace FhirPseudonymizer.Tests.Pseudonymization;

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
