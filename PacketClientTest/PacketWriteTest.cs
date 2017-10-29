using Microsoft.VisualStudio.TestTools.UnitTesting;
using PacketClient;
using System.Collections.Generic;
using System.Linq;

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
            Assert.AreEqual(4, packet.Size);
        }

        [TestMethod]
        public void WriteSByteTest()
        {
            packet.Write((sbyte)-1);

            Assert.AreEqual(255, packet.Buffer[3]);
            Assert.AreEqual(4, packet.Size);
        }

        [TestMethod]
        public void WriteShortTest()
        {
            packet.Write((short)-2);

            Assert.AreEqual(254, packet.Buffer[3]);
            Assert.AreEqual(255, packet.Buffer[4]);
            Assert.AreEqual(5, packet.Size);
        }

        [TestMethod]
        public void WriteUShortTest()
        {
            packet.Write((ushort)11);

            Assert.AreEqual(11, packet.Buffer[3]);
            Assert.AreEqual(0, packet.Buffer[4]);
            Assert.AreEqual(5, packet.Size);
        }

        [TestMethod]
        public void WriteIntTest()
        {
            packet.Write((int)-3);

            Assert.AreEqual(253, packet.Buffer[3]);
            Assert.AreEqual(255, packet.Buffer[4]);
            Assert.AreEqual(255, packet.Buffer[5]);
            Assert.AreEqual(255, packet.Buffer[6]);
            Assert.AreEqual(7, packet.Size);
        }

        [TestMethod]
        public void WriteUIntTest()
        {
            packet.Write((uint)7);

            Assert.AreEqual(7, packet.Buffer[3]);
            Assert.AreEqual(0, packet.Buffer[4]);
            Assert.AreEqual(0, packet.Buffer[5]);
            Assert.AreEqual(0, packet.Buffer[6]);
            Assert.AreEqual(7, packet.Size);
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
            Assert.AreEqual(11, packet.Size);
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
            Assert.AreEqual(11, packet.Size);
        }

        [TestMethod]
        public void WriteStringTest()
        {
            packet.Write("Hey");

            Assert.AreEqual(72, packet.Buffer[3]);
            Assert.AreEqual(101, packet.Buffer[4]);
            Assert.AreEqual(121, packet.Buffer[5]);
            Assert.AreEqual(0, packet.Buffer[6]);
            Assert.AreEqual(7, packet.Size);
        }

        [TestMethod]
        public void WriteFloatTest()
        {
            packet.Write(3.13f);

            Assert.AreEqual(7, packet.Size);
            CollectionAssert.AreEquivalent(
                new byte[] { 0xec, 0x51, 0x48, 0x40 }, 
                packet.Buffer.Skip(3).Take(4).ToArray());
        }

        [TestMethod]
        public void WriteDoubleTest()
        {
            packet.Write(0.851012351);

            Assert.AreEqual(11, packet.Size);
            CollectionAssert.AreEquivalent(
                new byte[] { 0xb3, 0x2f, 0x01, 0x41, 0x7e, 0x3b, 0xeb, 0x3f },
                packet.Buffer.Skip(3).Take(8).ToArray());
        }
    }
}
