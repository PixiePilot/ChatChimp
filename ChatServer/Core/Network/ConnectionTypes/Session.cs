using ChatServer.Core.Encryption;
using ChatServer.Core.Globals;
using ChatServer.Core.Structs;
using ChatServer.ServerStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace ChatServer.Core.Network.ConnectionTypes
{
    public class Session
    {
        private Socket remoteConn { get; set; }
        private string ipAddress { get; set; }
        private ushort port { get; set; }
        private ushort state { get; set; }

        private string key { get; set; }
        private byte[] data;

        public Session( Socket remoteConn )
        {
            this.remoteConn = remoteConn;
            string[] ipInfo = remoteConn.RemoteEndPoint.ToString().Split(':');
            
            ipAddress = ipInfo[0];
            port = ushort.Parse( ipInfo[1] );
            state = (ushort)UserStates.CREATE_KEY;
        }

        public void setKey( string publicKey )
        {
            key = publicKey;
        }

        public string getKey()
            => key;

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

        public void setState( ushort state )
            => this.state = state;

        public void changeToLoggedState( AccountStruct accountInfo )
        {
            User user = (User)this;
            user.setAccountInfo(accountInfo);
        }
    }
}
