using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Core.DataBase.LogBook {
    public static class LogBookEnums {
        public enum LogType {
            INSERT,
            UPDATE,
            DELETE,
        }
        public enum LogEventEnum {
            LOGIN,
        }
    }
}
