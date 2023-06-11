using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using GuidOne.Providers;

namespace GuidOne
{

    public enum GenerationMode { Fast = 1, NoDuplicates = 2 }

    public class UuidV1Generator : IDisposable
    {
        private readonly byte[] _clockSequenceBytes = new byte[Constants.CLOCK_SEQUENCE_BYTES_LENGTH];
        private readonly GenerationMode _generationMode;
        private readonly RandomNumberMode _randomNumberMode;
        private DateTime _lastTimeStampNoDuplicates;
        private readonly object _lock = new object();


        public UuidV1Generator(RandomNumberMode randomNumberMode = RandomNumberMode.Crypto, GenerationMode generationMode = GenerationMode.NoDuplicates)
        {
            _randomNumberMode = randomNumberMode;
            _generationMode = generationMode;
            RandomNumberProvider.FillBytes(_clockSequenceBytes, randomNumberMode);
            _lastTimeStampNoDuplicates = DateTime.UtcNow;
        }

        /// <summary>
        /// Generate a new V1 GUID using a specific time and random node bytes
        /// </summary>
        /// <param name="dateTime">Time to use in the V1 GUID</param>
        /// <returns></returns>
        public Uuid NewV1(DateTime dateTime) => Uuid.NewV1(dateTime, DateTimeToClockSequenceBytes(dateTime));

        /// <summary>
        /// Generate a V1 UUID using the device's physical MAC address for the current time
        /// </summary>
        /// <param name="mac">The current machine's MAC address</param>
        /// <returns>A new UUID</returns>
        public Uuid NewV1(PhysicalAddress mac) => NewV1(DateTime.UtcNow, mac);

        /// <summary>
        /// Generate a V1 UUID using the device's IP address for the current time
        /// </summary>
        /// <param name="ip">The current machine's IP address</param>
        /// <returns>A new UUID</returns>
        public Uuid NewV1(IPAddress ip) => NewV1(DateTime.UtcNow, ip);

        /// <summary>
        /// Generate a V1 UUID using the device's physical MAC address for a specific time
        /// </summary>
        /// <param name="dateTime">The timestamp to create the GUID for</param>
        /// <param name="ip">The current machine's MAC address</param>
        /// <returns>A new UUID</returns>
        public Uuid NewV1(DateTime dateTime, PhysicalAddress mac) => Uuid.NewV1(dateTime, DateTimeToClockSequenceBytes(dateTime), mac.GetAddressBytes());

        /// <summary>
        /// Generate a V1 UUID using the device's IP address for a specific time
        /// </summary>
        /// <param name="dateTime">The timestamp to create the GUID for</param>
        /// <param name="ip">The current machine's IP address</param>
        /// <returns>A new UUID</returns>
        public Uuid NewV1(DateTime dateTime, IPAddress ip) => Uuid.NewV1(dateTime, DateTimeToClockSequenceBytes(dateTime), IPAddressToNodeBytes(ip));

        private byte[] IPAddressToNodeBytes(IPAddress ip)
        {
            var ipBytes = ip.GetAddressBytes();
            var bytes = new byte[Constants.NODE_BYTES_LENGTH];
            Array.Copy(ipBytes, bytes, Constants.NODE_BYTES_LENGTH);
            return bytes;
        }

        /// <summary>
        /// If generation mode is "No Duplicates" then clock sequence bytes will be regenerated if timestamp is ever the same or gone backwards
        /// </summary>
        /// <param name="dateTime">The time the current V1 UUID is being generated for</param>
        /// <returns></returns>
        private byte[] DateTimeToClockSequenceBytes(DateTime dateTime)
        {
            switch (_generationMode)
            {
                case GenerationMode.Fast:
                    return _clockSequenceBytes;

                case GenerationMode.NoDuplicates:
                default:
                    lock (_lock)
                    {
                        if (dateTime <= _lastTimeStampNoDuplicates)
                            RandomNumberProvider.FillBytes(_clockSequenceBytes, _randomNumberMode);

                        _lastTimeStampNoDuplicates = dateTime;

                        return _clockSequenceBytes;
                    }
            }
        }

        public void Dispose()
        { }
    }
}
