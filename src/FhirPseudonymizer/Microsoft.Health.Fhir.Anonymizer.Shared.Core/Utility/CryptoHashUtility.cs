using System.Security.Cryptography;
using System.Text;
using Blake3;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Utility
{
    public class CryptoHashUtility
    {
        // Fixed, non-secret context for compressing an arbitrary-length cryptoHashKey down to
        // the 32 bytes BLAKE3's keyed mode requires. Binds the derived key to this specific use,
        // so it can never collide with BLAKE3 used for any other purpose in this application.
        private const string Blake3KeyDerivationContext = "FhirPseudonymizer.CryptoHash.Blake3.v1";

        public static string ComputeHmacSHA256Hash(string input, string hashKey)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var key = Encoding.UTF8.GetBytes(hashKey);
            using var hmac = new HMACSHA256(key);
            var plainData = Encoding.UTF8.GetBytes(input);
            var hashData = hmac.ComputeHash(plainData);

            return string.Concat(hashData.Select(b => b.ToString("x2")));
        }

        /// <summary>
        ///     Derives the 32-byte key BLAKE3's keyed mode requires from an arbitrary-length
        ///     cryptoHashKey. Callers that hash many values with the same key (e.g.
        ///     CryptoHashProcessor) should call this once and reuse the result across
        ///     <see cref="ComputeKeyedBlake3Hash" /> calls, instead of re-deriving it per value.
        /// </summary>
        public static byte[] DeriveBlake3Key(string hashKey)
        {
            // BLAKE3's keyed mode requires an exact 32-byte key. Its own key-derivation mode is
            // the correct tool for compressing the (arbitrary-length) configured key down to
            // that size - any existing cryptoHashKey value keeps working unchanged.
            using var keyDeriver = Hasher.NewDeriveKey(Blake3KeyDerivationContext);
            keyDeriver.Update(Encoding.UTF8.GetBytes(hashKey));
            return keyDeriver.Finalize().AsSpan().ToArray();
        }

        public static string ComputeKeyedBlake3Hash(string input, ReadOnlySpan<byte> derivedKey)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            using var hasher = Hasher.NewKeyed(derivedKey);
            hasher.Update(Encoding.UTF8.GetBytes(input));
            var hashData = hasher.Finalize().AsSpan();

            return Convert.ToHexStringLower(hashData);
        }
    }
}
