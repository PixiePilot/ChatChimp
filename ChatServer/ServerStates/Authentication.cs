using ChatServer.Core;
using ChatServer.Core.Network;
using ChatServer.Core.Network.ConnectionTypes;
using ChatServer.Core.Network.PacketClasses;
using ChatServer.Core.Reader.PacketHandlers;
using ChatServer.Core.Structs;
using System.Net.Sockets;

namespace ChatServer.ServerStates
{
    public class Authentication
    {

        public void handleMessage(Session session)
        {
            PacketReader reader = new PacketReader( session.getData() );

            int contentSize = reader.readIntBytes();
            int msgID = reader.readIntBytes();

            switch(msgID)
            {
                case (ushort)NetMessage.TS_CS_LOGIN_REQUEST:
                    handleLoginRequest( session, reader );
                    break;
                default:
                    GuiHandler.writeError($"invalid packet id: {msgID}");
                    return; // invalid message.
            }
        }

        public void handleLoginRequest( Session session, PacketReader reader )
        {
            LoginPacket packet = new LoginPacket( reader );
            AccountStruct accountInfo;
            bool result = Authenticator.loginUserWithResult( packet.getUsername(), packet.getPassword(), out accountInfo );
            Socket conn = session.getConn();
            if (!result)
            {
                conn.Send( packet.getResult() );
                return;
            }
            session.getConn().Send( packet.getResult(accountInfo) );
            session.changeToLoggedState( accountInfo );
        }
    }
}
