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
        static readonly int[] SoulLevelCosts = { 0, 1000, 2000, 4000, 6000, 9000, 12000 };

        static int SoulLevelFormula(int n)
        {
            if (n < 0 || n >= SoulLevelCosts.Length)
                return 0;
            return SoulLevelCosts[n];
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

        public static SkillResult? AddExpToSkill(TableParser<YwpUserYoukaiSkill> youkaiList, long YoukaiId, int expToAdd)
        {
            var UserYoukaiIndex = GetYoukaiSkillIndex(youkaiList, YoukaiId);
            if (UserYoukaiIndex == -1)
            {
                return null;
            }
            else
            {
                int amuLevel = youkaiList.Items[UserYoukaiIndex].Level;
                SkillResult? res = new();
                res.SkillID = YoukaiId;
                res.isMaxLevel = false;
                res.Before.Level = amuLevel;
                res.Before.Exp = youkaiList.Items[UserYoukaiIndex].Points;
                res.Before.ExpBar.Denominator = youkaiList.Items[UserYoukaiIndex].PercentageDenominator;
                res.Before.ExpBar.Numerator = youkaiList.Items[UserYoukaiIndex].PercentageNumerator;
                res.Before.ExpBar.Percentage = youkaiList.Items[UserYoukaiIndex].Percentage;

                if (amuLevel >= 7)
                {
                    res.After.Level = res.Before.Level;
                    res.After.Exp = res.Before.Exp;
                    res.After.ExpBar.Denominator = res.Before.ExpBar.Denominator;
                    res.After.ExpBar.Numerator = res.Before.ExpBar.Numerator;
                    res.After.ExpBar.Percentage = res.Before.ExpBar.Percentage;
                    res.isMaxLevel = true;
                    return res;
                }

                int current_points = res.Before.Exp;
                int new_points = expToAdd + current_points;
                res.After.Exp = new_points;

                int new_level = SoulLevel(new_points);

                int denominator = SoulLevelFormula(new_level);
                int numerator = new_points - SoulPoints(new_level - 1);
                int percentage = (int)(((double)numerator / (double)denominator) * 100);
                if (new_level >= 7) percentage = 0;

                res.After.Level = new_level;
                res.After.ExpBar.Denominator = denominator;
                res.After.ExpBar.Numerator = numerator;
                res.After.ExpBar.Percentage = percentage;

                return res;
            }
        }

        public static void DeleteYoukai(TableParser<YwpUserYoukai> userYokai, TableParser<YwpUserYoukaiSkill> userSkill, long youkaiId)
        {
            userYokai.Items.Remove(userYokai.Items[yokaiIdx]);
            userSkill.Items.Remove(userSkill.Items[skillIdx]);
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
            if (new_level >= 7)
            {
                percentage = 0;
            }
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
