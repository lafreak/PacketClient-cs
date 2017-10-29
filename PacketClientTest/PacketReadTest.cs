using Microsoft.VisualStudio.TestTools.UnitTesting;
using PacketClient;
using System;
using System.Collections.Generic;

namespace PacketClientTest
{
    [TestClass]
    public class PacketReadTest
    {
        [TestMethod]
        public void ReadMultipleTest()
        {
            var p = new Packet(new List<byte>() { 13, 0, 1, 6, 0, 0, 0, 3,
                Convert.ToByte('p'), Convert.ToByte('a'), Convert.ToByte('c'), Convert.ToByte('k'), 0 });

            var v = p.Read(typeof(int), typeof(byte), typeof(string));

            Assert.AreEqual(3, v.Length);
            Assert.AreEqual(3, p.Size);
            Assert.AreEqual(6, (int)v[0]);
            Assert.AreEqual(3, (byte)v[1]);
            Assert.AreEqual("pack", (string)v[2]);
        }

        [TestMethod]
        public void ReadIntAndByteSimpleTest()
        {
            var p = new Packet(new List<byte>() { 8, 0, 1, 6, 0, 0, 0, 3 });

            Assert.AreEqual(6, p.ReadInt());
            Assert.AreEqual(3, p.ReadByte());
        }

        [TestMethod]
        public void ReadStringTest()
        {
            var p = new Packet(new List<byte>() { 12, 0, 1, 107, 97, 108, 0, 97, 0, 119, 120, 0 });

            Assert.AreEqual("kal", p.ReadString());
            Assert.AreEqual("a", p.ReadString());
            Assert.AreEqual("wx", p.ReadString());
        }

        [TestMethod]
        public void ReadString2Test()
        {
            var p = new Packet(new List<byte>() { 9, 0, 1, 0, 107, 97, 108, 0, 97 });

            Assert.AreEqual("", p.ReadString());
            Assert.AreEqual("kal", p.ReadString());
            Assert.AreEqual("", p.ReadString());
        }

        [TestMethod]
        public void ReadFloatTest()
        {
            var p = new Packet(new List<byte>() { 7,0,1, 0x3b, 0xdf, 0x1f, 0x41 });

            Assert.AreEqual(9.992f, p.ReadFloat());
            Assert.AreEqual(3, p.Size);
        }

        [TestMethod]
        public void ReadDoubleTest()
        {
            var p = new Packet(new List<byte>() { 11,0,1, 0x92, 0x93, 0x89, 0x5b, 0x05, 0x31, 0xba, 0x3f });

            Assert.AreEqual(0.1023105, p.ReadDouble());
            Assert.AreEqual(3, p.Size);
        }
    }
}
