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
        public Header( PacketReader reader) {
            try {
                msgLen = reader.readIntBytes();
                msgId = reader.readIntBytes();
            }catch(Exception) { validHeader = false; }
            validHeader= true;
        }
    }
}
