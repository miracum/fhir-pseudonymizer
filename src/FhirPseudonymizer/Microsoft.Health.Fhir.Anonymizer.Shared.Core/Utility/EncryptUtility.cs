using System.Buffers;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Utility
{
    /// <summary>
    ///     AES-CBC with PKCS#7 padding and a random IV, which is prepended to the ciphertext; the
    ///     plaintext is UTF-8. Uses the one-shot <see cref="SymmetricAlgorithm.EncryptCbc(ReadOnlySpan{byte}, ReadOnlySpan{byte}, Span{byte}, PaddingMode)" />
    ///     APIs rather than a CryptoStream/StreamWriter pipeline, which produced the same bytes but
    ///     allocated several streams, buffers and intermediate copies per value.
    /// </summary>
    public static class EncryptUtility
    {
        // AES Initialization Vector length is 16 bytes
        private const int AesIvSize = 16;

        private const int MaxStackAllocBytes = 256;

        public static string EncryptTextToBase64WithAes(string plainText, byte[] key)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            return Convert.ToBase64String(Encrypt(plainText, key));
        }

        public static string DecryptTextFromBase64WithAes(string base64Text, byte[] key)
        {
            if (string.IsNullOrEmpty(base64Text))
            {
                return base64Text;
            }

            return Decrypt(Convert.FromBase64String(base64Text), key);
        }

        public static string EncryptTextToHexWithAes(string plainText, byte[] key)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            return Convert.ToHexString(Encrypt(plainText, key));
        }

        public static string DecryptTextFromHexStringWithAes(string hexText, byte[] key)
        {
            if (string.IsNullOrEmpty(hexText))
            {
                return hexText;
            }

            return Decrypt(Convert.FromHexString(hexText), key);
        }

        // Returns the IV followed by the ciphertext.
        private static byte[] Encrypt(string plainText, byte[] key)
        {
            var maxByteCount = Encoding.UTF8.GetMaxByteCount(plainText.Length);
            byte[] rentedBuffer = null;
            var plainBytes =
                maxByteCount <= MaxStackAllocBytes
                    ? stackalloc byte[MaxStackAllocBytes]
                    : (rentedBuffer = ArrayPool<byte>.Shared.Rent(maxByteCount));

            try
            {
                var plainByteCount = Encoding.UTF8.GetBytes(plainText, plainBytes);

                using var aes = Aes.Create();
                aes.Key = key;

                var result = new byte[
                    AesIvSize + aes.GetCiphertextLengthCbc(plainByteCount, PaddingMode.PKCS7)
                ];
                var iv = result.AsSpan(0, AesIvSize);
                RandomNumberGenerator.Fill(iv);

                aes.EncryptCbc(
                    plainBytes[..plainByteCount],
                    iv,
                    result.AsSpan(AesIvSize),
                    PaddingMode.PKCS7
                );

                return result;
            }
            finally
            {
                // the plaintext is the very value being de-identified: don't leave it behind
                // in a shared pool buffer
                if (rentedBuffer is not null)
                {
                    ArrayPool<byte>.Shared.Return(rentedBuffer, clearArray: true);
                }
                else
                {
                    CryptographicOperations.ZeroMemory(plainBytes);
                }
            }
        }

        private static string Decrypt(byte[] ivAndCipherText, byte[] key)
        {
            if (ivAndCipherText.Length < AesIvSize)
            {
                throw new FormatException(
                    $"The input text for decryption should not be less than {AesIvSize} bytes length!"
                );
            }

            using var aes = Aes.Create();
            aes.Key = key;

            var span = ivAndCipherText.AsSpan();
            var plainBytes = aes.DecryptCbc(
                span[AesIvSize..],
                span[..AesIvSize],
                PaddingMode.PKCS7
            );

            try
            {
                return Encoding.UTF8.GetString(plainBytes);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(plainBytes);
            }
        }
    }
}
