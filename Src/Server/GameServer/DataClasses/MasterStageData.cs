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
    }
    public static class MasterStageData
    {
        private static List<NecessaryMasterStageDataItem> _items = new();

        public static List<NecessaryMasterStageDataItem> Items
        {
            get
            {
                if (!_attemptedInit)
                    Init(DataManager.Logic.DataManager.GameDataManager!.GamedataCache["ywp_mst_stage"]);

                return _items;
            }
        }
        private static bool _attemptedInit = false;
        private static void Init(string data)
        {
            _attemptedInit = true;
            var obj = JObject.Parse(data);
            //Puni
            if (obj["tableData"] != null)
            {
                var tblStr = obj["tableData"]!.ToString();
                var prsr = new TableParser.Logic.TableParser<PuniMstStageItem>(tblStr);

                foreach (var item in prsr.Items)
                {
                    Items.Add(new NecessaryMasterStageDataItem
                    {
                        StageId = item.StageId,
                        StageType = item.StageType,
                        BattleType = item.BattleType
                    });
                }
                
            }
            //WibWob
            else if (obj["data"] is JArray arr)
            {
                var stageList = arr.ToObject<List<WibwobMstStageItem>>();

                foreach (var item in stageList)
                {
                    Items.Add(new NecessaryMasterStageDataItem
                    {
                        StageId = item.StageID,
                        StageType = item.StageType,
                        BattleType = 0
                    });
                }
                
            }
        }
    }
}
