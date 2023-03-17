using ChatServer.Core.Reader.PacketHandlers;
using ChatServer.Core.Structs;

namespace ChatServer.Core.Network.PacketClasses
{
    public class LoginPacket : BasePacket
    {
        private string username { get; set; }
        private string password { get; set; }

        public LoginPacket( MonkeyNetworkStream MonkeyStream ) : base( MonkeyStream )
        {
            username = MonkeyStream.readString();
            password = MonkeyStream.readString();
        }

        public string getUsername()
            => username;

        public string getPassword() 
            => password;

        public byte[] getResult( AccountStruct accountInfo )
        {
            string welcomeMsg = $"Welcome {accountInfo.username}";
            createStream(121);
            createHeader( welcomeMsg.Length , (int)NetMessage.TS_SC_LOGIN_RESULT );
            writeByte(1);// True
            writeString( welcomeMsg );
            return getData();
        }

        public byte[] getResult()
        {
            createStream(70);
            createHeader( 1, (int)NetMessage.TS_SC_LOGIN_RESULT );
            writeByte(0);// False
            return getData();
        }
    }
}
