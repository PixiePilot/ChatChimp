using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Core.Reader.PacketHandlers
{
    public class PacketReader
    {
        private MemoryStream ms { get; set; }
        private BinaryReader reader { get; set; }

        public PacketReader( byte[] data ) 
        {
            ms = new MemoryStream( data );
            reader = new BinaryReader( ms );
        }

        public int readIntBytes()
            => reader.Read7BitEncodedInt();
        
        public string readString( int lenMsg )
            => Encoding.UTF8.GetString(reader.ReadBytes(lenMsg + 1),1,lenMsg);
            

    }
}
