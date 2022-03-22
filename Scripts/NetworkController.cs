using Assets;
using Assets.Scripts.Networking;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : MonoBehaviour
{

    private DClient client;
    public string ip;

    void Start()
    {
        client = DClient.GetClient();
        client.Connect(ip);
    }

    private LinkedList<Packet> reQueue = new LinkedList<Packet>();
    void Update()
    {
        ConcurrentQueue<Packet> queue = client.GetQueue();

        

        Packet packet;

        while (client.GetQueue().TryDequeue(out packet))
        {
            bool success = false;
            if (DClient.GetClient().listeners.ContainsKey(packet.GetId())) { 
                foreach (Func<Packet, bool> action in DClient.GetClient().listeners[packet.GetId()]) //could use a better solution here later
                {
                    if (action(packet))
                    {
                        success = true; //the packet was succsessfully processed by the listener
                    }
                }
            }
            if (!success)
            {
                Debug.Log("Re-Enqueuing packet: " + packet.GetId());
                reQueue.AddFirst(packet); //re enqueue packet so that no packets are dropped
            }
        }
        foreach (Packet p in reQueue)
        {
            queue.Enqueue(p);
        }
        reQueue.Clear();
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        client.StopClient();
    }
}
