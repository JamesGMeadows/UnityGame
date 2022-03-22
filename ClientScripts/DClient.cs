using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;
using System.Collections.Concurrent;

namespace Assets.Scripts.Networking
{
    public class DClient
    {


        private TcpClient client;
        private NetworkStream stream;
        private ConcurrentQueue<Packet> queue;
        private Thread networkThread;
        private PacketDecoder decoder;
        private PacketEncoder encoder;
        private volatile bool running = true;
        private static DClient dedicated;
        private string ip;

        public Dictionary<short, List<Func<Packet,bool>>> listeners = new Dictionary<short, List<Func<Packet, bool>>>();

        public void RegisterPacketListener(PacketType type, Func<Packet, bool> action)
        {
            List<Func<Packet, bool>> list;
            if (!listeners.ContainsKey(type.ID))
            {
                list = new List<Func<Packet, bool>>();
                listeners.Add(type.ID, list);
            }
            else
            {
                list = listeners[type.ID];
            }
            list.Add(action);
        }

        public static DClient GetClient()
        {
            if (dedicated != null) return dedicated;

            dedicated = new DClient();
            return dedicated;
        }

        private DClient()
        {
 
        }

        public class StateObject
        {
            // Size of receive buffer.  
            public static int BufferSize = 1024;

            // Receive buffer.  
            public byte[] buffer = new byte[BufferSize];

            // Received data string.
            public byte[] bytes = new byte[BufferSize];

            public int totalRead = 0;


        }

        public ConcurrentQueue<Packet> GetQueue()
        {
            return queue;
        }


        public void Connect(string ip)
        {
            this.ip = ip;
            queue = new ConcurrentQueue<Packet>();
            decoder = new PacketDecoder();
            encoder = new PacketEncoder();
            networkThread = new Thread(StartClient);
            networkThread.IsBackground = true;
            networkThread.Start();
            var type = PacketType.POSITION_PACKET;//load PacketType class
        }

        private async void StartClient()
        {
            // Declare Variables
            //string host = "127.0.0.1";
            string host = ip;
            int port = 25565;
            Debug.Log("connecting to server -> " + host + ": " + port);

            using (client = new TcpClient())
            {
                client.Connect(host, port);
                Debug.Log("connected");

                StateObject state = new StateObject();

                using (stream = client.GetStream())
                {
                    while (running)
                    {
     
                        int bytesRead = await stream.ReadAsync(state.buffer, 0, state.buffer.Length);
                        Recieve(state, bytesRead);
                    }
                    
                        
                    
                }
            }
            Debug.Log(client.Connected);

            Debug.Log("Client connection thread ended");
        }

        public void Recieve(StateObject state, int bytesRead)
        {
            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
             
                for (int i = 0; i < bytesRead; i++)
                {
                    state.bytes[state.totalRead] = state.buffer[i];
                    state.totalRead++;
                }

                int read = ReadPacket(state);
                while (read > 0)
                {
                    for (int i = 0; i < state.totalRead; i++)
                    {
                        state.bytes[i] = state.bytes[read + i];
                    }

                    read = ReadPacket(state);
                }            

            }
        }

        private int ReadPacket(StateObject state)
        {
            PacketType type = null;
            if (state.totalRead >= StateObject.BufferSize)
            {
                Debug.Log("WARNING: NETWORK BUFFER OVERFLOWING");
            }
            if (state.totalRead >= 2)
            {
                short packetId = BitConverter.ToInt16(state.bytes, 0); //read the packet id from the first 2 bytes
                type = Packet.GetPacketFromId(packetId);
            }
            else return 0;

            if (type == null)
            {
                Debug.Log("INVALID PACKET!: " + BitConverter.ToInt16(state.bytes, 0));
                return 0;
            }

            int expectedBytes = type.BYTES;
            if (state.totalRead >= expectedBytes)
            {
                //all expected data recieved, creating packet

                state.totalRead -= expectedBytes;
                if (state.totalRead < 0)
                {
                    Debug.Log("Unexepected negative totalRead from packet");
                    return expectedBytes;
                }
                else if (state.totalRead + expectedBytes > state.bytes.Length)
                {
                    Debug.Log("Too many bytes recieved in packet decoder");
                    return expectedBytes;
                }
                decoder.DecodePacket(state.bytes, type, queue);
                return expectedBytes;

            }
            else
            {
                return 0;
                //still need more data
            }
        }


        public void StopClient()
        {
            running = false;
            client.Close();
            stream.Close();
            Debug.Log("running: " + running);
        }

        public void Write(Packet packet)
        {

            if (client == null)
            {
                Debug.Log("null client");
                return;
            }
            if (!client.Connected)
            {
                Debug.Log("not connected");
                return;
            }
            BinaryWriter writer = new BinaryWriter(stream);
            encoder.Encode(packet, writer);
            writer.Flush();
        }

    }
}
