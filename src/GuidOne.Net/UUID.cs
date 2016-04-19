using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace GuidOne
{
    /// <summary>
    /// Representation of a UUID
    /// </summary>
    public partial class UUID
    {
        /// <summary>
        /// Generate a V1 UUID using the device's physical MAC address for the current time
        /// </summary>
        /// <param name="mac">The current machine's MAC address</param>
        /// <returns>A new UUID</returns>
        public static UUID V1(PhysicalAddress mac)
        {
            return V1(DateTime.UtcNow, mac);
        }

        /// <summary>
        /// Generate a V1 UUID using the device's IP address for the current time
        /// </summary>
        /// <param name="ip">The current machine's IP address</param>
        /// <returns>A new UUID</returns>
        public static UUID V1(IPAddress ip)
        {
            return V1(DateTime.UtcNow, ip);
        }

        /// <summary>
        /// Generate a V1 UUID using the device's physical MAC address for a specific time
        /// </summary>
        /// <param name="dateTime">The timestamp to create the GUID for</param>
        /// <param name="ip">The current machine's MAC address</param>
        /// <returns>A new UUID</returns>
        public static UUID V1(DateTime dateTime, PhysicalAddress mac)
        {
            return V1(DateTimeToTimeBytes(dateTime), DateTimeToClockSequenceBytes(dateTime), mac.GetAddressBytes());
        }

        /// <summary>
        /// Generate a V1 UUID using the device's IP address for a specific time
        /// </summary>
        /// <param name="dateTime">The timestamp to create the GUID for</param>
        /// <param name="ip">The current machine's IP address</param>
        /// <returns>A new UUID</returns>
        public static UUID V1(DateTime dateTime, IPAddress ip)
        {
            return V1(DateTimeToTimeBytes(dateTime), DateTimeToClockSequenceBytes(dateTime), IPAddressToNodeBytes(ip));
        }

        private static byte[] IPAddressToNodeBytes(IPAddress ip)
        {
            var ipBytes = ip.GetAddressBytes();
            var bytes = new byte[6];
            Array.Copy(ipBytes, bytes, 6);
            return bytes;
        }
    }
}
