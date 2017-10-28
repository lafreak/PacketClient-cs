using Microsoft.VisualStudio.TestTools.UnitTesting;
using PacketClient;
using System.Collections.Generic;

namespace PacketClientTest
{
    [TestClass]
    public class PacketWriteTest
    {
        Packet packet;

        [TestInitialize]
        public void Initialize()
        {
            packet = new Packet(new List<byte>() { 3, 0, 1 });
        }

        [TestMethod]
        public void WriteByteTest()
        {
            packet.Write((byte)5);

            Assert.AreEqual(5, packet.Buffer[3]);
        }

        [TestMethod]
        public void WriteSByteTest()
        {
            packet.Write((sbyte)-1);

            Assert.AreEqual(255, packet.Buffer[3]);
        }

        [TestMethod]
        public void WriteShortTest()
        {
            packet.Write((short)-2);

            Assert.AreEqual(254, packet.Buffer[3]);
            Assert.AreEqual(255, packet.Buffer[4]);
        }

        [TestMethod]
        public void WriteUShortTest()
        {
            packet.Write((ushort)11);

            Assert.AreEqual(11, packet.Buffer[3]);
            Assert.AreEqual(0, packet.Buffer[4]);
        }

        [TestMethod]
        public void WriteIntTest()
        {
            packet.Write((int)-3);

            Assert.AreEqual(253, packet.Buffer[3]);
            Assert.AreEqual(255, packet.Buffer[4]);
            Assert.AreEqual(255, packet.Buffer[5]);
            Assert.AreEqual(255, packet.Buffer[6]);
        }

        [TestMethod]
        public void WriteUIntTest()
        {
            packet.Write((uint)7);

            Assert.AreEqual(7, packet.Buffer[3]);
            Assert.AreEqual(0, packet.Buffer[4]);
            Assert.AreEqual(0, packet.Buffer[5]);
            Assert.AreEqual(0, packet.Buffer[6]);
        }

        [TestMethod]
        public void WriteLongTest()
        {
            packet.Write((long)(-4));

            Assert.AreEqual(252, packet.Buffer[3]);
            Assert.AreEqual(255, packet.Buffer[4]);
            Assert.AreEqual(255, packet.Buffer[5]);
            Assert.AreEqual(255, packet.Buffer[6]);
            Assert.AreEqual(255, packet.Buffer[7]);
            Assert.AreEqual(255, packet.Buffer[8]);
            Assert.AreEqual(255, packet.Buffer[9]);
            Assert.AreEqual(255, packet.Buffer[10]);
        }

        [TestMethod]
        public void WriteULongTest()
        {
            packet.Write((ulong)(32));

            Assert.AreEqual(32, packet.Buffer[3]);
            Assert.AreEqual(0, packet.Buffer[4]);
            Assert.AreEqual(0, packet.Buffer[5]);
            Assert.AreEqual(0, packet.Buffer[6]);
            Assert.AreEqual(0, packet.Buffer[7]);
            Assert.AreEqual(0, packet.Buffer[8]);
            Assert.AreEqual(0, packet.Buffer[9]);
            Assert.AreEqual(0, packet.Buffer[10]);
        }

        [TestMethod]
        public void WriteStringTest()
        {
            packet.Write("Hey");

            Assert.AreEqual(72, packet.Buffer[3]);
            Assert.AreEqual(101, packet.Buffer[4]);
            Assert.AreEqual(121, packet.Buffer[5]);
            Assert.AreEqual(0, packet.Buffer[6]);
        }
    }
}
