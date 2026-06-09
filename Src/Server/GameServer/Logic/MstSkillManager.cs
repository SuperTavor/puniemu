using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;

namespace Puniemu.Src.Server.GameServer.Logic
{
    public static class MstSkillManager
    {
        private static TableParser<YwpMstYoukaiSkill> _mstSkill { get; set; }

        private static bool _isLoaded = false;
        private static void EnsureLoaded()
        {
            if(!_isLoaded)
            {
                _mstSkill = new TableParser<YwpMstYoukaiSkill>(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai_skill"]!)!["tableData"]);
            }
        }

        public static YwpMstYoukaiSkill? GetSkillObj(long youkaiId)
        {
            EnsureLoaded();
            return _mstSkill.Items.FirstOrDefault(x => x.SoultID == youkaiId);
        }

        public static bool IsBefriender(YwpMstYoukaiSkill obj)
        {
            return obj.SoultType == SoultType.Befriender || obj.SoultType == SoultType.SingleAttackerAndBefriender;
        }
    }
}
