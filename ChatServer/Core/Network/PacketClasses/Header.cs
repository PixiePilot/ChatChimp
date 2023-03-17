using ChatServer.Core.Reader.PacketHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Core.Network.PacketClasses {
    public struct Header {
        public int msgLen { get; set; }
        public int msgId { get; set; }
        public bool validHeader { get; set; }
        public Header( MonkeyNetworkStream MonkeyStream ) {
            try {
                msgLen = MonkeyStream.readIntBytes();
                msgId = MonkeyStream.readIntBytes();
            }catch(Exception) { validHeader = false; }
            validHeader= true;
        }
    }
}
