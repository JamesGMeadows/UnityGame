using System;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public class DServer
    {

        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private static readonly ClientRegistry registry = new ClientRegistry();

        public class StateObject
        {
            // Size of receive buffer.  
            public const int BufferSize = 1024;

            // Receive buffer.  
            public byte[] buffer = new byte[BufferSize];

            // Received data string.
            public byte[] bytes = new byte[BufferSize];

            public int totalRead = 0;

            // Client socket.
            public ClientConnection connection;
        }

        private static Thread networkListeningThread;
        public DServer()
        {
            networkListeningThread = new Thread(DServer.StartListening);
            networkListeningThread.IsBackground = true;
            networkListeningThread.Start();
            var type = PacketType.POSITION_PACKET;//load PacketType class
        }

        public static ClientRegistry GetClients()
        {
            return registry;
        }
        private static Socket listener;
        private static volatile bool running = true;
        public static bool local = true;
        public static void StartListening()
        {
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 25565);

            // Create a TCP/IP socket.  
            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            if (local)
            {
                listener.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 25565));
            } else listener.Bind(localEndPoint);

            Debug.Log("Server Socket Created");
            listener.Listen(20);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {

                while (running)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Debug.Log("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback),listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }

            Debug.Log("Server listener stopped");

        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            ClientConnection connection = new ClientConnection(handler);
            registry.Connect(connection);
            Debug.Log("Client Connected");

            // Create the state object.  
            StateObject state = new StateObject();
            state.connection = connection;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.connection.GetSocket();

            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

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

                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
        }

        private static int ReadPacket(StateObject state)
        {
            PacketType type = null;
            if (state.totalRead >= StateObject.BufferSize)
            {
                Debug.Log("WARNING: NETWORK BUFFER OVERFLOWING");
            }
            if (state.totalRead >= 2)
            {
                short packetId = BitConverter.ToInt16(state.bytes, 0);
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
                PacketDecoder decoder = state.connection.GetDecoder();
                decoder.DecodePacket(state.bytes, type, state.connection.GetQueue());
                return expectedBytes;

            }
            else
            {
                return 0;
                //still need more data
            }
        }



        public static void Shutdown()
        {
            running = false;
            listener.Close();
            
        }
    }
}
