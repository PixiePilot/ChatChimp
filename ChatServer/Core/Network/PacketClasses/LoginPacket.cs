using ChatServer.Core.DataBase.Models.Accounts;
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

        public void sendResult( MonkeyNetworkStream monkeyStream, string username, bool positive )//BaseAccount account )
        {
            if (!positive) {
                monkeyStream.sendHeader((int)NetMessage.TS_SC_LOGIN_RESULT, 1);
                monkeyStream.writeBool(positive);
                return;
            }

            string welcomeMsg = $"Welcome {username}";
            monkeyStream.sendHeader((int)NetMessage.TS_SC_LOGIN_RESULT, welcomeMsg.Length);
            monkeyStream.writeBool(positive);
            monkeyStream.writeString(welcomeMsg);
        }

    }
}
