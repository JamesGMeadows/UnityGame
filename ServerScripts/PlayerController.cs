using Assets.Scripts.Networking;
using Assets.Scripts.Networking.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Rigidbody controller;
    private ClientHandler client;

    void Start()
    {
        controller = GetComponent<Rigidbody>();
        client = GetComponent<ClientHandler>();
        client.RegisterPacketListener(PacketType.ATTACK_ENTITY_PACKET, GetAttackEntity());
        client.RegisterPacketListener(PacketType.POSITION_PACKET, GetPosition());
        client.RegisterPacketListener(PacketType.ROTATION_PACKET, GetRotation());
    }

    private Action<Packet> GetAttackEntity()
    {
        Action<Packet> listener = (packet) =>
        {
            AttackEntityPacket attack = packet as AttackEntityPacket;
            int networkId = attack.GetNetworkId();
            int collider = attack.GetCollider();
            this.AttackEntity(networkId, collider);
        };
        return listener;
    }

    private void AttackEntity(int entityNetId, int colliderHit)
    {
        Damagable[] damagables = GameObject.FindObjectsOfType<Damagable>();
        foreach (Damagable check in damagables)
        {
            if (check.GetNetwork().GetNetworkId() == entityNetId)
            {
                check.Damage(50);
            }
        }
    }


    private Vector3 position;

    //register position packet listener
    private Action<Packet> GetPosition()
    {
        Action<Packet> listener = (packet) =>
        {
            PositionPacket pos = packet as PositionPacket;
            position = new Vector3(pos.GetX(), pos.GetY(), pos.GetZ());
        };
        return listener;
    }

    private Vector3 rotation;

    //register rotation packet listener
    private Action<Packet> GetRotation()
    {
        Action<Packet> listener = (packet) =>
        {
            RotationPacket rot = packet as RotationPacket;
            rotation = new Vector3(rot.GetX(), rot.GetY(), rot.GetZ());
        };
        return listener;
    }
    private void InterpolateMovement()
    {
        if (position == null) return;
        Vector3 currentPos = controller.transform.position;
        controller.transform.position = position;
        if (Vector3.Distance(currentPos, position) > 5) //if the player position differs too much teleport them to the correct spot
        {
            controller.transform.position = position;
        }
        else
        {
            //controller.transform.position = Vector3.MoveTowards(currentPos, position, (Vector3.Distance(currentPos, position) / (1f / 3.33f) * Time.deltaTime));
            //controller.velocity = direction;
        }
        if (rotation != null)
        {
            transform.eulerAngles = rotation;
        }
    }


    void Update()
    {
        InterpolateMovement();
    }
}
