using System.Reflection;
using System.Text;

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
                        using (StreamReader reader = new StreamReader(stream,encoding:Encoding.UTF8))
                        {
                            string content = reader.ReadToEnd();
                            var filteredName = resourceName
                                .Replace($"{rootNamespace}.Resources.", "")
                                .Replace(".txt", "");
                            GamedataCache[filteredName] = content;
                        }
                    }
                }
            }
        }
    }
}
