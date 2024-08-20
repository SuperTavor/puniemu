namespace Puniemu.Src.ConfigManager
{
    public class CStaticGameDataManager
    {

        public Dictionary<string, string> GamedataCache = new();
        public CStaticGameDataManager()
        {
            CacheStaticGameData();
        }

        private void CacheStaticGameData()
        {
            var gamedataPath = CConfigManager.Cfg!.Value.GameDataPath;
            foreach (var file in Directory.GetFiles(gamedataPath))
            {
                GamedataCache[Path.GetFileNameWithoutExtension(file)] = File.ReadAllText(file);
            }
        }
    }
}
