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
    public class StageManager
    {
        public static void AddStage(ref TableParser<YwpUserStage> parser, long StageId)
        {
            var UserStageIndex = GetStageIndex(ref parser, StageId);
            var MstStageIndex = MasterStageData.StageItems.FindIndex(x => x.StageId == StageId);
            if (UserStageIndex == -1 && MstStageIndex != -1)
            {
                parser.AddItem(new YwpUserStage { StageId = StageId, IsClear = 0, Score = 0, Star1 = 0, Star2 = 0, Star3 = 0, NumClear = 0, Unk2 = 0 });
            }
        }

        public static void EditStage(ref TableParser<YwpUserStage> parser, long StageId, int isClear, long score, int star1, int star2, int star3, int numClear)
        {
            var UserStageIndex = GetStageIndex(ref parser, StageId);
            var MstStageIndex = MasterStageData.StageItems.FindIndex(x => x.StageId == StageId);
            if (UserStageIndex != -1 && MstStageIndex != -1)
            {
                if (score > parser.Items[UserStageIndex].Score)
                {
                    parser.Items[UserStageIndex].Score = score;
                }
                if (isClear == 1 && parser.Items[UserStageIndex].IsClear == 0)
                {
                    parser.Items[UserStageIndex].IsClear = 1;
                }
                if (star1 == 1 && parser.Items[UserStageIndex].Star1 == 0)
                {
                    parser.Items[UserStageIndex].Star1 = 1;
                }
                if (star2 == 1 && parser.Items[UserStageIndex].Star2 == 0)
                {
                    parser.Items[UserStageIndex].Star2 = 1;
                }
                if (star3 == 1 && parser.Items[UserStageIndex].Star3 == 0)
                {
                    parser.Items[UserStageIndex].Star2 = 1;
                }
                if (numClear > parser.Items[UserStageIndex].NumClear)
                {
                    parser.Items[UserStageIndex].NumClear = numClear;
                }
            }
        }

        public static int GetStageIndex(ref TableParser<YwpUserStage> parser, long StageId)
        {
            uint count = 0;
            foreach (YwpUserStage i in parser.Items)
            {
                if (i.StageId == StageId)
                {
                    return (int)count;
                }
                count += 1;
            }
            return -1;
        }
    }
}
