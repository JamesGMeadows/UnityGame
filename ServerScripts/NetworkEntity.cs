using Assets.Scripts;
using Assets.Scripts.Networking;
using Assets.Scripts.Networking.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkEntity : MonoBehaviour
{
    public int entityPrefabId = -1;
    public bool updateMovement = false;
    private int networkId = 0;
    private static int networkIdCount = 0;
    private Dictionary<int, ClientData> clientMap;
    private int clientId = -1;
    private Timer pollTimer;

    public void Start()
    {
        pollTimer = new Timer(.1f);
        networkId = networkIdCount;
        networkIdCount++;

        clientMap = new Dictionary<int, ClientData>();

        ClientHandler client = GetComponent<ClientHandler>();
        if (client != null)
        {
            clientId = client.GetClientId();
        }
    }

    public int GetNetworkId()
    {
        return networkId;
    }

    private Vector3 lastPos;
    private Vector3 lastRot;
    public void Update()
    {

        if (pollTimer.CheckFinished(false))
        {
            pollTimer.NextTime();
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            CheckClientIntegrity(players);
            if (updateMovement)
            {
                Broadcast3DLoc(players);
            }
            CheckStatusUpdates();
        } 
        
    }

    //Ensures that the NetworkEntity is brodcasting to all clients.
    private void CheckClientIntegrity(GameObject[] players)
    {
        foreach (GameObject player in players)
        {
            ClientHandler client = player.GetComponent<ClientHandler>();
            if (client.GetClientId() == clientId) continue;
            if (!clientMap.ContainsKey(client.GetClientId()))
            {
                lastPos = Vector3.zero;
                lastRot = Vector3.zero;
                Broadcast3DLoc(new GameObject[] { player });
            }
        }
    }
    /* */
    private void Broadcast3DLoc(GameObject[] players)
    {
        if (!lastPos.Equals(transform.position))
        {
            BroadcastPacket(new EntityPositionPacket(networkId, transform.position), players);
            lastPos = transform.position;
        }
        Vector3 rot = transform.eulerAngles;
        if (!lastRot.Equals(rot))
        {
            BroadcastPacket(new EntityRotationPacket(networkId, rot), players);
            lastRot = rot;
        }
    }

    private void CheckStatusUpdates()
    {
        foreach (var client in clientMap)
        {
            var statusSent = client.Value.statusSent;
            foreach (var item in client.Value.currentStatus)
            {
                short code = item.Key;
                float data = item.Value;
                if (statusSent.ContainsKey(code))
                {
                    if (statusSent[code] == data)
                    {
                        continue; //already up to date, avoids sending again
                    }
                }
                else statusSent.Add(code, data);

                statusSent[code] = data;
                SendPacket(new EntityUpdateFloatPacket(this.networkId, code, data), client.Value.client, true);
            }
        }
    }

    public void UpdateStatus(short code, float data)
    {
        foreach (var client in clientMap)
        {
            client.Value.currentStatus[code] = data;
        }
    }


    public void ForceUpdateStatus(short code, float data)
    {
        foreach (var client in clientMap)
        {
            var clientData = client.Value;
            clientData.statusSent[code] = data;
            SendPacket(new EntityUpdateFloatPacket(this.networkId, code, data), clientData.client, true);
        }
    }

    public void BroadcastPacket(Packet packet)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        BroadcastPacket(packet, players);
    }

    public void BroadcastPacket(Packet packet, GameObject[] players)
    {
        foreach (GameObject player in players)
        {
            ClientHandler client = player.GetComponent<ClientHandler>();
            SendPacket(packet, client, false);
        }
    }

    public void SendPacket(Packet packet, ClientHandler client, bool includeThisClient)
    {

        if (clientId == client.GetClientId())//avoid updating client to client
        {
            if (!includeThisClient) return;
        }
        else if (!clientMap.ContainsKey(client.GetClientId()))
        {
            client.SendPacket(new EntitySpawnPacket(networkId, this.entityPrefabId, transform.position));
            clientMap.Add(client.GetClientId(), new ClientData(client));//need to remove this object when player leaves TODO
        }

        client.SendPacket(packet);
        
    }

    public class ClientData
    {
        public Dictionary<short, float> currentStatus = new Dictionary<short, float>();
        public Dictionary<short, float> statusSent = new Dictionary<short, float>();
        public ClientHandler client;
        public ClientData(ClientHandler client)
        {
            this.client = client;
        }
    }
}
