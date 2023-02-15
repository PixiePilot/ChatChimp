using ChatServer.Core.Network;
using ChatServer.Core.Network.ConnectionTypes;
using ChatServer.Core.Network.PacketClasses;
using ChatServer.Core.Reader.PacketHandlers;
using ChatServer.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
