using Assets.Scripts.Networking;
using Assets.Scripts.Networking.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkEntity : MonoBehaviour
{


    int networkId = 0;
    private Dictionary<short, float> currentStatus; //maps the entity status codes to their float values

    void Start()
    {
        currentStatus = new Dictionary<short, float>();
        DClient.GetClient().RegisterPacketListener(PacketType.ENTITY_POSITION_PACKET, GetEntityPosition());
        DClient.GetClient().RegisterPacketListener(PacketType.ENTITY_VELOCITY_PACKET, GetEntityVelocity());
        DClient.GetClient().RegisterPacketListener(PacketType.ENTITY_ROTATION_PACKET, GetEntityRotation());
        DClient.GetClient().RegisterPacketListener(PacketType.ENTITY_UPDATE_FLOAT_PACKET, GetUpdateFloat());
    }

    public int GetNetworkId()
    {
        return networkId;
    }

    public void SetNetworkId(int id)
    {
        this.networkId = id;
    }

    public Dictionary<short, float> GetStatus()
    {
        return currentStatus;
    }

    private Func<Packet, bool> GetUpdateFloat()
    {

        Func<Packet, bool> listener = delegate (Packet packet)
        {
            EntityUpdateFloatPacket update = packet as EntityUpdateFloatPacket;
            if (update.GetNetworkId() != networkId) return false;
            currentStatus[update.GetStatusCode()] = update.GetData();
            return true;
        };
        return listener;
   
    }

    private Vector3 velocity;
    private Func<Packet, bool> GetEntityVelocity()
    {
        Func<Packet, bool> listener = delegate (Packet packet)
        {
            EntityVelocityPacket vel = packet as EntityVelocityPacket;
            if (vel.GetEntityId() != networkId) return false;
            velocity = new Vector3(vel.GetX(), vel.GetY(), vel.GetZ());

            return true;
        };
        return listener;
    }

    private Vector3 position;

    private Func<Packet, bool> GetEntityPosition()
    {
        Func<Packet, bool> listener = delegate (Packet packet)
        {
            EntityPositionPacket pos = packet as EntityPositionPacket;
            if (pos.GetEntityId() != networkId) return false;
            if (pos.GetX() == position.x && pos.GetZ() == position.z && pos.GetY() == position.y)
            {
                return true;
            }
            position = new Vector3(pos.GetX(), pos.GetY(), pos.GetZ());
            return true;
        };
        return listener;
    }

    private Quaternion rotation;

    private Func<Packet, bool> GetEntityRotation()
    {

        Func<Packet, bool> listener = delegate (Packet packet)
        {
            EntityRotationPacket rot = packet as EntityRotationPacket;
            if (rot.GetEntityId() != networkId) return false;
            if (rot.GetX() == rotation.x && rot.GetZ() == rotation.z && rot.GetY() == rotation.y)
            {
                return true;
            }
            rotation = Quaternion.Euler(rot.GetX(), rot.GetY(), rot.GetZ());
            return true;
        };
        return listener;
    }

    //Interpolate movement so that it is smooth over the network
    private void InterpolateMovement()
    {
        if (position == null) return;
        Vector3 currentPos = transform.position;

        if (Vector3.Distance(currentPos, position) > 5)
        {
            transform.position = position;
        }
        else
        {
            transform.position = Vector3.MoveTowards(currentPos, position, (Vector3.Distance(currentPos, position) / (1f / 5f) * Time.deltaTime));//fix later
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, .05f);

        this.CalculateVelocity();
    }

    Vector3 previousPos;
    float v;
    private void CalculateVelocity()
    {
        if (previousPos != null)
        {
            v = ((transform.position - previousPos).magnitude) / Time.deltaTime;
        }
        previousPos = transform.position;
    }

    public float GetVelocity()
    {
        return v;
    }

    // Update is called once per frame
    void Update()
    {
        InterpolateMovement();
    }
}
