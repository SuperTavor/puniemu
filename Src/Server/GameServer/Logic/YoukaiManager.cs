using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.InitGacha.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using System.Buffers;
using System.IO;
using System.Numerics;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Logic
{
    public class YoukaiManager
    {
        static int SoulLevelFormula(int n)
        {
            if (n == 0)
                return 0;
            return Math.Max(1, (n * (n + 1)) / 3) * 1000;
        }
        static int SoulLevel(int n)
        {
            int points = 0;
            for (int i = 1; i < 7; i++)
            {
                points += SoulLevelFormula(i);
                if (n < points)
                    return i;
            }
            return 7;
        }
        static int SoulPoints(int n)
        {
            int points = 0;
            for (int i = 1; i <= n; i++)
            {
                points += SoulLevelFormula(i);
            }
            return points;
        }

        public static void AddYoukai(TableParser<YwpUserYoukai> parser, long YoukaiId, TableParser<YwpUserYoukaiSkill> parser2)
        {
            var YoukaiMstTable = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager!.GamedataCache["ywp_mst_youkai"]!)!["tableData"]);
            var YoukaiLevelMstTable = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai_level"]!)!["tableData"]);
            var UserYoukaiIndex = GetYoukaiIndex(parser, YoukaiId);
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
                parser.AddItem(new YwpUserYoukai { YoukaiId = YoukaiId, Level = 1, Exp = 0, Hp =  int.Parse(YoukaiMstTable.Table[MstYoukaiIndex][8]), Atk = int.Parse(YoukaiMstTable.Table[MstYoukaiIndex][10]), ExpDenominator = int.Parse(YoukaiLevelMstTable.Table[tmpIdx][3]), ExpNumerator = 0, Percentage = 0, BefriendDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), BreakLimitCount = 0 });
            }
            YoukaiManager.AddYoukaiSkill(ref parser2, YoukaiId);
            return;
        }
        public static void AddYoukaiSkill(ref TableParser<YwpUserYoukaiSkill> youkaiList, long YoukaiId)
        {
            var UserYoukaiIndex = GetYoukaiSkillIndex(youkaiList, YoukaiId);
            if (UserYoukaiIndex == -1)
            {
                youkaiList.AddItem(new YwpUserYoukaiSkill { YoukaiId = YoukaiId, Level = 1, Points = 0, PercentageDenominator = 1000, PercentageNumerator = 0, Percentage = 0 });
                return;
            }
            int amuLevel = youkaiList.Items[UserYoukaiIndex].Level;
            if (amuLevel >= 7)
            {
                return;
            }
            int current_points = youkaiList.Items[UserYoukaiIndex].Points;
            int new_points = 1000 + current_points;
            int new_level = SoulLevel(new_points);

            int denominator = SoulLevelFormula(new_level);
            int numerator = new_points - SoulPoints(new_level - 1);
            int percentage = (int)(((double)numerator / (double)denominator) * 100);
            youkaiList.Items[UserYoukaiIndex].Level = new_level;
            youkaiList.Items[UserYoukaiIndex].Points = new_points;
            youkaiList.Items[UserYoukaiIndex].PercentageDenominator = denominator;
            youkaiList.Items[UserYoukaiIndex].PercentageNumerator = numerator;
            youkaiList.Items[UserYoukaiIndex].Percentage = percentage;
        }
        public static int GetYoukaiSkillIndex(TableParser<YwpUserYoukaiSkill> parser, long YoukaiId)
        {
            uint count = 0;
            foreach (YwpUserYoukaiSkill i in parser.Items)
            {
                if (i.YoukaiId == YoukaiId)
                {
                    return (int)count;
                }
                count += 1;
            }
            return -1;
        }
        public static int GetYoukaiIndex(TableParser<YwpUserYoukai> parser, long YoukaiId)
        {
            uint count = 0;
            foreach (YwpUserYoukai i in parser.Items)
            {
                if (i.YoukaiId == YoukaiId)
                {
                    return (int)count;
                }
                count += 1;
            }
            return -1;
        }
    }
}
