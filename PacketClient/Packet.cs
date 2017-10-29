using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketClient
{
    public class Packet
    {
        List<byte> data;

        public Packet(byte type)
        {
            data = new List<byte>() { 3, 0, type };
        }

        public Packet(List<byte> data)
        {
            this.data = data;

            if (Size > 1024)
                SetSize(1024);
        }

        public byte Type { get { return data[2]; } }
        public ushort Size { get { return BitConverter.ToUInt16(data.Take(2).ToArray(), 0); } }
        public byte[] Buffer { get { return data.ToArray(); } }

        public string ToAsciiString()
        {
            return Encoding.Default.GetString(data.Skip(3).ToArray());
        }

        public override string ToString()
        {
            return BitConverter.ToString(data.ToArray());
        }

        private void SetSize(ushort size)
        {
            var bytes = BitConverter.GetBytes(size);
            data[0] = bytes[0];
            data[1] = bytes[1];
        }

        private void AddSize(ushort size)
        {
            SetSize((ushort)(this.Size + size));
        }

        private void RemoveSize(ushort size)
        {
            SetSize((ushort)(this.Size - size));
            data.RemoveRange(3, size);
        }

        public void Write(params object[] args)
        {
            Action<object> write = (arg) =>
            {
                byte[] bytes = BitConverter.GetBytes((dynamic)arg);
                foreach (var b in bytes)
                    data.Add(b);
                AddSize((ushort)bytes.Length);
            };

            Action<object> writebyte = (arg) =>
            {
                data.Add((byte)arg);
                AddSize(1);
            };

            Action<object> writesbyte = (arg) =>
            {
                data.Add(unchecked((byte)(sbyte)arg));
                AddSize(1);
            };

            Action<object> writestring = (arg) =>
            {
                byte[] bytes = Encoding.ASCII.GetBytes((dynamic)arg);
                foreach (var b in bytes)
                    data.Add(b);
                data.Add(0);
                AddSize((ushort)(bytes.Length + 1));
            };

            var @switch = new Dictionary<Type, Action<object>>
            {
                { typeof(byte),     writebyte },
                { typeof(sbyte),    writesbyte },
                { typeof(short),    write },
                { typeof(ushort),   write },
                { typeof(int),      write },
                { typeof(uint),     write },
                { typeof(long),    write },
                { typeof(ulong),   write },
                { typeof(string),   writestring },
                { typeof(float), write },
                { typeof(double), write }
            };

            foreach (var arg in args)
            {
                if (@switch.ContainsKey(arg.GetType()))
                    @switch[arg.GetType()](arg);
            }
        }

        public byte ReadByte()
        {
            if (Size < 3 + 1)
                return 0;
            var b = data[3];
            RemoveSize(1);
            return b;
        }

        public sbyte ReadSByte()
        {
            if (Size < 3 + 1)
                return 0;
            var b = data[3];
            RemoveSize(1);
            return unchecked((sbyte)b);
        }

        public short ReadShort()
        {
            if (Size < 3 + 2)
                return 0;
            var s = BitConverter.ToInt16(data.ToArray(), 3);
            RemoveSize(2);
            return s;
        }

        public ushort ReadUShort()
        {
            if (Size < 3 + 2)
                return 0;
            var s = BitConverter.ToUInt16(data.ToArray(), 3);
            RemoveSize(2);
            return s;
        }

        public int ReadInt()
        {
            if (Size < 3 + 4)
                return 0;
            var i = BitConverter.ToInt32(data.ToArray(), 3);
            RemoveSize(4);
            return i;
        }

        public uint ReadUInt()
        {
            if (Size < 3 + 4)
                return 0;
            var i = BitConverter.ToUInt32(data.ToArray(), 3);
            RemoveSize(4);
            return i;
        }

        public long ReadLong()
        {
            if (Size < 3 + 8)
                return 0;
            var l = BitConverter.ToInt64(data.ToArray(), 3);
            RemoveSize(8);
            return l;
        }

        public ulong ReadULong()
        {
            if (Size < 3 + 8)
                return 0;
            var l = BitConverter.ToUInt64(data.ToArray(), 3);
            RemoveSize(8);
            return l;
        }

        public string ReadString()
        {
            if (Size < 3 + 1)
                return "";

            int? index = data.Select((b, i) => new { b, i }).FirstOrDefault((e) => e.i >= 3 && e.b == 0)?.i;
            if (index == null)
                return "";

            if (index.Value == 3)
            {
                RemoveSize(1);
                return "";
            }

            var len = index.Value - 3;
            var bytes = data.Skip(3).Take(len).ToArray();
            RemoveSize((ushort)(len + 1));
            return Encoding.UTF8.GetString(bytes);
        }

        public float ReadFloat()
        {
            if (Size < 3 + 4)
                return 0.0f;

            float f = BitConverter.ToSingle(data.ToArray(), 3);
            RemoveSize(4);
            return f;
        }

        public double ReadDouble()
        {
            if (Size < 3 + 8)
                return 0.0f;

            double d = BitConverter.ToDouble(data.ToArray(), 3);
            RemoveSize(8);
            return d;
        }

        private IEnumerable<object> ReadEx(params Type[] types)
        {
            foreach (var type in types)
            {
                var @switch = new Dictionary<Type, Func<object, object>>
                {
                    { typeof(byte),     (t) => { return ReadByte(); } },
                    { typeof(sbyte),    (t) => { return ReadSByte(); } },
                    { typeof(short),    (t) => { return ReadShort(); } },
                    { typeof(ushort),   (t) => { return ReadUShort(); } },
                    { typeof(int),      (t) => { return ReadInt(); } },
                    { typeof(uint),     (t) => { return ReadUInt(); } },
                    { typeof(long),     (t) => { return ReadLong(); } },
                    { typeof(ulong),    (t) => { return ReadULong(); } },
                    { typeof(string),   (t) => { return ReadString(); } },
                    { typeof(float),    (t) => { return ReadFloat(); } },
                    { typeof(double),    (t) => { return ReadDouble(); } }
                };

                if (@switch.ContainsKey(type))
                    yield return @switch[type](type);
                else
                    yield return null;
            }
        }

        public object[] Read(params Type[] types)
        {
            return ReadEx(types).ToArray();
        }
    }
}
