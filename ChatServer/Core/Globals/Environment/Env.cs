using MinecraftSharp.Core.Reader.OptionsReader;
namespace ChatServer.Core.Globals.Environment
{
    public class Env
    {
        // opt file
        public string ipAddress { get; set; }

        public int port { get; set; }

        public int maxUsers { get; set; }

        public int maxPing { get; set; }

        public string appName { get; set; }

        public string databaseAddress { get; set; }
        public string databaseUsername { get; set; }
        public string databasePassword { get; set; }
        public string databaseName { get; set; }

        // Non opt globals
        public int buffSzLogin = 1024;

        public int buffSzLogged = 2048;

        public Env()
        {
            OptionReader reader = new("options");
            ipAddress        = reader.getEnv("ip_address") ?? "0.0.0.0";
            port             = reader.getEnv("port") ?? 25565;
            maxUsers         = reader.getEnv("max_users") ?? 100;
            maxPing          = reader.getEnv("max_ping") ?? 100;
            appName          = reader.getEnv("app_name") ?? "Name not found!";
            databaseAddress  = reader.getEnv("db_address") ?? "127.0.0.1";
            databaseUsername = reader.getEnv("db_username") ?? "sa";
            // als er geen wachtwoord is verwacht het nog steeds een string.
            databasePassword = reader.getEnv("db_password") ?? string.Empty;
            databaseName     = reader.getEnv("db_name") ?? "ChatServer";
        }
    }
}
