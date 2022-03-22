using Assets.Scripts;
using Assets.Scripts.Networking;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class ClientReciever : MonoBehaviour
{
    public GameObject clientPrefab;
    private Timer pollTimer;
    private ConcurrentQueue<ClientConnection> clients;

    void Start()
    {
        pollTimer = new Timer(1);
        clients = DServer.GetClients().GetRegistry();
    }

    void Update()
    {
        if (pollTimer.CheckFinished(false))
        {
            pollTimer.NextTime();
            while (clients.TryDequeue(out ClientConnection connection))//Create clients from waiting queue
            {
                GameObject client = Instantiate(clientPrefab, transform.position, Quaternion.identity);
                ClientHandler handler = client.GetComponent<ClientHandler>();
                handler.SetConnection(connection);
            }
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        DServer.Shutdown();
    }
}
