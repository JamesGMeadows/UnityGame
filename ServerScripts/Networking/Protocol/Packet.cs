using Assets.Scripts.Networking.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public abstract class Packet
    {

        private static List<PacketType> packetList = new List<PacketType>();

        public static List<PacketType> PacketList()
        {
            return packetList;
        }

        public static PacketType GetPacketFromId(short id)
        {
            PacketType packet = null;
            foreach (PacketType type in packetList)
            {
                if (type.ID == id) packet = type;
            }
            if (packet == null)
            {
                Debug.Log("invalid packet id: " + id);
                throw new NullReferenceException("invalid packet id: " + id);
            }
            return packet;
        }


        private readonly short packetId;
        public byte[] buf;

        public Packet(PacketType type)
        {
            this.packetId = type.ID;
        }

        public short GetId()
        {
            return packetId;
        }

        public abstract void Decode(byte[] array, PacketDecoder decoder);

        public abstract void Encode(BinaryWriter writer);


    }

    public class PacketType
    {

        public static readonly PacketType POSITION_PACKET = new PacketType(1, 14);
        public static readonly PacketType VELOCITY_PACKET = new PacketType(2, 14);
        public static readonly PacketType ROTATION_PACKET = new PacketType(3, 14);

        public static readonly PacketType ENTITY_SPAWN_PACKET = new PacketType(4, 22);
        public static readonly PacketType ENTITY_POSITION_PACKET = new PacketType(5, 18);
        public static readonly PacketType ENTITY_VELOCITY_PACKET = new PacketType(6, 18);
        public static readonly PacketType ENTITY_ROTATION_PACKET = new PacketType(7, 18);
        public static readonly PacketType ENTITY_UPDATE_FLOAT_PACKET = new PacketType(8, 12);

        public static readonly PacketType ATTACK_ENTITY_PACKET = new PacketType(9, 10);

        public short ID;
        public int BYTES;

        public PacketType(short id, int bytes)
        {
            this.ID = id;
            this.BYTES = bytes;
            Packet.PacketList().Add(this);
        }

        public Packet InstantiatePacket()
        {
            if (this.Equals(PacketType.POSITION_PACKET))
            {
                return new PositionPacket();
            }
            else if (this.Equals(PacketType.VELOCITY_PACKET))
            {
                return new VelocityPacket();
            }
            else if (this.Equals(PacketType.ROTATION_PACKET))
            {
                return new RotationPacket();
            }
            else if (this.Equals(PacketType.ENTITY_SPAWN_PACKET))
            {
                return new EntitySpawnPacket();
            }
            else if (this.Equals(PacketType.ENTITY_POSITION_PACKET))
            {
                return new EntityPositionPacket();
            }
            else if (this.Equals(PacketType.ENTITY_VELOCITY_PACKET))
            {
                return new EntityVelocityPacket();
            }
            else if (this.Equals(PacketType.ENTITY_ROTATION_PACKET))
            {
                return new EntityRotationPacket();
            }
            else if (this.Equals(PacketType.ENTITY_UPDATE_FLOAT_PACKET))
            {
                return new EntityUpdateFloatPacket();
            }
            else if (this.Equals(PacketType.ATTACK_ENTITY_PACKET))
            {
                return new AttackEntityPacket();
            }
            Debug.Log("invalid packet id: " + ID);
            throw new NullReferenceException("invalid packet id: " + ID);
        }
    }

}
