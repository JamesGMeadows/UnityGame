using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public class RotationPacket : Packet
    {
        float x, y, z;
        public RotationPacket(float x, float y, float z) : base(PacketType.ROTATION_PACKET)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public RotationPacket(Vector3 vec) : this(vec.x, vec.y, vec.z)
        {

        }

        public RotationPacket() : this(0, 0, 0)
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
            x = decoder.NextFloat(array);
            y = decoder.NextFloat(array);
            z = decoder.NextFloat(array);
            //Debug.Log("Decoded position packet: " + x + " " + y + " " + z);
        }

        public override void Encode(BinaryWriter writer)
        {
            writer.Write(x);
            writer.Write(y);
            writer.Write(z);
        }
    }
}
