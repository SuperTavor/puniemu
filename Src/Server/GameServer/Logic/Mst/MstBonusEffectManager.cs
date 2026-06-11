using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.TableParser.Logic;

namespace Puniemu.Src.Server.GameServer.Logic.Mst
{
    public static class MstBonusEffectManager
    {

        private static TableParser<YwpMstYoukaiBonusEffect> _table { get; set; }

        private static bool _isLoaded { get; set; }
        private static void EnsureLoaded()
        {
            if(!_isLoaded)
            {
                var gm = DataManager.Logic.DataManager.GameDataManager;
                _table = new TableParser<YwpMstYoukaiBonusEffect>(gm.GetTableStringFromJson("ywp_mst_youkai_bonus_effect"));
                _isLoaded = true;
            }
        }

        public static bool IsHaveBonusEffect(long yokaiId)
        {
            EnsureLoaded();
            return _table.Items.FirstOrDefault(x => x.YoukaiID == yokaiId) != null;
        }

    }
}
