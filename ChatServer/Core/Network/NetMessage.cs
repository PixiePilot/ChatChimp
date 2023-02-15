using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Core.Network
{
    public enum NetMessage
    {
        TS_SC_RESPONSE_INIT = 1,

        TS_CS_LOGIN_REQUEST = 2,

        TS_SC_LOGIN_RESULT = 3,
    }

    public enum NetSizes
    {
        PRELOGIN = 300,
        LOGGEDIN = 6024,
    }
}
