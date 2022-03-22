using Assets.Scripts.Networking;
using Assets.Scripts.Networking.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{

    public GameObject playerPrefab;
    public GameObject zombiePrefab;
    public GameObject doorPrefab;
    void Start()
    {
        DClient.GetClient().RegisterPacketListener(PacketType.ENTITY_SPAWN_PACKET, GetEntitySpawn());
    }

    //map ids to prefabs
    private GameObject GetPrefabFromId(int id)
    {
        if (id == 0)
        {
            return playerPrefab;
        }
        else if (id == 1)
        {
            return zombiePrefab;
        }
        else return doorPrefab;
    }

    //register entity spawn packet listener
    private Func<Packet, bool> GetEntitySpawn()
    {

        Func<Packet, bool> listener = delegate (Packet packet)
        {
            EntitySpawnPacket spawn = packet as EntitySpawnPacket;
            GameObject entity = Instantiate(GetPrefabFromId(spawn.GetEntityType()), transform.position, Quaternion.identity);
            entity.GetComponent<NetworkEntity>().SetNetworkId(spawn.GetNetworkId());
            return true;
        };
        return listener;
    }

    void Update()
    {
        
    }
}
