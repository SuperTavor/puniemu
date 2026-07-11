using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.TableParser.Logic;

namespace Puniemu.Src.Server.GameServer.Logic.Mst
{
    public static class MstLegendReleaseManager
    {
        private static bool _loaded = false;
        public static TableParser<YwpMstYoukaiLegendRelease> Table { get; set; }
        
        public static void EnsureLoaded()
        {
            if(!_loaded)
            {
                var gm = DataManager.Logic.DataManager.GameDataManager;
                var raw = (string)JsonConvert.DeserializeObject<Dictionary<string, object>>(gm.GamedataCache["ywp_mst_youkai_legend_release"])["tableData"];
                Table = new TableParser<YwpMstYoukaiLegendRelease>(raw);
                _loaded = true;
            }
        }

        
        public static long CheckLegendYoukaiId(long youkaiId)
        {
            EnsureLoaded();
            var idx = Table.Items.FindIndex(x => x.Yokai1ID == youkaiId || x.Yokai2ID == youkaiId || x.Yokai3ID == youkaiId || x.Yokai4ID == youkaiId || x.Yokai5ID == youkaiId || x.Yokai6ID == youkaiId);
            if (idx == -1) return 0;

            return Table.Items[idx].LegendYokaiID;
        }

    }
}
