using FhirPseudonymizer.Config;

namespace FhirPseudonymizer.Tests.Config;

public class CryptoHashKeyDerivationTests
{
    [Fact]
    public void Derive_WithSameMasterKeyAndContext_ShouldReturnSameKey()
    {
        var first = CryptoHashKeyDerivation.Derive("master-key", "project-a");
        var second = CryptoHashKeyDerivation.Derive("master-key", "project-a");

        first.Should().Be(second);
    }

    [Fact]
    public void Derive_WithDifferentContext_ShouldReturnDifferentKey()
    {
        var projectA = CryptoHashKeyDerivation.Derive("master-key", "project-a");
        var projectB = CryptoHashKeyDerivation.Derive("master-key", "project-b");

        projectA.Should().NotBe(projectB);
    }

    [Fact]
    public void Derive_WithDifferentMasterKey_ShouldReturnDifferentKey()
    {
        var first = CryptoHashKeyDerivation.Derive("master-key-1", "project-a");
        var second = CryptoHashKeyDerivation.Derive("master-key-2", "project-a");

        first.Should().NotBe(second);
    }

    [Fact]
    public void Derive_ShouldReturnA32ByteKeyAsLowercaseHex()
    {
        var derived = CryptoHashKeyDerivation.Derive("master-key", "project-a");

        derived.Should().MatchRegex("^[0-9a-f]{64}$");
    }
}
