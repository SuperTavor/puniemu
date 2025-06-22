using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.InitGacha.DataClasses;
using System.Buffers;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Logic
{
    public class YoukaiManager
    {
        public static TableParser.Logic.TableParser AddYoukai(TableParser.Logic.TableParser parser, long YoukaiId)
        {
            var YoukaiMstTable = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(ConfigManager.Logic.ConfigManager.GameDataManager!.GamedataCache["ywp_mst_youkai"]!)!["tableData"]);
            var YoukaiLevelMstTable = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_mst_youkai_level"]!)!["tableData"]);
            var UserYoukaiIndex = parser.FindIndex([YoukaiId.ToString()]);
            var MstYoukaiIndex = YoukaiMstTable.FindIndex([YoukaiId.ToString()]);
            if (UserYoukaiIndex == -1)
            {
                // add new youkai
                var tmpIdx = 0;
                var levelType = int.Parse(YoukaiMstTable.Table[MstYoukaiIndex][5]);
                foreach (string[] str in YoukaiLevelMstTable.Table)
                {
                    if (str[0] == levelType.ToString() && str[1] == "1")
                        break;
                    tmpIdx++;
                }
                // set Youkai id, level, total exp, hp, attack power, exp limit (for this level), exp (for this level), percentage of exp limit and exp (for this level), actual date, unk (maybe paid level)
                parser.AddRow([YoukaiId.ToString(), "1", "0", YoukaiMstTable.Table[MstYoukaiIndex][8], YoukaiMstTable.Table[MstYoukaiIndex][10], YoukaiLevelMstTable.Table[tmpIdx][3], "0", "0", "0", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(), "0"]);
            }
            else
            {
                // else TODO : edit skill and other stuff if already befriends
                Console.WriteLine("Not implemented");
            }
            return new TableParser.Logic.TableParser(parser.ToString());
        }
    }
}
