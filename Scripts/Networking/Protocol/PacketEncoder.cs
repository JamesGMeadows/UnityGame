using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Networking
{
    public class PacketEncoder
    {
    
        public void Encode(Packet packet, BinaryWriter writer)
        {
            writer.Write(packet.GetId());
            packet.Encode(writer);
        }
    }
}
