using System.Reflection;

namespace Puniemu.Src.ConfigManager.Logic
{
    public class GameDataManager
    {

        public Dictionary<string, string> GamedataCache = new();
        public GameDataManager()
        {
            CacheGamedataFromResources();
        }

        private void CacheGamedataFromResources()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string rootNamespace = assembly.GetName().Name;
            string[] resourceNames = assembly.GetManifestResourceNames();

            foreach (var resourceName in resourceNames)
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName)!)
                {
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string content = reader.ReadToEnd();
                            GamedataCache[resourceName
                                .Replace($"{rootNamespace}.Resources.", "")
                                .Replace(".txt", "")
                                                    ] = content;
                        }
                    }
                }
            }
        }
    }
}
