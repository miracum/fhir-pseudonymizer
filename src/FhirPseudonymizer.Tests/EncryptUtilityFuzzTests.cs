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

    // Produced independently with
    //   printf '%s' 'Max Mustermann – Ärztin 🩺' | openssl enc -aes-256-cbc -K <KnownKey> -iv <iv>
    // and the IV prepended, i.e. the wire format: IV || AES-256-CBC(PKCS#7, UTF-8 plaintext).
    private const string KnownPlaintext = "Max Mustermann – Ärztin 🩺";

    private const string KnownCiphertextHex =
        "A0A1A2A3A4A5A6A7A8A9AAABACADAEAF"
        + "BD5F1D55C21C4E55B4E503A7C053233F6DE626B393C740A91039AF1FA7F6D5A4";

    private static readonly byte[] KnownKey = Convert.FromHexString(
        "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F"
    );

    [Fact]
    public void DecryptTextFromHexStringWithAes_WithAnOpenSslProducedCiphertext_ReturnsThePlaintext()
    {
        EncryptUtility
            .DecryptTextFromHexStringWithAes(KnownCiphertextHex, KnownKey)
            .Should()
            .Be(KnownPlaintext);
    }

    [Fact]
    public void EncryptTextToHexWithAes_ProducesARandomIvFollowedByTheCbcCiphertext()
    {
        var encrypted = Convert.FromHexString(
            EncryptUtility.EncryptTextToHexWithAes(KnownPlaintext, KnownKey)
        );

        // re-encrypting with the IV that was picked must reproduce the ciphertext exactly
        using var aes = Aes.Create();
        aes.Key = KnownKey;
        var expected = aes.EncryptCbc(Encoding.UTF8.GetBytes(KnownPlaintext), encrypted[..16]);

        encrypted[16..].Should().Equal(expected);
        EncryptUtility
            .EncryptTextToHexWithAes(KnownPlaintext, KnownKey)
            .Should()
            .NotBe(Convert.ToHexString(encrypted), "every encryption must use a fresh IV");
    }

    // Values encrypted before the switch to the one-shot APIs must still decrypt, and the other
    // way round, so the stream-based implementation it replaced is kept here as the reference.
    [Property]
    public bool CiphertextFromThePreviousImplementation_DecryptsToTheOriginalPlaintext(
        NonEmptyString plaintext,
        string keySeed
    )
    {
        var key = DeriveKey(keySeed);

        return EncryptUtility.DecryptTextFromHexStringWithAes(
                LegacyEncryptToHex(plaintext.Get, key),
                key
            ) == plaintext.Get;
    }

    [Property]
    public bool Ciphertext_DecryptsWithThePreviousImplementation(
        NonEmptyString plaintext,
        string keySeed
    )
    {
        // The previous implementation decrypted through a BOM-sniffing StreamReader, which
        // dropped a leading U+FEFF - a quirk of that reader, not of the format.
        if (plaintext.Get[0] == '\uFEFF')
        {
            return true;
        }

        var key = DeriveKey(keySeed);

        return LegacyDecryptFromHex(EncryptUtility.EncryptTextToHexWithAes(plaintext.Get, key), key)
            == plaintext.Get;
    }

    private static string LegacyEncryptToHex(string plainText, byte[] key)
    {
        using var aes = Aes.Create();
        var iv = aes.IV;
        aes.Key = key;
        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        return Convert.ToHexString([.. iv, .. msEncrypt.ToArray()]);
    }

    private static string LegacyDecryptFromHex(string hexText, byte[] key)
    {
        var byteData = Convert.FromHexString(hexText);

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = byteData[..16];
        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var msDecrypt = new MemoryStream(byteData[16..]);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        return srDecrypt.ReadToEnd();
    }
}
