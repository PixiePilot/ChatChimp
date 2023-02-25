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
using static System.Windows.Forms.AxHost;
using Azure.Core.GeoJson;

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
            
            EndPoint remoteEndPoint = remoteConn.RemoteEndPoint;
            try
            {
                remoteConn.BeginReceiveFrom
                    (
                        connection.getData(),
                        0,
                        (int)NetSizes.PRELOGIN,
                        SocketFlags.None,
                        ref remoteEndPoint,
                        receiveMessage,
                        connection
                    );
            }catch( Exception e )
            {
                removeUser(connection);
                return;
            }
        }

        public void receiveMessage( IAsyncResult asyncResult )
        {
            #region prepare for handling
            Session connection = (Session)asyncResult.AsyncState!;
            PacketReader reader = new PacketReader( connection.getData() );
            Header header = new Header(reader);
            #endregion
            switch(header.msgId) { // default packets that work across all states
                case (int)NetMessage.TS_CS_HEARTBEAT:
                    doHeartBeat(connection, reader);
                    break;
                default:
                    handleMessage(connection, header, reader); // handles state based messages
                    break;
            }

            listenForMessage( connection );
        }

        public void handleMessage( Session connection, Header header, PacketReader reader ) {
            switch (connection.getState()) {
                case (ushort)UserStates.CREATE_KEY:
                    setKey(connection, header, reader);
                    break;
                case (ushort)UserStates.PRELOGIN:
                    authentication.handleMessage(connection, header, reader);
                    break;
                case (ushort)UserStates.LOGGEDIN:
                    chatServer.handleMessage((User)connection, header, reader);
                    break;
            }
        }

        public void doHeartBeat( Session connection, PacketReader reader ) {
            HeartBeatPacket heartBeatPacket = new HeartBeatPacket( reader, connection );
            connection.getConn().Send( heartBeatPacket.getBeatData( (byte)connection.getState() ) );
        }

        public void setKey( Session connection, Header header, PacketReader reader )
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
