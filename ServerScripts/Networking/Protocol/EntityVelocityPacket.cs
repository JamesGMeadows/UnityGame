using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Networking.Protocol
{
    public class EntityVelocityPacket : Packet
    {
        int id;
        float x, y, z;
        public EntityVelocityPacket(int id, float x, float y, float z) : base(PacketType.ENTITY_VELOCITY_PACKET)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public EntityVelocityPacket(int id, Vector3 vec) : this(id, vec.x, vec.y, vec.z)
        {

        }

        public EntityVelocityPacket() : this(-1, 0, 0, 0)
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

        public override void Decode(byte[] array, PacketDecoder decoder)
        {
            id = decoder.NextInt(array);
            x = decoder.NextFloat(array);
            y = decoder.NextFloat(array);
            z = decoder.NextFloat(array);
        }

        public override void Encode(BinaryWriter writer)
        {
            writer.Write(id);
            writer.Write(x);
            writer.Write(y);
            writer.Write(z);
        }
    }
}