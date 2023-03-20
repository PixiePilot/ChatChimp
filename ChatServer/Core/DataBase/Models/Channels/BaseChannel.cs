using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Core.DataBase.Models.Channels {
    [StructLayout(LayoutKind.Sequential)]
    public struct BaseChannel {
        [MarshalAs(UnmanagedType.U4)]
        public uint channelId;
        [MarshalAs(UnmanagedType.U2)]
        public ushort channelType;
        [MarshalAs(UnmanagedType.U4)]
        public uint messageId;
        [MarshalAs(UnmanagedType.U4)]
        public uint ownerId;
    }
}
