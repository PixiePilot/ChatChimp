using ChatServer.Core.Structs;
using System.Net.Sockets;

namespace ChatServer.Core.Network.ConnectionTypes
{
    public class User : Session
    {
        private dynamic accountObject { get; set; }
        private ushort permissions { get; set; }

        public User(Socket remoteConn) : base(remoteConn)
        {
        }

        public void setAccountObject(dynamic accountObject) 
            => this.accountObject = accountObject;

    }
}
