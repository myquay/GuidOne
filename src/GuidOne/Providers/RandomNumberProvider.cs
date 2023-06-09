using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace GuidOne.Providers
{
    /// <summary>
    /// Provide support for quicker pseudo-random numbers as well as support for cryptographically strong random numbers 
    /// </summary>
    internal static class RandomNumberProvider
    {

        internal static void FillBytes(byte[] bytes, RandomNumberMode mode = RandomNumberMode.Crypto)
        {
            if (mode == RandomNumberMode.Pseudo)
            {
                    new Random().NextBytes(bytes);
            }
            else
            {
                using (var cryptoProvider = new RNGCryptoServiceProvider())
                {
                    cryptoProvider.GetBytes(bytes);
                }
            }
        }

        internal static byte[] GetRandomBytes(int length, RandomNumberMode mode = RandomNumberMode.Crypto)
        {
            var bytes = new byte[length];
            RandomNumberProvider.FillBytes(bytes, mode);
            return bytes;
        }
    }
}
