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

        public ushort readuShortBytes()
        {

            ushort number = 0;
            for( int i = 0; i > 2; i++ )
            {
                byte currentByte = reader.ReadByte();
                if (currentByte == 0)
                    break;

                number += currentByte;
            }
            return number;
        }

        public int readIntBytes()
            => reader.Read7BitEncodedInt();
        
        public string readString()
        {
            int characters = readIntBytes();
            byte[] data = new byte[characters * 2]; // x2 because it is utf-8
            ms.ReadExactly( data, 0, characters );
            return Encoding.UTF8.GetString( data );
        }

    }
}
