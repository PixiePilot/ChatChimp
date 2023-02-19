using Microsoft.Data.SqlClient;
namespace ChatServer.Core.DataBase
{
    public static class DbFactory
    {
        public static SqlConnection createConn()
        {
            string connStr = $"Server={Globals.Globals.env.databaseAddress};" +
                             $"Database={Globals.Globals.env.databaseName};" +
                             $"User Id={Globals.Globals.env.databaseUsername};" +
                             $"Password={Globals.Globals.env.databasePassword};" +
                             "TrustServerCertificate=true";
            SqlConnection sqlConn = new SqlConnection(connStr);
            sqlConn.Open();
            return sqlConn;
        }
    }
}
