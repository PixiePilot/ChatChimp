using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Core.Structs
{
    public struct AccountStruct
    {
        public int accountId { get; set; }
        public string username { get; set; }
        public int permissions { get; set; }

    }
}
