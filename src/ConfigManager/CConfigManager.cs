using Newtonsoft.Json;

namespace Puniemu.Src.ConfigManager
{
    public static class CConfigManager
    {
        public struct ConfigStructure
        {
            public string FirestoreDatabaseProjectID { get; set; }
            public string BaseDataDownloadURL { get; set; }
        }

        public static ConfigStructure? Cfg;
        //Logic
        [JsonIgnore]
        private const string CONFIG_PATH = "cfg.json";

        public static void Initialize()
        {
            if(!File.Exists(CONFIG_PATH))
            {
                throw new FileNotFoundException("Can't find config file. Please make sure it is located is server_data/cfg.json");
            } 
            else
            {
                var configFileContent = File.ReadAllText(CONFIG_PATH);
                Cfg = JsonConvert.DeserializeObject<ConfigStructure>(configFileContent);
                if (Cfg == null)
                {
                    throw new FormatException("Config file deserialization failed, perhaps it's formatted incorrectly?");
                }
            }
        }
    }
}
