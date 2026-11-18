using System.Buffers;
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

        // Values hashed here are identifiers and references - short enough that their UTF-8 form
        // comfortably fits a stack buffer. Anything larger falls back to a pooled array.
        private const int MaxStackAllocBytes = 256;

        public static string ComputeHmacSHA256Hash(string input, string hashKey)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return ComputeHmacSHA256Hash(input, GetHmacSha256KeyBytes(hashKey));
        }

        public static byte[] GetHmacSha256KeyBytes(string hashKey)
        {
            return Encoding.UTF8.GetBytes(hashKey);
        }

        public static string ComputeHmacSHA256Hash(string input, ReadOnlySpan<byte> hashKey)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            Span<byte> hashData = stackalloc byte[HMACSHA256.HashSizeInBytes];
            ComputeHmacSHA256Hash(input, hashKey, hashData);
            return Convert.ToHexStringLower(hashData);
        }

        /// <summary>
        ///     Writes the raw HMAC-SHA256 of <paramref name="input" /> into
        ///     <paramref name="destination" />, which must be at least
        ///     <see cref="HMACSHA256.HashSizeInBytes" /> long.
        /// </summary>
        public static void ComputeHmacSHA256Hash(
            string input,
            ReadOnlySpan<byte> hashKey,
            Span<byte> destination
        )
        {
            var maxByteCount = Encoding.UTF8.GetMaxByteCount(input.Length);
            byte[] rentedBuffer = null;
            var inputBytes =
                maxByteCount <= MaxStackAllocBytes
                    ? stackalloc byte[MaxStackAllocBytes]
                    : (rentedBuffer = ArrayPool<byte>.Shared.Rent(maxByteCount));

            try
            {
                var writtenBytes = Encoding.UTF8.GetBytes(input, inputBytes);

                HMACSHA256.HashData(hashKey, inputBytes[..writtenBytes], destination);
            }
            finally
            {
                if (rentedBuffer is not null)
                {
                    // Cleared on return: the buffer holds the plaintext of the value just hashed -
                    // a patient identifier or reference - and ArrayPool.Shared is process-wide, so
                    // an uncleared buffer would hand that plaintext to whatever rents it next.
                    ArrayPool<byte>.Shared.Return(rentedBuffer, clearArray: true);
                }
            }
        }

        /// <summary>
        ///     Derives the 32-byte key BLAKE3's keyed mode requires from an arbitrary-length
        ///     cryptoHashKey. Callers that hash many values with the same key (e.g.
        ///     CryptoHashProcessor) should call this once and reuse the result across
        ///     <see cref="ComputeKeyedBlake3Hash(string, ReadOnlySpan{byte})" /> calls, instead of re-deriving it per value.
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

            Span<byte> hashData = stackalloc byte[Blake3.Hash.Size];
            ComputeKeyedBlake3Hash(input, derivedKey, hashData);
            return Convert.ToHexStringLower(hashData);
        }

        /// <summary>
        ///     Writes the raw 32-byte keyed BLAKE3 hash of <paramref name="input" /> into
        ///     <paramref name="destination" />.
        /// </summary>
        public static void ComputeKeyedBlake3Hash(
            string input,
            ReadOnlySpan<byte> derivedKey,
            Span<byte> destination
        )
        {
            using var hasher = Hasher.NewKeyed(derivedKey);
            hasher.Update(Encoding.UTF8.GetBytes(input));
            hasher.Finalize(destination);
        }
    
        /// <summary>
        ///     Turns a hash of at least 16 bytes into an RFC 9562 version 8 (custom) UUID string,
        ///     like the name-based example in RFC 9562 Appendix B.2. Sets the version and variant
        ///     bits in place, so the first 16 bytes of <paramref name="hash" /> are modified.
        /// </summary>
        public static string ToUuid(Span<byte> hash)
        {
            hash[6] = (byte)((hash[6] & 0x0F) | 0x80); // version 8
            hash[8] = (byte)((hash[8] & 0x3F) | 0x80); // variant 10
            return new Guid(hash[..16], bigEndian: true).ToString();
        }
    }
}
