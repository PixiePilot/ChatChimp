using ChatServer.Core.Structs;
using ChatServer.ServerStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Core.Network.ConnectionTypes
{
    public class Session
    {
        private Socket remoteConn { get; set; }
        private string ipAddress { get; set; }
        private ushort port { get; set; }

        private ushort state { get; set; }

        private byte[] data;

        public Session( Socket remoteConn )
        {
            this.remoteConn = remoteConn;
            string[] ipInfo = remoteConn.RemoteEndPoint.ToString().Split(':');
            
            ipAddress = ipInfo[0];
            port = ushort.Parse( ipInfo[1] );
            state = (ushort)UserStates.PRELOGIN;
        }

        public Socket getConn() 
            => remoteConn;

        public void setDataSize( int size)
        {
            data = new byte[size];
        }

        public byte[] getData()
            => data;

        public ushort getState()
            => state;

        public void changeToLoggedState( AccountStruct accountInfo )
        {
            User user = (User)this;
            user.setAccountInfo(accountInfo);
        }
    }
}
