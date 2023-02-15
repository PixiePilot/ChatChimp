using ChatServer.Core.DataBase;
using ChatServer.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ChatServer
{
    public static class Authenticator
    {
        public static bool loginUserWithResult(string username, string password, out AccountStruct accountInfo)
        {
            accountInfo = new();
            SqlConnection sqlConn = DbFactory.createConn();
            
            using ( SqlCommand cmd = new SqlCommand("dbo.getUserViaLogin",sqlConn)) 
            {
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter INusername = new SqlParameter();
                INusername.ParameterName = "INusername";
                INusername.Value = username;
                INusername.Direction = ParameterDirection.Input;

                SqlParameter INpassword = new SqlParameter();
                INpassword.ParameterName = "INpassword";
                INpassword.Value = password;
                INpassword.Direction = ParameterDirection.Input;

                SqlParameter OUTusername = new SqlParameter();
                OUTusername.ParameterName = "OUTusername";
                OUTusername.DbType = DbType.String;
                OUTusername.Direction = ParameterDirection.Output;
                OUTusername.Size = 255;

                SqlParameter OUTaccountId = new SqlParameter();
                OUTaccountId.ParameterName = "OUTaccountId";
                OUTaccountId.DbType = DbType.Int32;
                OUTaccountId.Direction = ParameterDirection.Output;

                SqlParameter OUTpermission = new SqlParameter();
                OUTpermission.ParameterName = "OUTpermissions";
                OUTpermission.DbType = DbType.Int32;
                OUTpermission.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(INusername);
                cmd.Parameters.Add(INpassword);
                cmd.Parameters.Add(OUTusername);
                cmd.Parameters.Add(OUTaccountId);
                cmd.Parameters.Add(OUTpermission);

                using ( SqlDataReader reader = cmd.ExecuteReader())
                {
                    try
                    {
                        accountInfo.accountId = (int)OUTaccountId.Value;
                        accountInfo.username = (string)OUTusername.Value;
                        accountInfo.permissions = (int) OUTpermission.Value;
                    }
                    catch (Exception) { return false; }
                }
            }
            

            return true;
        }
    }
}
