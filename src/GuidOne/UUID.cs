using GuidOne.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;

namespace GuidOne
{
    public enum GuidVersion { Unknown = 0, Time = 1, DCE = 2, MD5 = 3, Random = 4, SHA1 = 5 }
    public enum GuidVariant { Unknown = 0, NCSReserved = 1, RFC4122 = 2, MicrosoftReserved = 3, FutureReserved = 4 }

    /// <summary>
    /// UUID (Universally Unique Identifier) is a 128-bit number used to uniquely identify some object or entity on the Internet.
    /// </summary>
    /// <seealso cref="http://www.ietf.org/rfc/rfc4122.txt">A Universally Unique Identifier (UUID) URN Namespace</seealso>
    public struct Uuid : IFormattable, IComparable, IComparable<Uuid>, IEquatable<Uuid>
    {

        /// <summary>
        /// Internal representation of the GUID
        /// </summary>
        private Guid _guid;

        /// <summary>
        /// Extract the GUID Version
        /// </summary>
        public GuidVersion Version
        {
            get
            {
                var versionByte = _guid.ToByteArray()[Constants.VERSION_BYTE_INDEX] & 0xf0;
                var version = Convert.ToInt32(versionByte);

                switch (version)
                {
                    case 0x10:
                        return GuidVersion.Time;
                    case 0x20:
                        return GuidVersion.DCE;
                    case 0x30:
                        return GuidVersion.MD5;
                    case 0x40:
                        return GuidVersion.Random;
                    case 0x50:
                        return GuidVersion.SHA1;
                    default:
                        return GuidVersion.Unknown;
                }
            }
        }

        /// <summary>
        /// Extract the GUID Variant
        /// </summary>
        public GuidVariant Variant
        {
            get
            {
                var variantByte = _guid.ToByteArray()[Constants.VARIANT_BYTE_INDEX];
                var variant = Convert.ToInt32(variantByte);

                if ((variant & 0xE0) == 0xE0)
                    return GuidVariant.FutureReserved;
                if ((variant & 0xC0) == 0xC0)
                    return GuidVariant.MicrosoftReserved;
                if ((variant & 0x80) == 0x80)
                    return GuidVariant.RFC4122;
                if ((variant & 0x00) == 0x00)
                    return GuidVariant.NCSReserved;

                return GuidVariant.Unknown;
            }
        }

        /// <summary>
        /// Extract the timestamp (returns null if not V1)
        /// </summary>
        public DateTime? Timestamp
        {
            get
            {
                if (Version != GuidVersion.Time)
                    return null;

                var guidBytes = _guid.ToByteArray();

                if (BitConverter.IsLittleEndian)
                    ToggleEndianness(guidBytes);

                var timeHigh = new byte[2];
                var timeMid = new byte[2];
                var timeLow = new byte[4];

                Array.Copy(guidBytes, 6, timeHigh, 0, 2);
                Array.Copy(guidBytes, 4, timeMid, 0, 2);
                Array.Copy(guidBytes, 0, timeLow, 0, 4);

                timeHigh[0] &= 0x07;

                var timeBytes = new byte[8];
                Array.Copy(timeHigh, 0, timeBytes, 0, 2);
                Array.Copy(timeMid, 0, timeBytes, 2, 2);
                Array.Copy(timeLow, 0, timeBytes, 4, 4);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(timeBytes);

                var ticks = BitConverter.ToInt64(timeBytes, 0) + Constants.GREGORIAN_CALENDAR_OFFSET.Ticks;

                return new DateTime(ticks);
            }
        }

        /// <summary>
        /// Nil Uuid
        /// </summary>
        public static Uuid Nil => new Uuid(Guid.Empty);

        /// <summary>
        /// UUID Constructor from string
        /// </summary>
        /// <param name="uuid"></param>
        public Uuid(Guid guid)
        {
            _guid = guid;
        }

        #region UUID V1

        /// <summary>
        /// Generate a new V1 GUID using the current timestamp and random node bytes
        /// </summary>
        /// <returns></returns>
        public static Uuid NewV1() => Uuid.NewV1(DateTime.UtcNow);

        /// <summary>
        /// Generate a new V1 GUID using a specific time and random node bytes
        /// </summary>
        /// <param name="dateTime">Time to use in the V1 GUID</param>
        /// <returns></returns>
        public static Uuid NewV1(DateTime dateTime) => Uuid.NewV1(dateTime, RandomNumberProvider.GetRandomBytes(Constants.CLOCK_SEQUENCE_BYTES_LENGTH));

        /// <summary>
        /// Generate a new V1 GUID from specified time and clock sequence bytes
        /// </summary>
        /// <param name="timeBytes">Bytes for the time value to use in the V1 GUID</param>
        /// <param name="clockSequenceBytes">Clock sequence bytes to use the the V1 GUID</param>
        /// <returns></returns>
        public static Uuid NewV1(DateTime dateTime, byte[] clockSequenceBytes) => Uuid.NewV1(dateTime, clockSequenceBytes, RandomNumberProvider.GetRandomBytes(Constants.NODE_BYTES_LENGTH));

        /// <summary>
        /// Generate a new V1 GUID from specified time, clock sequence bytes and node bytes
        /// </summary>
        /// <param name="timeBytes">Bytes for the time value to use in the V1 GUID</param>
        /// <param name="clockSequenceBytes">Clock sequence bytes to use the the V1 GUID</param>
        /// <returns></returns>
        public static Uuid NewV1(DateTime dateTime, byte[] clockSequenceBytes, byte[] nodeBytes)  => Uuid.NewV1(DateTimeToTimeBytes(dateTime), clockSequenceBytes, nodeBytes);

        /// <summary>
        /// Generate a new V1 GUID from specified time, clock sequence bytes and node bytes
        /// </summary>
        /// <param name="timeBytes">Bytes for the time value to use in the V1 GUID</param>
        /// <param name="clockSequenceBytes">Clock sequence bytes to use the the V1 GUID</param>
        /// <param name="nodeBytes">Node bytes to use in the V1 GUID</param>
        /// <returns></returns>
        public static Uuid NewV1(byte[] timeBytes, byte[] clockSequenceBytes, byte[] nodeBytes) =>  GenerateFromComponents(timeBytes, clockSequenceBytes, nodeBytes, GuidVersion.Time, GuidVariant.RFC4122);

        /// <summary>
        /// Convert a timestamp to bytes
        /// </summary>
        /// <param name="dateTime">The timestamp we're converting to bytes</param>
        /// <returns></returns>
        private static byte[] DateTimeToTimeBytes(DateTime dateTime)
        {
            var timeBytes = new byte[Constants.TIME_BYTES_LENGTH];

            var bytesToCopy = BitConverter.GetBytes(dateTime.Ticks - Constants.GREGORIAN_CALENDAR_OFFSET.Ticks); //Since .NET time is from 0, minus all the extra ticks
            Array.Copy(bytesToCopy, 0, timeBytes, 0, Math.Min(Constants.TIME_BYTES_LENGTH, timeBytes.Length));

            return timeBytes;
        }

        #endregion

        #region UUID V3

        /// <summary>
        /// Generate a V3 GUID with a given name in a given namespace
        /// </summary>
        /// <param name="namespaceId">The namespace we're generating the GUID in</param>
        /// <param name="name">The name we're generating the GUID for</param>
        /// <returns></returns>
        public static Uuid NewV3(Guid namespaceId, string name)
        {
            return GenerateNameBased(namespaceId, name, GuidVersion.MD5);
        }

        #endregion

        #region UUID V4

        /// <summary>
        /// Generate a random V4 GUID
        /// </summary>
        /// <returns></returns>
        public static Uuid NewV4()
        {
            var timeBytes = RandomNumberProvider.GetRandomBytes(Constants.TIME_BYTES_LENGTH);
            var clockSequenceBytes = RandomNumberProvider.GetRandomBytes(Constants.CLOCK_SEQUENCE_BYTES_LENGTH);
            var nodeBytes = RandomNumberProvider.GetRandomBytes(Constants.NODE_BYTES_LENGTH);

            return GenerateFromComponents(timeBytes, clockSequenceBytes, nodeBytes, GuidVersion.Random, GuidVariant.RFC4122);
        }

        #endregion

        #region UUID V5

        /// <summary>
        /// Generate a V5 UUID with a given name in a given namespace
        /// </summary>
        /// <param name="namespaceId">The namespace we're generating the GUID in</param>
        /// <param name="name">The name we're generating the GUID for</param>
        /// <returns></returns>
        public static Uuid NewV5(Guid namespaceId, string name)
        {
            return GenerateNameBased(namespaceId, name, GuidVersion.SHA1);
        }

        #endregion

        #region UUID NAME BASED

        /// <summary>
        /// Generate a namespace GUID (V3 or V5) with a given algorithm
        /// </summary>
        /// <param name="namespaceId">The namespace we're generating the GUID in</param>
        /// <param name="name">The name we're generating the GUID for</param>
        /// <param name="version">The version to generate (MD5 or SHA1)</param>
        /// <returns></returns>
        private static Uuid GenerateNameBased(Guid namespaceId, string name, GuidVersion version)
        {
            if (version != GuidVersion.MD5 && version != GuidVersion.SHA1)
                throw new ArgumentException("Name based GUIDs can only be version 3 (MD5), or 5 (SHA1)", nameof(version));

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "The name parameter cannot be empty or null");

            byte[] nameBytes = Encoding.UTF8.GetBytes(name);
            byte[] namespaceBytes = namespaceId.ToByteArray();

            if (BitConverter.IsLittleEndian)
                ToggleEndianness(namespaceBytes);

            var hash = version == GuidVersion.MD5 ?
                HashProvider.GenerateMD5Hash(namespaceBytes, nameBytes) :
                HashProvider.GenerateSHA1Hash(namespaceBytes, nameBytes);

            if (BitConverter.IsLittleEndian)
                ToggleEndianness(hash);

            return GenerateFromComponents(
                hash.Skip(Constants.TIMESTAMP_BYTE_INDEX).Take(Constants.TIME_BYTES_LENGTH).ToArray(),
                hash.Skip(Constants.CLOCK_SEQUENCE_BYTE_INDEX).Take(Constants.CLOCK_SEQUENCE_BYTES_LENGTH).ToArray(),
                hash.Skip(Constants.NODE_BYTE_INDEX).Take(Constants.NODE_BYTES_LENGTH).ToArray(),
                version, GuidVariant.RFC4122);
        }

        #endregion

        /// <summary>
        /// Generate a GUID from its components
        /// </summary>
        /// <param name="timeBytes">The time based component</param>
        /// <param name="clockSequenceBytes">The clock sequence component</param>
        /// <param name="nodeBytes">The node component</param>
        /// <param name="version">The GUID version</param>
        /// <param name="variant">The GUID variant</param>
        /// <returns></returns>
        private static Uuid GenerateFromComponents(byte[] timeBytes, byte[] clockSequenceBytes, byte[] nodeBytes, GuidVersion version, GuidVariant variant)
        {
            var guidBytes = new byte[Constants.GUID_BYTES_LENGTH];

            if (clockSequenceBytes == null)
                throw new ArgumentException("Please specify the clock sequence bytes", "clockSequenceBytes");

            if (nodeBytes == null)
                throw new ArgumentException("Please specify the node bytes", "nodeBytes");

            if (clockSequenceBytes.Length != Constants.CLOCK_SEQUENCE_BYTES_LENGTH)
                throw new ArgumentException("The clock sequence bytes must be of length " + Constants.CLOCK_SEQUENCE_BYTES_LENGTH, "clockSequenceBytes");

            if (nodeBytes.Length != Constants.NODE_BYTES_LENGTH)
                throw new ArgumentException("The node bytes must be of length " + Constants.NODE_BYTES_LENGTH, "nodeBytes");

            if (timeBytes.Length != Constants.TIME_BYTES_LENGTH)
                throw new ArgumentException("The time bytes must be of length " + Constants.TIME_BYTES_LENGTH, "nodeBytes");

            //Copy over the different byte segments
            Array.Copy(timeBytes, 0, guidBytes, Constants.TIMESTAMP_BYTE_INDEX, Constants.TIME_BYTES_LENGTH);
            Array.Copy(clockSequenceBytes, 0, guidBytes, Constants.CLOCK_SEQUENCE_BYTE_INDEX, Constants.CLOCK_SEQUENCE_BYTES_LENGTH);
            Array.Copy(nodeBytes, 0, guidBytes, Constants.NODE_BYTE_INDEX, Constants.NODE_BYTES_LENGTH);

            //To put in the variant, take the  9th byte and perform an and operation using  0x3f, followed by an or operation with 0x80.

            //Put in the version
            int maskVariant;
            int shiftVariant;

            switch (variant)
            {
                case GuidVariant.FutureReserved:
                    maskVariant = 0x1F;
                    shiftVariant = 0xE0;
                    break;
                case GuidVariant.MicrosoftReserved:
                    maskVariant = 0x1F;
                    shiftVariant = 0xC0;
                    break;
                case GuidVariant.NCSReserved:
                    maskVariant = 0x0;
                    shiftVariant = 0x0;
                    break;
                case GuidVariant.RFC4122:
                    maskVariant = 0x3f;
                    shiftVariant = 0x80;
                    break;
                default:
                    maskVariant = 0x3f;
                    shiftVariant = 0;
                    break;
            }

            int maskVersion;
            int shiftVersion;
            switch (version)
            {
                case GuidVersion.DCE:
                    maskVersion = 0x0f;
                    shiftVersion = 0x20;
                    break;
                case GuidVersion.MD5:
                    maskVersion = 0x0f;
                    shiftVersion = 0x30;
                    break;
                case GuidVersion.Random:
                    maskVersion = 0x0f;
                    shiftVersion = 0x40;
                    break;
                case GuidVersion.SHA1:
                    maskVersion = 0x0f;
                    shiftVersion = 0x50;
                    break;
                case GuidVersion.Time:
                    maskVersion = 0x0f;
                    shiftVersion = 0x10;
                    break;
                default:
                    maskVersion = 0x3f;
                    shiftVersion = 0;
                    break;
            }

            guidBytes[Constants.VARIANT_BYTE_INDEX] &= (byte)maskVariant;
            guidBytes[Constants.VARIANT_BYTE_INDEX] |= (byte)shiftVariant;

            guidBytes[Constants.VERSION_BYTE_INDEX] &= (byte)maskVersion;
            guidBytes[Constants.VERSION_BYTE_INDEX] |= (byte)shiftVersion;

            return new Uuid(new Guid(guidBytes));
        }

        /// <summary>
        /// Return as a .NET GUID
        /// </summary>
        /// <returns></returns>
        public Guid AsGuid()
        {
            return _guid;
        }

        #region Helper methods to help us work with the correct byte order regardless of platform

        private static void ToggleEndianness(byte[] guid)
        {
            SwitchBytes(guid, 0, 3);
            SwitchBytes(guid, 1, 2);
            SwitchBytes(guid, 4, 5);
            SwitchBytes(guid, 6, 7);
        }

        private static void SwitchBytes(byte[] guid, int left, int right)
        {
            (guid[right], guid[left]) = (guid[left], guid[right]);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ((IFormattable)_guid).ToString(format, formatProvider);
        }

        public int CompareTo(object obj)
        {
            return ((IComparable)_guid).CompareTo(obj);
        }

        public int CompareTo(Uuid other)
        {
            return AsGuid().CompareTo(other.AsGuid());
        }

        public bool Equals(Uuid other)
        {
            return AsGuid().Equals(other.AsGuid());
        }

        #endregion

    }


}