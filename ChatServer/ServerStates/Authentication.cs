using ChatServer.Core;
using ChatServer.Core.DataBase.LogBook;
using ChatServer.Core.DataBase.Models.Accounts;
using ChatServer.Core.MemoryMagic;
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

        public void handleMessage( Session session, Header header, MonkeyNetworkStream MonkeyStream )
        {
            switch(header.msgId)
            {
                case (ushort)NetMessage.TS_CS_LOGIN_REQUEST:
                    handleLoginRequest( session, MonkeyStream );
                    break;
                default:
                    GuiHandler.writeError($"invalid packet id: {header.msgId}");
                    return; // invalid message.
            }
        }

        public void handleLoginRequest( Session session, MonkeyNetworkStream MonkeyStream )
        {
            LoginPacket packet = new LoginPacket( MonkeyStream ); // have to rework packet
            int accountId;
            bool result = Authenticator.loginUserWithResult( packet.getUsername(), packet.getPassword(), out accountId);
            Socket conn = session.getConn();
            if (!result)
            {
                conn.Send( packet.getResult() );
                return;
            }
            BaseAccount account = new BaseAccount(accountId);
            switch ( account.getAccountType() ) {
                case (byte)AccountTypesEnum.USER:
                    account = ModelMarshal.acc_cast_user( account );
                    break;
                case (byte)AccountTypesEnum.ADMIN:
                    account = ModelMarshal.acc_cast_admin( account );
                    break;
                case (byte)AccountTypesEnum.BOT:
                    account = ModelMarshal.acc_cast_bot( account );
                    break;
            }
            LogBookHandler.Log(
                oldValue: ((BaseAccount)account).getUsername(),
                eventId:(int)LogBookEnums.LogEventEnum.LOGIN
                );
            // refactor below
            session.setState((int)UserStates.LOGGEDIN);
            session.getConn().Send(packet.getResult(account));
            // cast is REQUIRED, As we don't know what type of class it is here. and downcasting is always possible!!
            GuiHandler.writeEvent($"User logged on {((BaseAccount)account).getUsername()} at: {session.getIpAddress()}");
            session.changeToLoggedState( account );
        }
    }
}
