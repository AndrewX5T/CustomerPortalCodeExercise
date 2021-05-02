using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;

namespace DataAccessLayer
{
    public interface IHashingService
    {
        public string Hash(string input);
    }

    public class HashingService : IHashingService, IServiceProvider
    {
        /// <summary>
        /// Takes a single string, converts via hashed algorithm
        /// </summary>
        /// <param name="input"></param>
        /// <returns>a 256-bit encrypted key</returns>
        public string Hash(string input)
        {
            string hashed = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: input,
                    prf: KeyDerivationPrf.HMACSHA1,
                    salt: new byte[0],
                    iterationCount: 16,
                    numBytesRequested: 256 / 8)
                );

            return hashed;
        }

        public object GetService(Type type)
        {
            return new HashingService();
        }
    }
}
