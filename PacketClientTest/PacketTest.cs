using Microsoft.VisualStudio.TestTools.UnitTesting;
using PacketClient;
using System.Collections.Generic;

namespace PacketClientTest
{
    [TestClass]
    public class PacketTest
    {
        Packet packet;

        [TestInitialize]
        public void Initialize()
        {
            packet = new Packet(new List<byte>() { 7, 0, 4, 67, 68, 70, 0 });
        }

        [TestMethod]
        public void GetTypeTest()
        {
            Assert.AreEqual(4, packet.Type);

            packet = new Packet(new List<byte>() { 7, 0, 254, 67, 68, 70, 0 });
            Assert.AreEqual(254, packet.Type);
        }

        [TestMethod]
        public void GetSizeTest()
        {
            Assert.AreEqual(7, packet.Size);

            packet = new Packet(new List<byte>() { 7, 2, 254, 67, 68, 70, 0 });
            Assert.AreEqual(519, packet.Size);
        }

        [TestMethod]
        public void ConstructorTest()
        {
            packet = new Packet(112);

            Assert.AreEqual(3, packet.Size);
            Assert.AreEqual(112, packet.Type);
        }

        [TestMethod]
        public void ToAsciiStringTest()
        {
            Assert.AreEqual("CDF\0", packet.ToAsciiString());
        }

        [TestMethod]
        public void ToStringTest()
        {
            Assert.AreEqual("07-00-04-43-44-46-00", packet.ToString());
        }

    }
}
