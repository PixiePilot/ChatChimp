using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChatServer.Core.Reader.PacketHandlers
{
    public class PacketWriter
    {
        private MemoryStream ms { get; set; }
        private BinaryWriter writer { get; set; }

        public PacketWriter(byte[] data ) {
            ms = new MemoryStream(data);
            writer = new BinaryWriter(ms);
        }

        public void writeInt( int value )
        {
            writer.Write7BitEncodedInt( value );
        }
    }
}
