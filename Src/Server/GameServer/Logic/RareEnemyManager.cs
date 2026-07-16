using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Logic
{
    public class RareEnemyManager
    {
        private static List<RareEnemyEntry> _rareEnemies = null;

        private static void EnsureLoaded()
        {
            if (_rareEnemies == null)
                _rareEnemies = JsonConvert.DeserializeObject<List<RareEnemyEntry>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["rare_enemy"]);
        }
        //Returns -1 if no drop
        public static long GetDrop(int stageId)
        {
            EnsureLoaded();
            var mstStageItem = MasterStageData.StageItems.FirstOrDefault(x => x.StageId == stageId);
            if (mstStageItem == null) return -1;
            if(mstStageItem.BossFlag == 0)
            {
                //Try get stage specific rare yokai first
                var stageEntries = _rareEnemies
                        .Where(x => x.Scope == RareYokaiScope.StagesWide & x.Params.Contains(stageId));

                foreach (var entry in stageEntries)
                {
                    if (Random.Shared.Next(100) < entry.Rate)
                        return entry.EnemyID;
                }

                int mapId = stageId / 1000;
                //Now try map specific
                var mapEntry = _rareEnemies.FirstOrDefault(x =>
                  x.Scope == RareYokaiScope.MapsWide &&
                  x.Params.Contains(mapId) &&
                  (x.ExceptionParams == null || !x.ExceptionParams.Contains(stageId)));

                if (mapEntry != null && Random.Shared.Next(100) < mapEntry.Rate)
                {
                    return mapEntry.EnemyID;
                }

            }

            return -1; 
        }
    }
}
