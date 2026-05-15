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
    public class MstStageManager
    {
        public static int GetStageConditionIndex(ref TableParser<YwpMstStageCondition> parser, long ConditionId)
        {
            uint count = 0;
            foreach (YwpMstStageCondition i in parser.Items)
            {
                if (i.ConditionId == ConditionId)
                {
                    return (int)count;
                }
                count += 1;
            }
            return -1;
        }
        public static int GetStageIndex(ref TableParser<PuniMstStageItem> parser, long StageId)
        {
            uint count = 0;
            foreach (PuniMstStageItem i in parser.Items)
            {
                if (i.StageId == StageId)
                {
                    return (int)count;
                }
                count += 1;
            }
            return -1;
        }

        public static long GetNextStage(ref TableParser<PuniMstStageItem> parser, long StageId)
        {
            int mapId = (int)Math.Floor(StageId / 1000.0);
            int count = ((int)StageId % 1000) + 1;
            while (true)
            {
                int stageIndex = GetStageIndex(ref parser, (mapId * 1000) + count);
                if (stageIndex == -1)
                {
                    break;
                }
                if (parser.Items[stageIndex].StageType == 1)
                {
                    return parser.Items[stageIndex].StageId;
                }
                count++;
            }
            return -1;
        }

        // guesse the unlocked secret stage based on stageId
        public static long GetUnlockedSecretStage(ref TableParser<PuniMstStageItem> parser, ref TableParser<YwpMstStageCondition> parser2, long StageId, int _skipp)
        {
            int mapId = (int)Math.Floor(StageId / 1000.0);
            int maxStageId = (int)StageId % 1000;
            int skipp = 0;
            int count = 1;
            while (true)
            {
                int stageIndex = GetStageIndex(ref parser, (mapId * 1000) + count);
                if (stageIndex == -1)
                {
                    break;
                }
                if (count < maxStageId) {
                    int _count = 4; // cond before 4 (1,2,3) are stars flg
                    while (true)
                    {
                        int condIndex = GetStageConditionIndex(ref parser2, (((mapId * 1000) + count) * 10) + _count);
                        if (condIndex == -1)
                        {
                            break;
                        }
                        skipp++;
                        _count++;
                    }
                }
                else
                {
                    if (parser.Items[stageIndex].StageType == 2) // secret stage
                    {
                        if (skipp > 0)
                        {
                            skipp--;
                        }
                        else if (_skipp > 0)
                        {
                            _skipp--;
                        }
                        else
                        {
                            return parser.Items[stageIndex].StageId;
                        }
                    }
                }
                count++;
            }
            return -1;

        }
    }
}
