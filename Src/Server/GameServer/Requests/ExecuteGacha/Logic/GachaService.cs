using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;

namespace Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.Logic
{
    public static class GachaService
    {

        private static int SoulLevelFormula(int n)
        {
            if (n == 0)
                return 0;
            return Math.Max(1, (n * (n + 1)) / 3) * 1000;
        }
        private static int SoulLevel(int n)
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
        private static int SoulPoints(int n)
        {
            int points = 0;
            for (int i = 1; i <= n; i++)
            {
                points += SoulLevelFormula(i);
            }
            return points;
        }

        public static SkillResult? CompureSkillPctg(TableParser<YwpUserYoukaiSkill> youkaiList, long YoukaiId)
        {
            var UserYoukaiIndex = YoukaiManager.GetYoukaiSkillIndex(youkaiList, YoukaiId);
            if (UserYoukaiIndex == -1)
            {
                return null;
            }
            else
            {
                int amuLevel = youkaiList.Items[UserYoukaiIndex].Level;
                SkillResult? res = new();
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
                int new_points = 1000 + current_points;
                res.After.Exp = new_points;

                int new_level = SoulLevel(new_points);

                int denominator = SoulLevelFormula(new_level);
                int numerator = new_points - SoulPoints(new_level - 1);
                int percentage = (int)(((double)numerator / (double)denominator) * 100);

                res.After.Level = new_level;
                res.After.ExpBar.Denominator = denominator;
                res.After.ExpBar.Numerator = numerator;
                res.After.ExpBar.Percentage = percentage;

                return res;
            }
        }

        public static List<(int CapsuleID, YokaiGachaResult Result)> Crank(int gachaId, int pullCount)
        {
            List<YokaiGachaResult> droppedYokai = new();

            for (int i = 0; i < pullCount; i++)
            {
                var yokai = GachaPoolManager.GetYokai(gachaId);
                if(yokai == null)
                {
                    throw new Exception("Error while rolling yokai");
                }
                droppedYokai.Add(yokai.Value);
            }

            List<(int CapsuleID, YokaiGachaResult Result)> resultList = new List<(int, YokaiGachaResult)>();

            //PLACEHOLDER: Currently 1 which is gray but should be according to rank. Check notes on ExecuteGachaHandler for mroe info
            int capsuleColor = 1;
            foreach (var yokaiData in droppedYokai)
            {
                resultList.Add((capsuleColor, yokaiData));
            }

            return resultList;
        }
    }
}
