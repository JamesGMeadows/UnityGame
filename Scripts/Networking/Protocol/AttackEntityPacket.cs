using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public class AttackEntityPacket : Packet
    {
        int network_id, collider;
        public AttackEntityPacket(int network_id, int collider) : base(PacketType.ATTACK_ENTITY_PACKET)
        {
            this.network_id = network_id;
            this.collider = collider;
        }

        public AttackEntityPacket() : this(-1, -1)
        {

        }
            
        public int GetNetworkId()
        {
            return network_id;
        }

        public int GetCollider()
        {
            return collider;
        }

        public override void Decode(byte[] array, PacketDecoder decoder)
        {
            network_id = decoder.NextInt(array);
            collider = decoder.NextInt(array);
        }

        public override void Encode(BinaryWriter writer)
        {
            writer.Write(network_id);
            writer.Write(collider);
        }
    }
}