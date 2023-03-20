using System.Net.Sockets;
using System.Text;

namespace ChatServer.Core.Reader.PacketHandlers
{
    public class MonkeyNetworkStream : NetworkStream
    {
        public MonkeyNetworkStream(Socket socket) : base(socket) {
        }
        
        public int readIntBytes() 
            => new BinaryReader(this).Read7BitEncodedInt();
        

        public string readString() {
            BinaryReader reader = new BinaryReader(this);
            int msglen = reader.Read7BitEncodedInt();
            return Encoding.UTF8.GetString(reader.ReadBytes(msglen + 1), 1, msglen);
        }

        public void Seek(int offset) {
            for( int i = 0; i == offset; offset--)
                ReadByte();
        }

        public byte readByte()
            => (byte)ReadByte();
        public void writerHeader(int packetId, int size) {
            BinaryWriter writer = new BinaryWriter(this);
            writer.Write7BitEncodedInt(size);
            writer.Write7BitEncodedInt(packetId);
        }
        public void writeBool(bool b) 
            => new BinaryWriter(this).Write(b);
        

        public void writeInt(int value) 
            => new BinaryWriter(this).Write7BitEncodedInt(value);
        
        public void writeString(string value) 
            => new BinaryWriter(this).Write(value);
        
        
    }
}
