using System.Net;
using System.Net.Sockets;
using ChatServer.Core.Globals;
using ChatServer.Core;
using ChatServer.Core.Network.PacketClasses;
using ChatServer.Core.Network.ConnectionTypes;
using ChatServer.Core.Network;
using ChatServer.ServerStates;
using ChatServer.Core.Reader.PacketHandlers;
using ChatServer.Core.Encryption;

namespace ChatServer
{
    public class IoAcceptor
    {
        private bool isAlive = true;
        private List<Session> connectedSessions = new List<Session>();
        #region Servers
        private Socket serverSocket { get; set; }

        private ServerChat chatServer { get; set; }

        private Authentication authentication { get; set; }
        #endregion

        public IoAcceptor()
        {
            authentication  = Globals.authentication;
            chatServer      = Globals.chatServer;
            #region initSocket
            string Ipaddress = Globals.env.ipAddress;
            int port = Globals.env.port;
            IPAddress address = IPAddress.Parse( Ipaddress );
            IPEndPoint localEndPoint = new IPEndPoint( address, port );
            #endregion
            serverSocket = new Socket( address.AddressFamily, SocketType.Stream, ProtocolType.Tcp );
            serverSocket.Bind( localEndPoint );
        }

        public void start()
        {
            serverSocket.Listen(600);
            GuiHandler.writeEvent("Server started");
            while( isAlive)
            {
                handleNewSocket( serverSocket.Accept() );
            }
        }

        public void handleNewSocket( Socket remoteSocket )
        {
            new Thread(() =>
            {
                if (10 >= Globals.env.maxUsers) { denyUser(remoteSocket); } // need to create a connection list still.
                else { acceptUser(remoteSocket); }
            }).Start();
        }
        public void denyUser( Socket remoteConn )
        {
            #region socketSettings
            remoteConn.ReceiveTimeout = Globals.env.maxPing;
            remoteConn.SendTimeout = Globals.env.maxPing;
            #endregion
            InitPacket packet = new InitPacket();
            remoteConn.Send(packet.createResponse(1));
            remoteConn.Shutdown( SocketShutdown.Send );
            remoteConn.Close();
        }

        public void acceptUser( Socket remoteConn )
        {
            #region socketSettings
            remoteConn.ReceiveTimeout = Globals.env.maxPing;
            remoteConn.SendTimeout = Globals.env.maxPing;
            #endregion
            InitPacket packet = new InitPacket();
            remoteConn.Send( packet.createResponse(0) );
            Session session = new User( remoteConn );
            connectedSessions.Add( session );
            GuiHandler.writeEvent( "Remote connection opened at: " + remoteConn.RemoteEndPoint!.ToString() );
            new Thread( () => listenForMessage( session ) ).Start();
        }

        public void removeUser( Session session )
        {
            GuiHandler.writeEvent("Remote connection closed at: " + session.getIpAddress());
            connectedSessions.RemoveAll( conn => ( session.getIpAddress() == conn.getIpAddress() ) && ( session.getPort() == conn.getPort() ) );
            if (!validConn(session.getConn())) {
                return;
            }
            
            try { session.getConn().Shutdown(SocketShutdown.Send);  }
            finally { session.getConn().Close(); }
        }

        public bool validConn( Socket remoteConn )
        {
            try
            {
                return !( remoteConn.Poll( 1, SelectMode.SelectRead ) && remoteConn.Available == 0 );
            }
            catch (Exception) { return false; }
        }

        public void listenForMessage( Session connection ) 
        {
            Socket remoteConn = connection.getConn();
            #region validSocket check
            if ( !validConn(remoteConn) )
            {
                connection.setState( (ushort)UserStates.DISCONNECTED );
                removeUser(connection);
                return;
            }
            #endregion
            switch ( connection.getState() )
            {
                // new state for version check.
                case (ushort)UserStates.CREATE_KEY:
                    connection.setDataSize( (int)NetSizes.CREATE_KEY );
                    break;
                case (ushort)UserStates.PRELOGIN:
                    connection.setDataSize( (int)NetSizes.PRELOGIN );
                    break;
                case (ushort)UserStates.LOGGEDIN:
                    connection.setDataSize( (int)NetSizes.LOGGEDIN );
                    break;
            }


            connection.ResetMonkey();
            bool result = connection.MonkeyWaitBanana();
            if( result )
                receiveBananas(connection);
            else {
                removeUser(connection);
            }
        }

        public void receiveBananas( Session connection )
        {
            #region prepare for handling
            MonkeyNetworkStream MonkeyStream = connection.GetMonkey();
            Header header = new Header(MonkeyStream);
            #endregion
            switch(header.msgId) { // default packets that work across all states
                case (int)NetMessage.TS_CS_HEARTBEAT:
                    doHeartBeat(connection, MonkeyStream);
                    break;
                default:
                    handleMessage(connection, header, MonkeyStream); // handles state based messages
                    break;
            }

            listenForMessage( connection );
        }

        public void handleMessage( Session connection, Header header, MonkeyNetworkStream MonkeyStream ) {
            switch (connection.getState()) {
                case (ushort)UserStates.CREATE_KEY:
                    setKey( connection, header, MonkeyStream );
                    break;
                case (ushort)UserStates.PRELOGIN:
                    authentication.handleMessage( connection, header, MonkeyStream );
                    break;
                case (ushort)UserStates.LOGGEDIN:
                    chatServer.handleMessage( (User)connection, header, MonkeyStream );
                    break;
            }
        }

        public void doHeartBeat( Session connection, MonkeyNetworkStream reader ) {
            HeartBeatPacket heartBeatPacket = new HeartBeatPacket( reader, connection );
            connection.getConn().Send( heartBeatPacket.getBeatData( (byte)connection.getState() ) );
        }

        public void setKey( Session connection, Header header, MonkeyNetworkStream reader )
        {
            string clientKey = reader.readString();
            string publicKey;
            bool result = createKey(clientKey, out publicKey);

            if ( !result || header.msgId != 9999 )
            {
                removeUser(connection);
                return;
            }

            connection.setKey(publicKey);
            connection.setState((ushort)UserStates.PRELOGIN);
            EncryptKeyPacket packet = new EncryptKeyPacket( (int)NetSizes.CREATE_KEY, connection.getKey() );
            connection.getConn().Send( packet.getData() );
        }

        public bool createKey( string clientKey, out string publicKey )
        {
            publicKey = string.Empty;
            if (clientKey.Length != 8)
                return false;

            string tempkey = SHA.ComputeSha256Hash(clientKey.Substring(0, 4) + Globals.env.privateKey + clientKey.Substring(4, 8) ); // always 64 chars
            char[] keyArray = tempkey.ToCharArray();
            keyArray.Reverse();
            publicKey = new string(keyArray);
            return true;
        }
    }
}
