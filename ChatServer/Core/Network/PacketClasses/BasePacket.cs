using ChatServer.Core.Encryption;
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

        protected PacketWriter writer { get; set; }

        public BasePacket( int size ) 
        { // requires creation of packet writer
            data = new byte[size];
            ms = new MemoryStream( data );
        }

        public BasePacket( PacketReader reader )
        {

        }

        public void createStream( int size )
        {
            data = new byte[size];
            writer = new PacketWriter( data );
        }

        public void createHeader(int size, int packetId)
        {
            writer.writeInt( size - 2);
            writer.writeInt( packetId );
        }

        public void writeInt( int value )
        {
            writer.writeInt( value );
        }

        public void writeByte( byte b ) 
            => ms.WriteByte( b );
        
        public void writeString( string message )
        {
            writeInt( message.Length );
            ms.Write( Encoding.UTF8.GetBytes(message) );
        }

        public void writeString( string message, string publicKey)
        {
            writeInt( message.Length );
            ms.Write( Encoding.UTF8.GetBytes( RC4.apply(message,publicKey) ) );
        }

        public byte[] getData() 
            => data;
    }
}
