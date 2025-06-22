using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.InitGacha.DataClasses;
using System.Buffers;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Logic
{
    public class DictionaryManager
    {
        public static TableParser.Logic.TableParser EditDictionary(TableParser.Logic.TableParser parser, long YoukaiId, bool addSeen, bool addBefriends)
        {
            var YoukaiIndex = parser.FindIndex([YoukaiId.ToString()]);
            var seen = 0;
            var befriend = 0;
            if (addSeen) {
                seen = 1;
            }
            if (addBefriends) {
                befriend = 1;
            }
            if (YoukaiIndex == -1)
            {
                parser.AddRow([YoukaiId.ToString(), befriend.ToString(), seen.ToString()]);
            }
            else
            {
                if (addSeen)
                {
                    parser.Table[YoukaiIndex][2] = "1";
                }
                if (addBefriends)
                {
                    parser.Table[YoukaiIndex][1] = "1";
                }
            }
            return new TableParser.Logic.TableParser(parser.ToString());
        }
    }
}
