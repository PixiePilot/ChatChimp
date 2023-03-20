using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;
#pragma warning disable CS8618
namespace ChatServer.Core.DataBase.Models.Accounts {
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // aligns it bytewise from small to big.
    public unsafe class BaseAccount : IDisposable {
        // NOTE: LPStr will be read as byte buffer which stringBuilder can do when required.
        [MarshalAs(UnmanagedType.U4)]
        private int accountId;
        [MarshalAs(UnmanagedType.LPStr)]
        private string email;
        [MarshalAs(UnmanagedType.LPStr)]
        private string username;
        [MarshalAs(UnmanagedType.LPStr)]
        private string nickname;
        [MarshalAs(UnmanagedType.U2)]
        private ushort tag;
        [MarshalAs(UnmanagedType.LPStr)]
        private string creationDate;
        [MarshalAs(UnmanagedType.LPStr)]
        private string lastLoggedDate;
        [MarshalAs(UnmanagedType.LPStr)]
        private string clientInfo;
        [MarshalAs(UnmanagedType.LPStr)]
        private string ipAddress;
        [MarshalAs(UnmanagedType.U1)]
        private byte accountType;
        [MarshalAs(UnmanagedType.Bool)]
        private bool disposedValue;

        public BaseAccount(int accountId) { // TODO: maybe create a query builder or implement procedure
            SqlConnection conn = DbFactory.createConn();
            
            using ( SqlCommand cmd = new($"SELECT * FROM [dbo.accounts] WHERE primary_key = @account_id;",conn) ) {
                cmd.Parameters.AddWithValue("@account_id", accountId);
                using ( SqlDataReader reader = cmd.ExecuteReader() ) {
                    if( reader.Read() ) {
                        email           = (string)reader["email"];
                        username        = (string)reader["username"]; 
                        nickname        = (string)reader["nickname"];
                        tag             = (ushort)reader["tag"];
                        creationDate    = (string)reader["creation_date"];
                        lastLoggedDate  = (string)reader["last_logged_date"];
                        clientInfo      = (string)reader["client_info"];
                        ipAddress       = (string)reader["ip_address"];
                        accountType     = (byte)reader["account_type"];
                        this.accountId  = accountId;
                    }
                    else {
                        Dispose();
                    }
                }
            }
            // We don't await, As we don't need the result,
            // just quick and dirty Async usage.
            Task.Factory.StartNew( () => CloseConn(conn) ); 

        }
        ~BaseAccount() {
            Dispose();
        }
        public byte getAccountType()
            => accountType;
        public string getUsername()
            => username;
        private void CloseConn( SqlConnection conn ) {
            conn.Close();
            conn.Dispose();
        }
        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: Save changes to SQL Database? 
                    // NOTE: The baseclass will hold all the data that users have in the baseclass,
                    // Anything else is simply extra methods and adds on to check certain things which SHOULDNT be saved
                    // DO NOT SAVE ANY IMPORTANT DATA IN THE CHILD CLASSES!!!!
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BaseAccount()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
