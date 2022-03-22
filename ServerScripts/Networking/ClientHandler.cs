using Assets.Scripts.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientHandler : MonoBehaviour
{

    private ClientConnection connection;
    private Dictionary<short, Action<Packet>> listeners = new Dictionary<short, Action<Packet>>();

    private int clientId = 0;
    private static int clientCount = 0;


    void Start()
    {
        this.clientId = clientCount;
        Debug.Log("Assigned client id: " + clientId);
        clientCount++;

    }

    public int GetClientId()
    {
        return clientId;
    }

    public void RegisterPacketListener(PacketType type, Action<Packet> action)
    {
        listeners.Add(type.ID, action);
    }

    public void SetConnection(ClientConnection connection)
    {
        this.connection = connection;
    }

    public void SendPacket(Packet packet)
    {
        connection.SendPacket(packet);
    }

    // Update is called once per frame
    void Update()
    {
        if (connection == null)
        {
            Debug.Log("invalid client connection");
            return;
        }
        Packet packet;

        while (connection.GetQueue().TryDequeue(out packet))
        {
            listeners[packet.GetId()](packet);
        }
    }
}
