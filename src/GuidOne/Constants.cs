﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GuidOne
{
    /// <summary>
    /// Random number generate modes supported
    /// </summary>
    public enum RandomNumberMode { Pseudo = 1, Crypto = 2 }

    /// <summary>
    /// Default GUID namespaces for V3 and V5 GUIDs from the RFC
    /// </summary>
    public static class GuidNamespaces
    {
        public static Guid DNS = Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c8");
        public static Guid URL = Guid.Parse("6ba7b811-9dad-11d1-80b4-00c04fd430c8");
        public static Guid ISO_OID = Guid.Parse("6ba7b812-9dad-11d1-80b4-00c04fd430c8");
        public static Guid X500_DN = Guid.Parse("6ba7b814-9dad-11d1-80b4-00c04fd430c8");
    }

    /// <summary>
    /// Constants from the RFC
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Length of the time bytes segment
        /// </summary>
        public const int TIME_BYTES_LENGTH = 8;

        /// <summary>
        /// Length of the node bytes segment
        /// </summary>
        public const int NODE_BYTES_LENGTH = 6;

        /// <summary>
        /// Length of the clock sequence bytes segment
        /// </summary>
        public const int CLOCK_SEQUENCE_BYTES_LENGTH = 2;

        /// <summary>
        /// Length of the GUID in bytes
        /// </summary>
        public const int GUID_BYTES_LENGTH = 16;

        /// <summary>
        /// Where the timestamp bytes start in the GUID 
        /// </summary>
        public const int TIMESTAMP_BYTE_INDEX = 0;

        /// <summary>
        /// The position of the version byte
        /// </summary>
        /// <remarks>
        /// In C# this depends on endianness due to the internal structure .NET stores GUIDs in
        /// </remarks>
        public static int VERSION_BYTE_INDEX
        {
            get
            {
                if (BitConverter.IsLittleEndian)
                    return 7;
                else
                    return 6;
            }
        }

        /// <summary>
        /// Position of the variant byte
        /// </summary>
        public const int VARIANT_BYTE_INDEX = 8;

        /// <summary>
        /// Where they clock sequence bytes start
        /// </summary>
        public const int CLOCK_SEQUENCE_BYTE_INDEX = 8;

        /// <summary>
        /// Where the node bytes start
        /// </summary>
        public const int NODE_BYTE_INDEX = 10;

        /// <summary>
        /// Offset from .NET 0 from Gregorian 0
        /// </summary>
        public static DateTimeOffset GREGORIAN_CALENDAR_OFFSET
        {
            get
            {
                return new DateTimeOffset(1582, 10, 15, 0, 0, 0, TimeSpan.Zero);
            }
        }
    }
}
