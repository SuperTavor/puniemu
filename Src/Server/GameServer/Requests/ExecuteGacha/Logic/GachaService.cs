using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;

namespace Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.Logic
{
    public static class GachaService
    {
        public static Dictionary<RarityType, CapsuleColor> CAPSULE_CLRS =
        new() {
            {RarityType.RarityE, CapsuleColor.Gray },
            {RarityType.RarityD, CapsuleColor.Gray },
            {RarityType.RarityC, CapsuleColor.Blue },
            {RarityType.RarityB, CapsuleColor.Blue},
            {RarityType.RarityA, CapsuleColor.Red},
            {RarityType.RarityS, CapsuleColor.Gold },
            {RarityType.RaritySS, CapsuleColor.Gold },
            {RarityType.RaritySSS, CapsuleColor.Gold },
            {RarityType.RarityZ, CapsuleColor.Gold },
            {RarityType.RarityZZ, CapsuleColor.Gold },
            {RarityType.RarityZZZ, CapsuleColor.Rainbow },
            {RarityType.RarityUZ, CapsuleColor.Rainbow },
            {RarityType.RarityUZP, CapsuleColor.Rainbow },
        };
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

       
    }
}
