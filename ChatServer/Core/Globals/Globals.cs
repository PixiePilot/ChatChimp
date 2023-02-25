using ChatServer.Core.Globals.Environment;
using ChatServer.ServerStates;
namespace ChatServer.Core.Globals
{
    public static class Globals
    {
        public static Env env { get; set; }
        public static IoAcceptor acceptor { get; set; }
        public static Authentication authentication { get; set; }
        public static ServerChat chatServer { get; set; }

        #region Const values
        public const int heartBeatTime = 5000;

        #endregion
    }
}
