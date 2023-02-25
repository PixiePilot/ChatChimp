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

        public void handleMessage( Session session, Header header, PacketReader reader )
        {
            switch(header.msgId)
            {
                case (ushort)NetMessage.TS_CS_LOGIN_REQUEST:
                    handleLoginRequest( session, reader );
                    break;
                default:
                    GuiHandler.writeError($"invalid packet id: {header.msgId}");
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
            session.setState((int)UserStates.LOGGEDIN);
            session.getConn().Send(packet.getResult(accountInfo));
            GuiHandler.writeEvent($"User logged on {accountInfo.username} at: {session.getIpAddress()}");
            session.changeToLoggedState( accountInfo );
        }
    }
}
