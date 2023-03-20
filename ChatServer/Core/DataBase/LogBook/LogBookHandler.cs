using Microsoft.Data.SqlClient;

namespace ChatServer.Core.DataBase.LogBook
{
    public static class LogBookHandler
    {
        public async static void Log(string oldValue, string newValue = "", int eventId = 0, int type = 0)
        {
            SqlConnection conn = DbFactory.createConn();
            using (SqlCommand cmd = new SqlCommand(
                "INSERT INTO logBook (event_id,type,old_value,new_value) VALUES(@event_id,@type,@old_value,@new_value)", conn))
            {
                cmd.Parameters.AddWithValue("@event_id", eventId);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@old_value", oldValue);
                cmd.Parameters.AddWithValue("@new_value", newValue);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}
