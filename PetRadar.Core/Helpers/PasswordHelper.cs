using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Helpers
{
    public class PasswordHelper : IPasswordHelper
    {

        public byte[] GenerateHash(string password, byte[] salt)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Cannot be null or empty", nameof(password));

            return KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, iterationCount: 10000, 256);
        }

        public byte[] GenerateSalt()
        {
            byte[] salt = new byte[128];

            RandomNumberGenerator.Fill(salt);

            return salt;
        }

        public string GeneratePassword()
        {
            //Length of password
            int length = 8;
            //list of characters
            string validCharacters = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";

            Random random = new Random();

            // Select one random character from the list of characters to create a char array

            char[] characters = new char[length];
            for (int i = 0; i < length; i++)
            {
                characters[i] = validCharacters[random.Next(0, validCharacters.Length - 1)];
            }
            return new string(characters);
        }
    }
}
