using Newtonsoft.Json;
using Puniemu.Src.ConfigManager.DataClasses;
namespace Puniemu.Src.ConfigManager.Logic
{
    public static class ConfigManager
    {

        public static GameDataManager GameDataManager;
        public static ConfigStructure? Cfg;
        private const string CONFIG_PATH = "cfg.json";

        public static void Initialize()
        {
            if (!File.Exists(CONFIG_PATH))
            {
                throw new FileNotFoundException($"Can't find config file. Please make sure it is located at {CONFIG_PATH}");
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
            GameDataManager = new GameDataManager();
        }
    }
}
