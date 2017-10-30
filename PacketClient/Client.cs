using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace PacketClient
{
    public class Client
    {
        string hostname;
        int port;
        TcpClient client;
        bool firstConnectAttempt = true;
        Thread worker;
        bool shouldInvokeOnDisconnected = true;

        Action onConnected = () => { };
        Action onDisconnected = () => { };
        Action<Exception> onUnableToConnect = (e) => { };
        Action<Packet> onUnknownPacket = (packet) => { };

        public bool Connected { get { return client != null && client.Connected; } }

        IDictionary<byte, Action<Packet>> events = new Dictionary<byte, Action<Packet>>();

        public Client(string hostname, int port)
        {
            this.hostname = hostname;
            this.port = port;
        }

        private void RunWorker(ThreadStart method)
        {
            worker = new Thread(method);
            worker.IsBackground = true;
            worker.Start();
        }

        public void Disconnect()
        {
            if (worker == null)
                return;

            shouldInvokeOnDisconnected = false;

            client.Close();
            firstConnectAttempt = true;
        }

        public void Connect()
        {
            if (firstConnectAttempt)
            {
                firstConnectAttempt = false;
                RunWorker(Connect);
                return;
            }

            if (client != null && client.Connected)
                return;

            try
            {
                client = new TcpClient();
                client.Connect(hostname, port);
            }
            catch (Exception e)
            {
                onUnableToConnect(e);
                return;
            }

            onConnected();

            Listen();
        }

        private void Listen()
        {
            while (true)
            {
                try
                {
                    Receive().ToList().ForEach((packet) =>
                    {
                        if (events.ContainsKey(packet.Type))
                            events[packet.Type](packet);
                        else
                            onUnknownPacket(packet);
                    });
                }
                catch
                {
                    client.Close();

                    if (!shouldInvokeOnDisconnected)
                        shouldInvokeOnDisconnected = true;
                    else
                        onDisconnected();
                    return;
                }
            }
        }

        private IEnumerable<Packet> Receive()
        {
            byte[] buffer = new byte[1024];
            int n = client.GetStream().Read(buffer, 0, 1024);

            while (n > 0)
            {
                ushort m = BitConverter.ToUInt16(buffer, 0);
                if (m == 0) m = 1;

                Packet packet = new Packet(buffer.Take(m).ToList());

                n -= m;
                var temp = buffer.Skip(m).ToList();
                while (temp.Count < 1024)
                    temp.Add(0);
                buffer = temp.ToArray();

                if (m < 3)
                    continue;

                yield return packet;
            }
        }

        public void OnConnected(Action action)
        {
            onConnected = action;
        }

        public void OnDisconnected(Action action)
        {
            onDisconnected = action;
        }

        public void OnUnableToConnect(Action<Exception> action)
        {
            onUnableToConnect = action;
        }

        public void OnUnknownPacket(Action<Packet> action)
        {
            onUnknownPacket = action;
        }

        public void On(byte type, Action<Packet> action)
        {
            events[type] = action;
        }

        public void Send(byte type, params object[] args)
        {
            if (!client.Connected)
                return;
            var packet = new Packet(type);
            packet.Write(args);

            SendPacket(packet);
        }

        public void SendPacket(Packet packet)
        {
            if (!client.Connected)
                return;

            try
            {
                client.GetStream().Write(packet.Buffer, 0, packet.Size);
            }
            catch
            {
                onDisconnected();
            }
        }
    }
}
