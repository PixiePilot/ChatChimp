using System.Net;
using System.Net.Sockets;
using ChatServer.Core.Globals;
using ChatServer.Core;
using ChatServer.Core.Network.PacketClasses;
using ChatServer.Core.Network.ConnectionTypes;
using ChatServer.Core.Network;
using ChatServer.ServerStates;
using ChatServer.Core.Reader.PacketHandlers;

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
            GuiHandler.writeEvent("Remote connection closed at: " + session.getConn().RemoteEndPoint.ToString());
            try { session.getConn().Shutdown(SocketShutdown.Send);  }
            finally { session.getConn().Close(); }
        }

        public bool validConn( Socket remoteConn )
        {
            try
            {
                return !( remoteConn.Poll( 1, SelectMode.SelectRead ) && remoteConn.Available == 0 );
            }
            catch (SocketException) { return false; }
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
            Session connection = (Session)asyncResult.AsyncState!;
            switch( connection.getState() )
            {
                case (ushort)UserStates.CREATE_KEY:
                    createKey( connection );
                    break;
                case (ushort)UserStates.PRELOGIN:
                    authentication.handleMessage( connection );
                    break;
                case (ushort)UserStates.LOGGEDIN:
                    chatServer.handleMessage( (User)connection  );
                    break;
            }

            listenForMessage( connection );
        }

        public void createKey( Session connection)
        {
            PacketReader reader = new PacketReader( connection.getData() );

            int contentSize = reader.readIntBytes();
            int msgID = reader.readIntBytes(); // should be 9999 bc doesn't make sense ( security reasons )
            string publicKey = reader.readString();
            bool result = connection.createKey( publicKey );

            if ( !result || msgID != 9999 )
            {
                removeUser(connection);
                return;
            }
            EncryptKeyPacket packet = new EncryptKeyPacket( (int)NetSizes.CREATE_KEY, connection.getKey() );
            connection.getConn().Send( packet.getData() );
        }
    }
}
