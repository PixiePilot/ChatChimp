using ChatServer.Core.Encryption;
using ChatServer.Core.Globals;
using ChatServer.Core.Reader.PacketHandlers;
using ChatServer.Core.Structs;
using ChatServer.ServerStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Timers;

namespace ChatServer.Core.Network.ConnectionTypes
{
    public class Session
    {
        private Socket remoteConn { get; set; }
        private string ipAddress { get; set; }
        private ushort port { get; set; }
        private ushort state { get; set; }

        public bool isConnected { get; set; }
        private MonkeyNetworkStream networkStream { get; set; }
        private System.Timers.Timer heartBeatTimer { get; set; }
        private string key { get; set; }
        private byte[] data;

        public Session( Socket remoteConn )
        {
            this.remoteConn = remoteConn;
            if (!isValidConn()) {
                return;
            }
            networkStream = new MonkeyNetworkStream( this.remoteConn );
            string[] ipInfo = remoteConn.RemoteEndPoint.ToString().Split(':');
            
            ipAddress = ipInfo[0];
            port = ushort.Parse( ipInfo[1] );
            state = (ushort)UserStates.PRELOGIN;
            heartBeatTimer = new System.Timers.Timer(Globals.Globals.env.heartBeatTime);
            heartBeatTimer.Elapsed += onHeartBeatElapsed;
            heartBeatTimer.Start();
        }
        ~Session() {
            // save things?
        }
        public void MonkeyWaitBanana() {
            while (!networkStream.DataAvailable) {
                Thread.Sleep(1);
            }
        }
        public MonkeyNetworkStream GetMonkey()
            => networkStream;

        public void ResetMonkey() {
            networkStream.Flush();
        }

        public bool isValidConn() {
            isConnected = Globals.Globals.acceptor.validConn(remoteConn);
            return isConnected;
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

        public void resetTimer() {
            heartBeatTimer.Stop();
            heartBeatTimer.Start();
        }
        public string getIpAddress()
            => ipAddress;

        public int getPort()
            => port;
        private void onHeartBeatElapsed( object? source, ElapsedEventArgs e ) {
            heartBeatTimer.Stop();
            disconnect();
        }

        public void disconnect() {
            Globals.Globals.acceptor.removeUser(this);
        }
    }
}
