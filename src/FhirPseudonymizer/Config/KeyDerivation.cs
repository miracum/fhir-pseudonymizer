using System.Security.Cryptography;
using System.Text;

namespace FhirPseudonymizer.Config;

/// <summary>
///     Derives independent, non-reversible keys for the crypto hash and encrypt anonymization
///     methods from a single shared master key, using HKDF (RFC 5869). A distinct "purpose"
///     label is folded into the HKDF "info" parameter per key type, so the two derived keys stay
///     cryptographically independent even when derived from the same (master key, context) pair.
/// </summary>
public static class KeyDerivation
{
    private const int CryptoHashKeyLength = 32; // HMAC-SHA256
    private const int EncryptKeyLength = 32; // AES-256

    // Fixed, non-secret salt for every derivation. Binds derived keys to this specific
    // application, so the same (master key, context) pair fed into some other HKDF-based system
    // wouldn't happen to produce the same output. Changing this value changes every derived key.
    private static readonly byte[] Salt = Encoding.UTF8.GetBytes(
        "FhirPseudonymizer.KeyDerivation.v1"
    );

    public static string DeriveCryptoHashKey(string masterKey, string context)
    {
        var derivedKey = DeriveBytes(masterKey, context, "cryptoHash", CryptoHashKeyLength);
        return Convert.ToHexStringLower(derivedKey);
    }

    public static byte[] DeriveEncryptKey(string masterKey, string context)
    {
        return DeriveBytes(masterKey, context, "encrypt", EncryptKeyLength);
    }

    private static byte[] DeriveBytes(
        string masterKey,
        string context,
        string purpose,
        int outputLength
    )
    {
        return HKDF.DeriveKey(
            HashAlgorithmName.SHA256,
            ikm: Encoding.UTF8.GetBytes(masterKey),
            outputLength: outputLength,
            salt: Salt,
            info: Encoding.UTF8.GetBytes($"{context}:{purpose}")
        );
    }
}
