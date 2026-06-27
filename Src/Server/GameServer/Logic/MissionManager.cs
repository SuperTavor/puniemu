using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses.Mission;
using Puniemu.Src.Server.GameServer.DataClasses.Mission.CustomMissionCfg;
using Puniemu.Src.TableParser.Logic;
using Puniemu.Src.UserDataManager.Logic;

namespace Puniemu.Src.Server.GameServer.Logic
{
    public static class MissionManager
    {

        private static TableParser<YwpMstMission> _mstMission = null;
        private static async Task<TableParser<YwpUserMission>> GetUserMission(string gdkey)
        {
            var raw = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(gdkey, "ywp_user_mission");
            return new TableParser<YwpUserMission>(raw);
        }

        private static async Task SaveUserMission(string gdkey, TableParser<YwpUserMission> userMission)
        {
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(gdkey, "ywp_user_mission", userMission.ToString());
        }

        private static readonly List<SeriesCfgItem> _seriesCfg = JsonConvert.DeserializeObject<List<SeriesCfgItem>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["mission_cfg"])!;
        private static TableParser<YwpMstMission> GetMstMission()
        {
            if(_mstMission == null)
            {
                var raw = DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_mission"];
                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(raw);
                _mstMission = new TableParser<YwpMstMission>((string)json["tableData"]);
            }
            return _mstMission;
        }

        public static async Task UpdateProgress(string gdkey, MissionType missionType, int progressToUpdate)
        {
            var userMission = await GetUserMission(gdkey);
            var mstMission = GetMstMission();
            //Get latest mission for type
            foreach(var mission in userMission.Items)
            {
                if (mission.MissionCompleteStatus != MissionCompleteStatus.NotComplete) continue;
                var mstEntry = mstMission.Items.Where(x => x.MissionID == mission.MissionID).FirstOrDefault();
                if (mstEntry == null) continue;
                if(mstEntry.MissionType == missionType)
                {
                    //Get cfg entry
                    int missionCfgIdx = -1;
                    SeriesCfgItem seriesCfgItem = null;
                    foreach (var tSeries in _seriesCfg)
                    {
                        var tCfgEntry = tSeries.Missions.FindIndex(x => x.MissionID == mstEntry.MissionID);
                        if (tCfgEntry != -1)
                        {
                            missionCfgIdx = tCfgEntry;
                            seriesCfgItem = tSeries;
                            break;
                        }
                    }
                    if (missionCfgIdx == -1 || seriesCfgItem == null)
                    {
                        throw new InvalidDataException("Can't find missionID in missionCfg: " + mission.MissionID);
                    }
                    var missionCfgItem = seriesCfgItem.Missions[missionCfgIdx];
                    Console.WriteLine($"[*] Updating mission for user ${gdkey}: \"{missionCfgItem.MissionName}\".");
                    if(missionType == MissionType.TotalPurchaseShop)
                    {
                        int newProgress = mission.MissionParamProgress += progressToUpdate;
                        if(newProgress >= mission.MissionParamTarget)
                        {
                            mission.MissionParamProgress = mission.MissionParamTarget;
                            mission.MissionCompleteStatus = MissionCompleteStatus.CompletePendingReward;
                            ////Try unlock next mission in series
                            //int nextMissionCfgIdx = missionCfgIdx + 1;
                            //if(seriesCfgItem.Missions.Count > nextMissionCfgIdx)
                            //{
                            //    var nextMissionCfgItem = seriesCfgItem.Missions[nextMissionCfgIdx];
                            //    var newMission = new YwpUserMission()
                            //    {
                            //        MissionID = nextMissionCfgItem.MissionID,
                            //        MissionIDWithSeries = nextMissionCfgItem.MissionID,
                            //        MissionCompleteStatus = MissionCompleteStatus.NotComplete,
                            //        IsAppear = 1,
                            //        MissionParamTarget = nextMissionCfgItem.Params[0],
                            //        MissionParamProgress = mission.MissionParamProgress,
                            //        NewStatus = MissionNewStatus.ShowNewPopup,
                            //    };
                            //}
                        }
                        else
                        {
                            mission.MissionParamProgress += progressToUpdate;
                        }
                    }
                    if(missionType == MissionType.TotalScoreInScoreAttack)
                    {
                        int newProgress = mission.MissionParamProgress += progressToUpdate;
                        if(newProgress >= mission.MissionParamTarget)
                        {
                            mission.MissionParamProgress = mission.MissionParamTarget;
                            mission.MissionCompleteStatus = MissionCompleteStatus.CompletePendingReward;
                        }
                    }

                    await SaveUserMission(gdkey, userMission);
                }
            }
            
        }
    }
}
