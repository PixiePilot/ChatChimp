namespace ChatServer.Core
{
    public static class CommandHandler
    {
        public static void handleCommand(string cmd)
        {
            cmd = cmd.TrimEnd('\n');
            switch (cmd)
            {
                default:
                    GuiHandler.writeError($"Error, This command {cmd} doesn't exist, Please use Help to see all commands");
                    return;
            }
        }
    }
}
