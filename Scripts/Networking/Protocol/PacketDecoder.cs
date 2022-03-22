using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Networking
{

    public class PacketDecoder
    {
        public static int MIN_PACKET_SIZE = 2;
        private static int index = 0;

        public void DecodePacket(byte[] bytes, PacketType type, ConcurrentQueue<Packet> queue)
        {
            index = 2;
            Packet packet = type.InstantiatePacket();
            packet.Decode(bytes, this);
            queue.Enqueue(packet);
        }

        public short NextShort(byte[] buffer)
        {
            short s = BitConverter.ToInt16(buffer, index);
            index += 2;
            return s;
        }

        public float NextFloat(byte[] buffer)
        {
            float f = BitConverter.ToSingle(buffer, index);
            index += 4;
            return f;
        }
        public int NextInt(byte[] buffer)
        {
            int i = BitConverter.ToInt32(buffer, index);
            index += 4;
            return i;
        }
    }
}
