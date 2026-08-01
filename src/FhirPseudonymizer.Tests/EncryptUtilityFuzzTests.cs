using System.Security.Cryptography;
using System.Text;
using FsCheck;
using FsCheck.Xunit;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Utility;

/// <summary>
///     Property-based round-trip tests for <see cref="EncryptUtility" />, the pure AES
///     encrypt/decrypt pair backing the anonymizer's <c>encrypt</c> de-identification method (and
///     the corresponding <c>$de-pseudonymize</c> decryption path). There were no tests at all for
///     this utility before. A round trip is the classic property to fuzz for an encrypt/decrypt
///     pair: for arbitrary plaintext and any valid-length key, decrypting what was just encrypted
///     must always reproduce the original text exactly - unlike a handful of hand-picked examples,
///     this throws whatever plaintext (empty, unicode, control characters, ...) FsCheck comes up
///     with at it.
/// </summary>
public class EncryptUtilityFuzzTests
{
    // AES accepts 128/192/256 bit keys; hashing an arbitrary seed to 32 bytes with SHA-256 is a
    // convenient way to turn any FsCheck-generated string into a valid AES-256 key.
    private static byte[] DeriveKey(string seed) =>
        SHA256.HashData(Encoding.UTF8.GetBytes(seed ?? string.Empty));

    [Property]
    public bool Base64RoundTrip_ReturnsTheOriginalPlaintext(string plaintext, string keySeed)
    {
        var key = DeriveKey(keySeed);

        var encrypted = EncryptUtility.EncryptTextToBase64WithAes(plaintext, key);
        var decrypted = EncryptUtility.DecryptTextFromBase64WithAes(encrypted, key);

        return decrypted == plaintext;
    }

    [Property]
    public bool HexRoundTrip_ReturnsTheOriginalPlaintext(string plaintext, string keySeed)
    {
        var key = DeriveKey(keySeed);

        var encrypted = EncryptUtility.EncryptTextToHexWithAes(plaintext, key);
        var decrypted = EncryptUtility.DecryptTextFromHexStringWithAes(encrypted, key);

        return decrypted == plaintext;
    }

    [Property]
    public bool Base64AndHexEncryptionOfTheSamePlaintext_DecryptToTheSameValue(
        NonEmptyString plaintext,
        string keySeed
    )
    {
        var key = DeriveKey(keySeed);

        var viaBase64 = EncryptUtility.DecryptTextFromBase64WithAes(
            EncryptUtility.EncryptTextToBase64WithAes(plaintext.Get, key),
            key
        );
        var viaHex = EncryptUtility.DecryptTextFromHexStringWithAes(
            EncryptUtility.EncryptTextToHexWithAes(plaintext.Get, key),
            key
        );

        return viaBase64 == viaHex && viaBase64 == plaintext.Get;
    }
}
