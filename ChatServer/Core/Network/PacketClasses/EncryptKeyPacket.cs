using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Core.Network.PacketClasses
{
    public class EncryptKeyPacket : BasePacket
    {
        public EncryptKeyPacket( int size, string publicKey ) : base(size)
        {
            createStream(size);
            createHeader( publicKey.Length , (int)NetMessage.TS_SC_GIVE_KEY );
            writeString(publicKey);
        }
    }
}
