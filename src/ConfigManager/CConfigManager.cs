using Newtonsoft.Json;

namespace Puniemu.Src.ConfigManager
{
    public class CConfigManager
    {
        //Config fields
        public int Port { get; set; }
        public int MaxAllocForDataDownloadCache { get; set; }
        public string FirestoreDatabaseProjectID { get; set; }

        [JsonIgnore]
        private const string CONFIG_PATH = "server_data/cfg.json";

        [JsonIgnore]
        public static CConfigManager? Shared
        {
            get; private set;
        }
        public static void LoadConfigToShared()
        {
            Shared = new();
            if(!File.Exists(CONFIG_PATH))
            {
                throw new FileNotFoundException("Can't find config file. Please make sure it is located is server_data/cfg.json");
            } 
            else
            {
                var configFileContent = File.ReadAllText(CONFIG_PATH);
                Shared = JsonConvert.DeserializeObject<CConfigManager>(configFileContent);
                if (Shared == null)
                {
                    throw new FormatException("Config file deserialization failed, perhaps it's formatted incorrectly?");
                }
            }
        }
    }
}
