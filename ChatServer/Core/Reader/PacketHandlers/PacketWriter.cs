using ChatServer.Core.Encryption;
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

        public void writeByte(byte value)
            => writer.Write(value);

        public void writeString(string message) {
            writeInt(message.Length);
            ms.Write(Encoding.UTF8.GetBytes(message));
        }

        public void writeString(string message, string publicKey) {
            writeInt(message.Length);
            ms.Write(Encoding.UTF8.GetBytes(RC4.apply(message, publicKey)));
        }
    }
}
