using ChatServer.Core.Structs;
using System.Net.Sockets;

namespace ChatServer.Core.Network.ConnectionTypes
{
    public class User : Session
    {
        private AccountStruct accountInfo { get; set; }
        private ushort permissions { get; set; }

        public User(Socket remoteConn) : base(remoteConn)
        {
        }

        public void setAccountInfo(AccountStruct accountInfo) 
            => this.accountInfo = accountInfo;

    }
}
