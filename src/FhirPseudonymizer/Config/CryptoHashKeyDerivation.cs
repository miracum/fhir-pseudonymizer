using System.Security.Cryptography;
using System.Text;

namespace FhirPseudonymizer.Config;

public static class CryptoHashKeyDerivation
{
    private const int DerivedKeyLength = 32;

    public static string Derive(string masterKey, string context)
    {
        var derivedKey = HKDF.DeriveKey(
            HashAlgorithmName.SHA256,
            ikm: Encoding.UTF8.GetBytes(masterKey),
            outputLength: DerivedKeyLength,
            info: Encoding.UTF8.GetBytes(context)
        );

        return Convert.ToHexStringLower(derivedKey);
    }
}
