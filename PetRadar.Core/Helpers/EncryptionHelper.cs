using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Helpers
{
    public class EncryptionHelper : IEncryptionHelper
    {
        private readonly string _encryptionKey;
        private readonly string _encryptionIV;
        public EncryptionHelper()
        {
            _encryptionKey = Environment.GetEnvironmentVariable("ENCRYPTION_KEY");
            _encryptionIV = Environment.GetEnvironmentVariable("ENCRYPTION_IV");
        }

        private byte[] GetBytes(string keyOrIV, int requiredLength)
        {
            if (string.IsNullOrEmpty(keyOrIV) || keyOrIV.Length != requiredLength)
                throw new InvalidOperationException("Invalid key or IV length");
            return Encoding.UTF8.GetBytes(keyOrIV);

        }

        private byte[] Key => GetBytes(_encryptionKey, 32); // AES-256 key length
        private byte[] IV => GetBytes(_encryptionIV, 16); // AES block size


        /// <summary>
        /// Encrypts the given plain text using AES encryption and returns the encrypted string in Base64 format.
        /// </summary>
        /// <param name="plainText">The plain text to encrypt.</param>
        /// <returns>The encrypted string in Base64 format.</returns>
        /// <exception cref="ArgumentException">Thrown when the plain text is null or empty.</exception>
        public string Encrypt (string plainText)
        {

            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("Cannot be null or empty", nameof(plainText));

            using var aes = Aes.Create();

            aes.Key = Key;
            aes.IV = IV;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
           
            using var ms = new MemoryStream();
            
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("Cannot be null or empty", nameof(cipherText));

            using var aes = Aes.Create();

            aes.Key = Key;
            aes.IV = IV;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));

            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))

            using (var sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }

    }
}
