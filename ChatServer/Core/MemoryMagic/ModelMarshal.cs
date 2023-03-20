using ChatServer.Core.DataBase.Models.Accounts;
using System.Runtime.InteropServices;

namespace ChatServer.Core.MemoryMagic {
    public unsafe static class ModelMarshal {
        public static unsafe IntPtr get_acc_ptr( BaseAccount acc_obj ) {
            IntPtr acc_ptr = sizeof(BaseAccount); // creates a memory block the size of the object.
            Marshal.StructureToPtr(acc_obj, acc_ptr , true );
            return acc_ptr;
        }
        public static unsafe AdminAccount acc_cast_admin(BaseAccount acc_obj ) {
            return (AdminAccount)Marshal.PtrToStructure(get_acc_ptr(acc_obj), typeof(AdminAccount))!;
        }
        public static unsafe UserAccount acc_cast_user(BaseAccount acc_obj) {
            return (UserAccount)Marshal.PtrToStructure(get_acc_ptr(acc_obj), typeof(UserAccount))!;
        }
        public static unsafe BotAccount acc_cast_bot(BaseAccount acc_obj) {
            return (BotAccount)Marshal.PtrToStructure(get_acc_ptr(acc_obj), typeof(BotAccount))!;
        }
    }
}
