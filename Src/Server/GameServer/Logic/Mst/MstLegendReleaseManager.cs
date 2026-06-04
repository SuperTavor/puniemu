using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.TableParser.Logic;

namespace Puniemu.Src.Server.GameServer.Logic.Mst
{
    public static class MstLegendReleaseManager
    {
        private static bool _loaded = false;
        private static TableParser<YwpMstYoukaiLegendRelease> _mstLegend { get; set; }
        
        private static void EnsureLoaded()
        {
            if(!_loaded)
            {
                var gm = DataManager.Logic.DataManager.GameDataManager;
                var raw = (string)JsonConvert.DeserializeObject<Dictionary<string, object>>(gm.GamedataCache["ywp_mst_youkai_legend_release"])["tableData"];
                _mstLegend = new TableParser<YwpMstYoukaiLegendRelease>(raw);
                _loaded = true;
            }
        }

        
        public static long CheckLegendYoukaiId(long youkaiId)
        {
            EnsureLoaded();
            var idx = _mstLegend.Items.FindIndex(x => x.Yokai1ID == youkaiId || x.Yokai2ID == youkaiId || x.Yokai3ID == youkaiId || x.Yokai4ID == youkaiId || x.Yokai5ID == youkaiId || x.Yokai6ID == youkaiId);
            if (idx == -1) return 0;

            return _mstLegend.Items[idx].LegendYokaiID;
        }

    }
}
