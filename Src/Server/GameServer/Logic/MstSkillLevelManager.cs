using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.TableParser.Logic;
using System.Collections.Concurrent;

namespace Puniemu.Src.Server.GameServer.Logic
{
    public static class MstSkillLevelManager
    {
        private static List<YwpMstYoukaiSkillLevel> _skillLevelEntries;

        private static bool _isLoaded = false;

        private static ConcurrentDictionary<(long, int), YwpMstYoukaiSkillLevel> _cache = new ();

        const float BEFRIENDER_SLT_PNT_DIVISOR = 187.5f;
        private static void EnsureLoaded()
        {
            if (!_isLoaded)
            {
                var gm = DataManager.Logic.DataManager.GameDataManager;
                _skillLevelEntries = new TableParser<YwpMstYoukaiSkillLevel>(gm.GetTableStringFromJson("ywp_mst_youkai_skill_level")).Items;
                _isLoaded = true;
            }
        }

        public static YwpMstYoukaiSkillLevel GetEntry(long youkaiId, int soultLevel)
        {
            EnsureLoaded();
            try
            {
                var entry = _cache[(youkaiId, soultLevel)];
                return entry;   
            }
            catch (KeyNotFoundException)
            {
                var entry = _skillLevelEntries.FirstOrDefault(x => x.YoukaiID == youkaiId && x.SoultLevel == soultLevel);
                if (entry == null) throw new KeyNotFoundException("Can't find soult data for befriender");
                _cache[(entry.YoukaiID, entry.SoultLevel)] = entry;
                return entry;
            }

        }
       
    }
}
