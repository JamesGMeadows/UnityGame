using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public class ClientConnection
    {

        private readonly ConcurrentQueue<Packet> queue = new ConcurrentQueue<Packet>();
        private readonly PacketDecoder decoder = new PacketDecoder();
        private readonly PacketEncoder encoder = new PacketEncoder();
        private readonly Socket client;
        private ClientStatus status;

        public ClientConnection(Socket client)
        {
            this.client = client;
        }

        public PacketDecoder GetDecoder()
        {
            return decoder;
        }

        public void SendPacket(Packet packet)
        {
            NetworkStream stream = new NetworkStream(client);
            BinaryWriter writer = new BinaryWriter(stream);
            encoder.Encode(packet, writer);
            writer.Flush();
        }

        public ConcurrentQueue<Packet> GetQueue()
        {
            return queue;
        }

        public Socket GetSocket()
        {
            return client;
        }

        public void SetStatus(ClientStatus status)
        {
            this.status = status;
        }

        public ClientStatus GetStatus()
        {
            return status;
        }
    }

    public enum ClientStatus
    {
        NOT_CONNECTED,
        CONNECTING,
        ACTIVE,
        DISCONNECTING
    }
}
