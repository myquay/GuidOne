using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace GuidOne.Providers
{
    /// <summary>
    /// Helper class used to generate the MD5 and SHA1 hashes used in V3 and V5 GUIDs
    /// </summary>
    internal static class HashProvider
    {
        internal static byte[] GenerateMD5Hash(byte[] namespaceId, byte[] data)
        {
            using (MD5 md5 = MD5.Create())
            {
                return md5.ComputeHash(namespaceId.Concat(data).ToArray()).Take(16).ToArray();
            }
        }

        internal static byte[] GenerateSHA1Hash(byte[] namespaceId, byte[] data)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                return sha1.ComputeHash(namespaceId.Concat(data).ToArray()).Take(16).ToArray();
            }
        }
    }
}
