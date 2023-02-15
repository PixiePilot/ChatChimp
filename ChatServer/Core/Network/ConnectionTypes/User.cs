using ChatServer.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
