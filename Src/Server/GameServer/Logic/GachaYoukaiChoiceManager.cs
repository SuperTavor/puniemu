using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.TableParser.Logic;

namespace Puniemu.Src.Server.GameServer.Logic
{
    public static class GachaYoukaiChoiceManager
    {
        private static TableParser<YwpMstGachaYoukaiChoice> _table { get; set; }

        private static bool _loaded = false;
        private static void EnsureLoaded()
        {
            if(!_loaded)
            {
                var gm = DataManager.Logic.DataManager.GameDataManager;
                _table = new(gm.GetTableStringFromJson("ywp_mst_gacha_youkai_choice"));
                _loaded = true;
            }
        }

        public static bool IsChoiceOk(int gachaId, int youkaiId)
        {
            EnsureLoaded();
            return _table.Items.FirstOrDefault(x => x.GachaID == gachaId && x.YokaiID == youkaiId) != null;
        }
    }
}
