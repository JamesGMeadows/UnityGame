using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Networking.Protocol
{
    public class EntitySpawnPacket : Packet
    {
        int id, type;
        float x, y, z;
        public EntitySpawnPacket(int id, int type, float x, float y, float z) : base(PacketType.ENTITY_SPAWN_PACKET)
        {
            this.id = id;
            this.type = type;
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public EntitySpawnPacket(int id, int type, Vector3 vec) : this(id, type, vec.x, vec.y, vec.z)
        {

        }

        public EntitySpawnPacket() : this(-1, -1, 0, 0, 0)
        {

        }

        public float GetX()
        {
            return x;
        }

        public float GetY()
        {
            return y;
        }

        public float GetZ()
        {
            return z;
        }

        public int GetNetworkId()
        {
            return id;
        }

        public int GetEntityType()
        {
            return type;
        }

        public override void Decode(byte[] array, PacketDecoder decoder)
        {
            id = decoder.NextInt(array);
            type = decoder.NextInt(array);
            x = decoder.NextFloat(array);
            y = decoder.NextFloat(array);
            z = decoder.NextFloat(array);
        }

        public override void Encode(BinaryWriter writer)
        {
            writer.Write(id);
            writer.Write(type);
            writer.Write(x);
            writer.Write(y);
            writer.Write(z);
        }
    }
}
