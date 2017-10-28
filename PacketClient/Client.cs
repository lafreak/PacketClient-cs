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

        Action onConnected = () => { };
        Action onDisconnected = () => { };
        Action<Exception> onUnableToConnect = (e) => { };
        Action<Packet> onUnknownPacket = (packet) => { };

        IDictionary<ushort, Action<Packet>> events = new Dictionary<ushort, Action<Packet>>();

        public Client(string hostname, int port)
        {
            this.hostname = hostname;
            this.port = port;
        }

        private void RunInBackground(ThreadStart method)
        {
            Thread t = new Thread(method);
            t.IsBackground = true;
            t.Start();
        }

        public void Connect()
        {
            if (firstConnectAttempt)
            {
                firstConnectAttempt = false;
                RunInBackground(Connect);
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
                onUnableToConnect.Invoke(e);
                return;
            }

            onConnected.Invoke();

            Listen();
        }

        private void Listen()
        {
            byte[] buffer = new byte[1024];

            while (true)
            {
                int n;
                try
                {
                    n = client.GetStream().Read(buffer, 0, 1024);
                }
                catch (Exception)
                {
                    onDisconnected.Invoke();
                    client.Close();
                    return;
                }

                Packet packet = new Packet(buffer.Take(n).ToList());

                if (events.ContainsKey(packet.Type))
                    events[packet.Type].Invoke(packet);
                else
                    onUnknownPacket.Invoke(packet);
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

        public void On(ushort type, Action<Packet> action)
        {
            events[type] = action;
        }

        public void Send(byte type, params object[] args)
        {
            if (!client.Connected)
                return;
            var packet = new Packet(type);
            packet.Write(args);
            client.GetStream().Write(packet.Buffer, 0, packet.Size);
        }
    }
}
