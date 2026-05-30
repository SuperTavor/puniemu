using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class NecessaryMasterStageDataItem
    {
        public long StageId;
        public int StageType;
        //unused in wibwob
        public int BattleType;
        public long[] StarCondIDs = new long[3];
        public int BossFlag;
        public int UseActionPoint;
        public int UseActionType;
        public int UseActionID;
    }
    public static class MasterStageData
    {
        private static List<NecessaryMasterStageDataItem> _stageItems = new();

        private static List<StageConditionItem> _conditionItems = new();
        public static List<NecessaryMasterStageDataItem> StageItems
        {
            get
            {
                if (!_isInitStage)
                    InitStage();

                return _stageItems;
            }
        }

        public static List<StageConditionItem> ConditionItems
        {
            get
            {
                if (!_isInitCond)
                    InitCond();

                return _conditionItems;
            }
            set
            {
                _conditionItems = value;
            }
        }
        private static bool _isInitStage = false;
        private static bool _isInitCond = false;
        private static void InitCond()
        {
            var data = DataManager.Logic.DataManager.GameDataManager!.GamedataCache["ywp_mst_stage_condition"];
            _isInitCond = true;
            var obj = JObject.Parse(data);
            //puni
            if (obj["tableData"] != null)
            {
                var tbl = new TableParser<StageConditionItem>(obj["tableData"]!.ToString(), delimiter:"^");
                ConditionItems = tbl.Items;
            }
            else if (obj["data"] is JArray arr)
            {
                var condList = arr.ToObject<List<StageConditionItem>>();
                ConditionItems = condList;
            }
            else
            {
                throw new InvalidDataException("Bad ywp_mst_stage_condition");
            }
        }
        private static void InitStage()
        {
            var ywpMstStage = DataManager.Logic.DataManager.GameDataManager!.GamedataCache["ywp_mst_stage"];
            _isInitStage = true;
            var obj = JObject.Parse(ywpMstStage);
            //Puni
            if (obj["tableData"] != null)
            {
                var tblStr = obj["tableData"]!.ToString();
                var prsr = new TableParser<PuniMstStageItem>(tblStr);

                foreach (var item in prsr.Items)
                {
                    StageItems.Add(new NecessaryMasterStageDataItem
                    {
                        StageId = item.StageId,
                        StageType = item.StageType,
                        BattleType = 0,
                        StarCondIDs = [item.StarCond1, item.StarCond2, item.StarCond3],
                        BossFlag = item.BossFlag,
                        UseActionID = item.UseActionId,
                        UseActionPoint = item.UseActionPoint,
                        UseActionType = item.UseActionType

                    });
                }
                
            }
            //WibWob
            else if (obj["data"] is JArray arr)
            {
                var stageList = arr.ToObject<List<WibwobMstStageItem>>();

                foreach (var item in stageList)
                {
                    StageItems.Add(new NecessaryMasterStageDataItem
                    {
                        StageId = item.StageID,
                        StageType = item.StageType,
                        BattleType = 0,
                        StarCondIDs = [item.StarCond1, item.StarCond2, item.StarCond3],
                        BossFlag = item.BossFlag,
                        UseActionID = item.UseActionID,
                        UseActionPoint = item.UseActionPoint,
                        UseActionType = item.UseActionType
                    });
                }
                
            }
            else
            {
                throw new InvalidDataException("Bad ywp_mst_stage");
            }
        }

        public static int GetStageConditionIndex(long ConditionId)
        {
            uint count = 0;
            foreach (StageConditionItem i in ConditionItems)
            {
                if (i.ConditionId == ConditionId)
                {
                    return (int)count;
                }
                count += 1;
            }
            return -1;
        }
        public static long GetNextStage(long StageId)
        {
            int mapId = (int)Math.Floor(StageId / 1000.0);
            int count = ((int)StageId % 1000) + 1;
            while (true)
            {
                int stageIndex = StageItems.FindIndex(x => x.StageId == (mapId * 1000) + count);
                if (stageIndex == -1)
                {
                    break;
                }
                if (StageItems[stageIndex].StageType == 1)
                {
                    return StageItems[stageIndex].StageId;
                }
                count++;
            }
            return -1;
        }

        // guesse the unlocked secret stage based on stageId
        public static long GetUnlockedSecretStage(long StageId, int _skipp)
        {
            int mapId = (int)Math.Floor(StageId / 1000.0);
            int maxStageId = (int)StageId % 1000;
            int skipp = 0;
            int count = 1;
            while (true)
            {
                int stageIndex = StageItems.FindIndex(x => x.StageId == (mapId * 1000) + count);
                if (stageIndex == -1)
                {
                    break;
                }
                if (count < maxStageId)
                {
                    int _count = 4; // cond before 4 (1,2,3) are stars flg
                    while (true)
                    {
                        int condIndex = GetStageConditionIndex((((mapId * 1000) + count) * 10) + _count);
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
                    if (StageItems[stageIndex].StageType == 2) // secret stage
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
                            return StageItems[stageIndex].StageId;
                        }
                    }
                }
                count++;
            }
            return -1;

        }
    }
}
