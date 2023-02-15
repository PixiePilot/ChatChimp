using ChatServer.Core.Reader.PacketHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Core.Network.PacketClasses
{
    public class BasePacket
    {
        protected MemoryStream ms { get; set; }

        protected byte[] data { get; set; }

        public BasePacket( int size ) 
        {
            data = new byte[size];
            ms = new MemoryStream( data );
        }

        public BasePacket( PacketReader reader )
        {

        }

        public void createStream( int size )
        {
            data = new byte[size];
            ms = new MemoryStream(data);
        }

        public void createHeader(int size, byte packetId)
        {
            ms.WriteByte( (byte)(size - 1) );
            ms.WriteByte(0);
            ms.WriteByte( packetId );
            ms.WriteByte(0);
        }

        public void writeIntByte( int value )
        {
            ms.WriteByte((byte)value);
        }

        public void writeByte( byte b ) 
            => ms.WriteByte( b );
        
        public void writeString( string message)
        {
            writeIntByte( message.Length );
            ms.Write( Encoding.UTF8.GetBytes(message) );
        }

        public byte[] getData() 
            => data;
    }
}
