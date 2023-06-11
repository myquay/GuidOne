using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace GuidOne.Tests
{
    [TestClass]
    public class UuidGenerationTests
    {
        /// <summary>
        /// Test for the hand worked version here: https://michael-mckenna.com/guid-as-gold/
        /// </summary>
        [TestMethod]
        public void VersionOneGenerateGuid()
        {
            var clockSequence = new byte[] { 0x78, 0x52 };
            var nodeBytes = PhysicalAddress.Parse("29-06-76-EC-E2-D7").GetAddressBytes();
            var guid = Uuid.NewV1(DateTime.Parse("2000-01-01"), clockSequence, nodeBytes);

            Assert.AreEqual(Guid.Parse("63b00000-bfde-11d3-b852-290676ece2d7"), guid.AsGuid());
        }

        [TestMethod]
        public void VersionOneExtractComponents()
        {
            var uuid = new Uuid(Guid.Parse("63b00000-bfde-11d3-b852-290676ece2d7"));

            Assert.AreEqual(uuid.Version, GuidVersion.Time);
            Assert.AreEqual(uuid.Variant, GuidVariant.RFC4122);
            Assert.AreEqual(uuid.Timestamp, DateTime.Parse("2000-01-01"));
        }

        [TestMethod]
        public void VersionThreeGenerateGuid()
        {

            var guid = Uuid.NewV3(GuidNamespaces.URL, "www.example.com");

            Assert.AreEqual(Guid.Parse("a777199a-c522-31c4-8f4b-335feec7215b"), guid.AsGuid());
        }

        [TestMethod]
        public void VersionFourExtractComponents()
        {
            var uuid = new Uuid(Guid.Parse("db3c0418-5dac-4eb9-96e7-bca812f2b362"));

            Assert.AreEqual(uuid.Version, GuidVersion.Random);
            Assert.AreEqual(uuid.Variant, GuidVariant.RFC4122);
            Assert.IsNull(uuid.Timestamp);
        }

        [TestMethod]
        public void VersionThreeExtractComponents()
        {
            var uuid = new Uuid(Guid.Parse("a777199a-c522-31c4-8f4b-335feec7215b"));

            Assert.AreEqual(uuid.Version, GuidVersion.MD5);
            Assert.AreEqual(uuid.Variant, GuidVariant.RFC4122);
            Assert.IsNull(uuid.Timestamp);
        }

        /// <summary>
        /// Test for the hand worked version here: https://michael-mckenna.com/guid-as-gold/
        /// </summary>
        [TestMethod]
        public void VersionFiveGenerateGuid()
        {

            var guid = Uuid.NewV5(GuidNamespaces.URL  , "www.example.com");

            Assert.AreEqual(Guid.Parse("b63cdfa4-3df9-568e-97ae-006c5b8fd652"), guid.AsGuid());
        }

        [TestMethod]
        public void VersionFiveExtractComponents()
        {
            var uuid = new Uuid(Guid.Parse("2ed6657d-e927-568b-95e1-2665a8aea6a2"));

            Assert.AreEqual(uuid.Version, GuidVersion.SHA1);
            Assert.AreEqual(uuid.Variant, GuidVariant.RFC4122);
            Assert.IsNull(uuid.Timestamp);
        }

    }
}