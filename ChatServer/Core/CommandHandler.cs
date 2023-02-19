using ChatServer.Core.Structs;

namespace ChatServer.Core
{
    public static class CommandHandler
    {
        public static void handleCommand(string cmd)
        {
            cmd = cmd.TrimEnd('\n');
            switch (cmd)
            {
                case "test":
                    AccountStruct accountInfo;
                    bool result = Authenticator.loginUserWithResult("test", "123", out accountInfo);
                    GuiHandler.writeEvent(result.ToString());
                    return;

                default:
                    GuiHandler.writeError($"Error, This command {cmd} doesn't exist, Please use Help to see all commands");
                    return;
            }
        }
    }
}
