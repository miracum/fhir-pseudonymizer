using FhirPseudonymizer.Config;

namespace FhirPseudonymizer.Tests.Config;

public class KeyDerivationTests
{
    [Fact]
    public void DeriveCryptoHashKey_WithSameMasterKeyAndContext_ShouldReturnSameKey()
    {
        var first = KeyDerivation.DeriveCryptoHashKey("master-key", "project-a");
        var second = KeyDerivation.DeriveCryptoHashKey("master-key", "project-a");

        first.Should().Be(second);
    }

    [Fact]
    public void DeriveCryptoHashKey_WithDifferentContext_ShouldReturnDifferentKey()
    {
        var projectA = KeyDerivation.DeriveCryptoHashKey("master-key", "project-a");
        var projectB = KeyDerivation.DeriveCryptoHashKey("master-key", "project-b");

        projectA.Should().NotBe(projectB);
    }

    [Fact]
    public void DeriveCryptoHashKey_WithDifferentMasterKey_ShouldReturnDifferentKey()
    {
        var first = KeyDerivation.DeriveCryptoHashKey("master-key-1", "project-a");
        var second = KeyDerivation.DeriveCryptoHashKey("master-key-2", "project-a");

        first.Should().NotBe(second);
    }

    [Fact]
    public void DeriveCryptoHashKey_ShouldReturnA32ByteKeyAsLowercaseHex()
    {
        var derived = KeyDerivation.DeriveCryptoHashKey("master-key", "project-a");

        derived.Should().MatchRegex("^[0-9a-f]{64}$");
    }

    [Fact]
    public void DeriveEncryptKey_WithSameMasterKeyAndContext_ShouldReturnSameKey()
    {
        var first = KeyDerivation.DeriveEncryptKey("master-key", "project-a");
        var second = KeyDerivation.DeriveEncryptKey("master-key", "project-a");

        first.Should().Equal(second);
    }

    [Fact]
    public void DeriveEncryptKey_WithDifferentContext_ShouldReturnDifferentKey()
    {
        var projectA = KeyDerivation.DeriveEncryptKey("master-key", "project-a");
        var projectB = KeyDerivation.DeriveEncryptKey("master-key", "project-b");

        projectA.Should().NotEqual(projectB);
    }

    [Fact]
    public void DeriveEncryptKey_ShouldReturnA32ByteKey()
    {
        var derived = KeyDerivation.DeriveEncryptKey("master-key", "project-a");

        derived.Should().HaveCount(32);
    }

    [Fact]
    public void DeriveCryptoHashKeyAndDeriveEncryptKey_WithSameMasterKeyAndContext_ShouldReturnIndependentKeys()
    {
        var cryptoHashKey = KeyDerivation.DeriveCryptoHashKey("master-key", "project-a");
        var encryptKey = KeyDerivation.DeriveEncryptKey("master-key", "project-a");

        Convert.ToHexStringLower(encryptKey).Should().NotBe(cryptoHashKey);
    }
}
