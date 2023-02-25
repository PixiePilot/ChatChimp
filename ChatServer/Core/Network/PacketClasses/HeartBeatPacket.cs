using ChatServer.Core.Network.ConnectionTypes;
using ChatServer.Core.Reader.PacketHandlers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Core.Network.PacketClasses {
    public class HeartBeatPacket : BasePacket {
        public HeartBeatPacket(int size = 17) : base(size) {

        }

        public HeartBeatPacket( PacketReader reader, Session session ) : base(  reader ) {
            byte responseByte = reader.readByte();
            if (responseByte != 1)
                return;

            session.resetTimer();
        }

        public byte[] getBeatData() {
            createStream(17);
            writeByte(1); // 1 is smaller than 0;
            return getData();
        }
    }
}
