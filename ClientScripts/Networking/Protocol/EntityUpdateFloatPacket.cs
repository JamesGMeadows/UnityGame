using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Networking.Protocol
{
    public class EntityUpdateFloatPacket : Packet
    {
        int network_id;
        short status_code;
        float data;
        public EntityUpdateFloatPacket(int network_id, short status_code, float data) : base(PacketType.ENTITY_UPDATE_FLOAT_PACKET)
        {
            this.network_id = network_id;
            this.status_code = status_code;
            this.data = data;
        }

        public EntityUpdateFloatPacket() : this(-1, -1, 0)
        {

        }

        public int GetNetworkId()
        {
            return network_id;
        }

        public short GetStatusCode()
        {
            return status_code;
        }

        public float GetData()
        {
            return data;
        }

        public override void Decode(byte[] array, PacketDecoder decoder)
        {
            network_id = decoder.NextInt(array);
            status_code = decoder.NextShort(array);
            data = decoder.NextFloat(array);
        }

        public override void Encode(BinaryWriter writer)
        {
            writer.Write(network_id);
            writer.Write(status_code);
            writer.Write(data);
        }
    }
}
