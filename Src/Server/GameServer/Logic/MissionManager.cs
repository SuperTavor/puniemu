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

        public static async Task SaveUserMission(string gdkey, TableParser<YwpUserMission> userMission)
        {
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(gdkey, "ywp_user_mission", userMission.ToString());
        }

        private static readonly List<SeriesCfgItem> _seriesCfg = JsonConvert.DeserializeObject<List<SeriesCfgItem>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["mission_cfg"])!;
        public static TableParser<YwpMstMission> GetMstMission()
        {
            if(_mstMission == null)
            {
                var raw = DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_mission"];
                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(raw);
                _mstMission = new TableParser<YwpMstMission>((string)json["tableData"]);
            }
            return _mstMission;
        }

        public static async void TryUnlockNextMission(int missionId, TableParser<YwpUserMission> userMission)
        {
            var currentMissionIdx = userMission.Items.FindIndex(x => x.MissionID == missionId);
            var currentMission = userMission.Items[currentMissionIdx];
            var idxResult = GetMissionCfgIdx(missionId);
            SeriesCfgItem seriesCfgItem = idxResult.Item1;
            int missionCfgIdx = idxResult.Item2;
            //Try unlock next mission in series
            int nextMissionCfgIdx = missionCfgIdx + 1;
            if (seriesCfgItem.Missions.Count > nextMissionCfgIdx)
            {
                var nextMissionCfgItem = seriesCfgItem.Missions[nextMissionCfgIdx];
                var newMission = new YwpUserMission()
                {
                    MissionID = nextMissionCfgItem.MissionID,
                    SeqNo = int.Parse("1" + nextMissionCfgItem.MissionID.ToString()),
                    MissionCompleteStatus = MissionCompleteStatus.NotComplete,
                    IsAppear = 1,
                    MissionParamTarget = nextMissionCfgItem.Params[0],
                    MissionParamProgress = currentMission.MissionParamProgress,
                    NewStatus = MissionNewStatus.ShowNewPopup,
                };
                
                userMission.Items.Insert(currentMissionIdx,newMission);
                //Append it at end and make it disappear
                currentMission.IsAppear = 0;
                userMission.Items.Remove(currentMission);
                userMission.Items.Add(currentMission);
            }
        }
        private static (SeriesCfgItem, int) GetMissionCfgIdx(int missionId)
        {
            int missionCfgIdx = -1;
            SeriesCfgItem seriesCfgItem = null;
            foreach (var tSeries in _seriesCfg)
            {
                var tCfgEntry = tSeries.Missions.FindIndex(x => x.MissionID == missionId);
                if (tCfgEntry != -1)
                {
                    missionCfgIdx = tCfgEntry;
                    seriesCfgItem = tSeries;
                    break;
                }
            }
            return (seriesCfgItem, missionCfgIdx);
        }

        private static void BasicProgressCheck(YwpUserMission mission, int progressToUpdate)
        {
            mission.MissionParamProgress += progressToUpdate;
            if (mission.MissionParamProgress >= mission.MissionParamTarget)
            {
                mission.MissionCompleteStatus = MissionCompleteStatus.CompletePendingReward;

            }
        }
        public static async Task<TableParser<YwpUserMission>> UpdateProgress(string gdkey, MissionType missionType, int progressToUpdate, 
            TableParser<YwpUserMission> paramUserMission = null, bool manualSave = false)
        {
            TableParser<YwpUserMission> userMission;
            if (paramUserMission == null)
                userMission = await GetUserMission(gdkey);
            else userMission = paramUserMission;
            var mstMission = GetMstMission();
            //Get latest mission for type
            foreach(var mission in userMission.Items)
            {
                var mstEntry = mstMission.Items.Where(x => x.MissionID == mission.MissionID).FirstOrDefault();
                var cfgIdx = GetMissionCfgIdx(mission.MissionID);
                if (cfgIdx.Item1 == null) continue;
                var missionCfgItem = cfgIdx.Item1.Missions[cfgIdx.Item2];
                if (mstEntry == null) continue;
                if(mstEntry.MissionType == missionType && mission.IsAppear == 1 && mission.MissionCompleteStatus != MissionCompleteStatus.CompleteRewardAcquired)
                {
                    Console.WriteLine($"[*] Checking mission for user ${gdkey}: \"{missionCfgItem.MissionName}\".");
                    if (missionType == MissionType.TotalPurchaseShop)
                    {
                        BasicProgressCheck(mission, progressToUpdate);
                    }
                    else if (missionType == MissionType.BuySpecificItemAtShop)
                    {
                        if (missionCfgItem.Params[0] == progressToUpdate)
                        {
                            mission.MissionParamProgress = 1;
                            mission.MissionCompleteStatus = MissionCompleteStatus.CompletePendingReward;
                        }
                    }
                    else if (missionType == MissionType.CollectTotalScore)
                    {
                        BasicProgressCheck(mission, progressToUpdate);
                    }
                    else if (missionType == MissionType.TotalCrank)
                    {
                        BasicProgressCheck(mission, progressToUpdate);
                    }
                    else if (missionType == MissionType.CollectTotalStars)
                    {
                        BasicProgressCheck(mission, progressToUpdate);
                    }
                    else if (missionType == MissionType.CreateTotalBonusBalls)
                    {
                        BasicProgressCheck(mission, progressToUpdate);
                    }
                    else if (missionType == MissionType.DoTotalSoults)
                    {
                        BasicProgressCheck(mission, progressToUpdate);
                    }
                    else if (missionType == MissionType.EnterFeverTimeTotalTimes)
                    {
                        BasicProgressCheck(mission, progressToUpdate);
                    }
                    else if (missionType == MissionType.UseTotalItems)
                    {
                        BasicProgressCheck(mission, progressToUpdate);
                    }
                    else if(missionType == MissionType.TotalLoginDays)
                    {
                        BasicProgressCheck(mission, progressToUpdate);
                    }
                    else if(missionType == MissionType.TotalScoreInScoreAttack)
                    {
                        BasicProgressCheck(mission, progressToUpdate);
                    }
                    else if(missionType == MissionType.PopTotalPuni)
                    {
                        BasicProgressCheck(mission, progressToUpdate);
                    }
                    else if (missionType == MissionType.FuseTotalYokai)
                    {
                        BasicProgressCheck(mission, progressToUpdate);
                    }
                    if (!manualSave)
                        await SaveUserMission(gdkey, userMission);
                }
            }
            return userMission;
            
        }
    }
}
