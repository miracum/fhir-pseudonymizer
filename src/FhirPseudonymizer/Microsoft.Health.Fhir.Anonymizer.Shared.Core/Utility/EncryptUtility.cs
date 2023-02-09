using System;
using System.IO;
using System.Security.Cryptography;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Utility
{
    public static class EncryptUtility
    {
        // AES Initialization Vector length is 16 bytes
        private const int AesIvSize = 16;

        public static string EncryptTextToBase64WithAes(string plainText, byte[] key)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            /* Create AES encryptor:
             * Mode: CBC
             * Block size: 16 bytes
             * Acceptable key sizes： [128, 192, 256]
             */
            using var aes = Aes.Create();
            var iv = aes.IV;
            aes.Key = key;
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            // Get encrypted bytes
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            var encryptedBytes = msEncrypt.ToArray();

            // Concat IV to encrpted bytes
            var result = new byte[AesIvSize + encryptedBytes.Length];
            Buffer.BlockCopy(iv, 0, result, 0, AesIvSize);
            Buffer.BlockCopy(encryptedBytes, 0, result, AesIvSize, encryptedBytes.Length);

            return Convert.ToBase64String(result);
        }

        public static string DecryptTextFromBase64WithAes(string base64Text, byte[] key)
        {
            if (string.IsNullOrEmpty(base64Text))
            {
                return base64Text;
            }

            // Extract IV info from base64 text
            var byteData = Convert.FromBase64String(base64Text);
            if (byteData.Length < AesIvSize)
            {
                throw new FormatException(
                    $"The input base64Text for decryption should not be less than {AesIvSize} bytes length!"
                );
            }

            var iv = new byte[16];
            Buffer.BlockCopy(byteData, 0, iv, 0, AesIvSize);
            var encryptedBytes = new byte[byteData.Length - AesIvSize];
            Buffer.BlockCopy(byteData, AesIvSize, encryptedBytes, 0, encryptedBytes.Length);

            // Get decryptor
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            // Decrypt the cipher bytes.
            using var msDecrypt = new MemoryStream(encryptedBytes);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }

        public static string EncryptTextToHexWithAes(string plainText, byte[] key)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            /* Create AES encryptor:
             * Mode: CBC
             * Block size: 16 bytes
             * Acceptable key sizes： [128, 192, 256]
             */
            using var aes = Aes.Create();
            var iv = aes.IV;
            aes.Key = key;
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            // Get encrypted bytes
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            var encryptedBytes = msEncrypt.ToArray();

            // Concat IV to encrypted bytes
            var result = new byte[AesIvSize + encryptedBytes.Length];
            Buffer.BlockCopy(iv, 0, result, 0, AesIvSize);
            Buffer.BlockCopy(encryptedBytes, 0, result, AesIvSize, encryptedBytes.Length);

            return Convert.ToHexString(result);
        }

        public static string DecryptTextFromHexStringWithAes(string base64Text, byte[] key)
        {
            if (string.IsNullOrEmpty(base64Text))
            {
                return base64Text;
            }

            // Extract IV info from base64 text
            var byteData = Convert.FromHexString(base64Text);
            if (byteData.Length < AesIvSize)
            {
                throw new FormatException(
                    $"The input base64Text for decryption should not be less than {AesIvSize} bytes length!"
                );
            }

            var iv = new byte[16];
            Buffer.BlockCopy(byteData, 0, iv, 0, AesIvSize);
            var encryptedBytes = new byte[byteData.Length - AesIvSize];
            Buffer.BlockCopy(byteData, AesIvSize, encryptedBytes, 0, encryptedBytes.Length);

            // Get decryptor
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            // Decrypt the cipher bytes.
            using var msDecrypt = new MemoryStream(encryptedBytes);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
    }
}
