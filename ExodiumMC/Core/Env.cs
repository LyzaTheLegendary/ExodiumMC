using ExodiumMC.Core;

namespace Core
{
    public static class Env
    {
        public static string reconHost = string.Empty;
        public static string reconPass = string.Empty;

        public static string serverHost = string.Empty;
        public static int serverVer = 0;
        public static int receiveDelay = 999;
        public static void Construct()
        {
            SettingReader reader = new("settings.txt");
            reconHost = reader.GetValue("recon.host");
            reconPass = reader.GetValue("recon.password");
            serverHost = reader.GetValue("server.host");
            serverVer = int.Parse(reader.GetValue("server.ver"));
            receiveDelay = int.Parse(reader.GetValue("server.delay"));

        }
    }
}
