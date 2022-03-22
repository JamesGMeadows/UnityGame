using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Networking.Protocol
{
    class VelocityPacket : Packet
    {
        float x, y, z;
        public VelocityPacket(float x, float y, float z) : base(PacketType.VELOCITY_PACKET)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public VelocityPacket() : this(0, 0, 0)
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
        }

        public override void Encode(BinaryWriter writer)
        {
            writer.Write(x);
            writer.Write(y);
            writer.Write(z);
        }
    }
}
