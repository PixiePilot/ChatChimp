using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Core.Network.PacketClasses
{
    public class InitPacket : BasePacket
    {
        int packetSize { get; set; }

        public InitPacket( int size = 7  ) : base(size) { packetSize = size; } // to be reworked to use packet writer

        /// <summary>
        /// ResponseState 0 = no issues, ReponseState 1 = full, ResponseState 2 = Banned
        /// </summary>
        /// <param name="responseState"></param>
        /// <returns>Data to send</returns>
        public byte[] createResponse( byte responseState )
        {
            createStream( packetSize );
            createHeader( packetSize, (int)NetMessage.TS_SC_RESPONSE_INIT );
            writeByte( responseState );
            return getData();
        }
    }
}
