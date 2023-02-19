namespace ChatServer.Core.Network
{
    public enum NetMessage
    {
        TS_SC_GIVE_KEY = 4,

        TS_SC_RESPONSE_INIT = 1,

        TS_CS_LOGIN_REQUEST = 2,

        TS_SC_LOGIN_RESULT = 3,
    }

    public enum NetSizes
    {
        CREATE_KEY = 120,
        PRELOGIN = 300,
        LOGGEDIN = 6024,
    }
}
