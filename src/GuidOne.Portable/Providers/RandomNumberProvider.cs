using Org.BouncyCastle.Security;

namespace GuidOne.Providers
{
    /// <summary>
    /// Helper class to generate cryptographically strong numbers on .NET portable
    /// </summary>
    public partial class RandomNumberProvider
    {
        private SecureRandom random = new SecureRandom();

        private void FillCryptoBytes(byte[] bytes)
        {
            random.NextBytes(bytes); // generates 8 random bytes
        }
    }
}
