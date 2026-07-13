using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses.Mission;
using Puniemu.Src.Server.GameServer.DataClasses.Mission.CustomMissionCfg;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using Puniemu.Src.UserDataManager.Logic;
using System.Collections.Immutable;

namespace Puniemu.Src.Server.GameServer.Logic
{
    public static class MissionManager
    {

        private static TableParser<YwpMstMission> _mstMission = null;

        //Which missions DONT carry over progress in the series
        private static readonly ImmutableHashSet<MissionType> NotCarryOver = [
            MissionType.BuySpecificItemAtShop,
            MissionType.UseSpecificItemInBattle,
            MissionType.BefriendSpecificYokai,
            MissionType.GetSpecificYokaiToLevel,
        ];

        //Which mission types need just a basic progress check
        private static readonly ImmutableHashSet<MissionType> BasicProgress = [
            MissionType.CollectTotalScore,
            MissionType.CollectTotalStars,
            MissionType.TotalCrank,
            MissionType.TotalLoginDays,
            MissionType.UseTotalItems,
            MissionType.AddTotalYokaiToMedallium,
            MissionType.FuseTotalYokai,
            MissionType.TotalPurchaseShop,
            MissionType.TotalScoreInScoreAttack,
            MissionType.CreateTotalBonusBalls,
            MissionType.DoTotalSoults,
            MissionType.PopTotalPuni,
            MissionType.EnterFeverTimeTotalTimes,
        ];

        //Which mission types just need a basic check with a param
        private static readonly ImmutableHashSet<MissionType> BasicParam = [
            MissionType.BefriendSpecificYokai,
            MissionType.BuySpecificItemAtShop,
            MissionType.UseSpecificItemInBattle
        ];
        private static async Task<TableParser<YwpUserMission>> GetUserMission(string gdkey)
        {
            var raw = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(gdkey, "ywp_user_mission");
            return new TableParser<YwpUserMission>(raw);
        }

        public static void SortUserMission(TableParser<YwpUserMission> userMission, int finishedMissionsIsAppear, bool killNewPopup)
        {
            List<YwpUserMission> newOrder = [];
            //First kill all the new mission popups
            foreach(var mission in userMission.Items)
            {
                if(mission.MissionCompleteStatus == MissionCompleteStatus.CompleteRewardAcquired) mission.IsAppear = finishedMissionsIsAppear;
                if((mission.NewStatus == MissionNewStatus.ShowNewPopup || mission.NewStatus == MissionNewStatus.ShowNewTag) && killNewPopup)
                {
                    mission.NewStatus = MissionNewStatus.None;
                }
            }
            //Add it by this order:
            //Missions with pending reward -> mission in progress -> missions done (with isAppear as false)
            newOrder.AddRange(userMission.Items.Where(x => x.MissionCompleteStatus == MissionCompleteStatus.CompletePendingReward));
            newOrder.AddRange(userMission.Items.Where(x => x.MissionCompleteStatus == MissionCompleteStatus.NotComplete));

            newOrder.AddRange(
                userMission.Items
                    .Where(x => x.MissionCompleteStatus == MissionCompleteStatus.CompleteRewardAcquired)
                    .OrderBy(x => x.MissionID)   
            );

            userMission.Items = newOrder;
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

        public static async Task TryUnlockNextMission(int missionId, TableParser<YwpUserMission> userMission, TableParser<YwpUserYoukai> userYokai)
        {
            var currentMissionIdx = userMission.Items.FindIndex(x => x.MissionID == missionId);
            var currentMission = userMission.Items[currentMissionIdx];
            //Append it at end and make it disappear
            currentMission.IsAppear = 0;
            userMission.Items.Remove(currentMission);
            userMission.Items.Add(currentMission);
            var idxResult = GetMissionCfgIdx(missionId);
            SeriesCfgItem seriesCfgItem = idxResult.Item1;
            int missionCfgIdx = idxResult.Item2;
            //Try unlock next mission in series
            int nextMissionCfgIdx = missionCfgIdx + 1;
            if (seriesCfgItem.Missions.Count > nextMissionCfgIdx)
            {
                var nextMissionCfgItem = seriesCfgItem.Missions[nextMissionCfgIdx];
                var newProgress = currentMission.MissionParamProgress;
                var newTarget = nextMissionCfgItem.Params[0];
                if (NotCarryOver.Contains(nextMissionCfgItem.MissionType)) newProgress = 0;
                if(nextMissionCfgItem.MissionType == MissionType.GetSpecificYokaiToLevel)
                {
                    int yokaiId = nextMissionCfgItem.Params[0];

                    var myYokai = userYokai.Items.FirstOrDefault(x => x.YoukaiId == yokaiId);

                    if(myYokai != null)
                    {
                        newProgress = myYokai.Level;
                    }

                    newTarget = nextMissionCfgItem.Params[1];
                }
                var newMission = new YwpUserMission()
                {
                    MissionID = nextMissionCfgItem.MissionID,
                    SeqNo = int.Parse("1" + nextMissionCfgItem.MissionID.ToString()),
                    MissionCompleteStatus = MissionCompleteStatus.NotComplete,
                    IsAppear = 1,
                    MissionParamTarget = newTarget,
                    MissionParamProgress = newProgress,
                    NewStatus = MissionNewStatus.ShowNewPopup,
                };
                if (newProgress >= newMission.MissionParamTarget) newMission.MissionCompleteStatus = MissionCompleteStatus.CompletePendingReward;
                userMission.Items.Insert(currentMissionIdx,newMission);
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

        private static void BasicParamCheck(YwpUserMission mission, int progressToUpdate, int param)
        {
            if (param == progressToUpdate)
            {
                mission.MissionParamProgress = 1;
                mission.MissionCompleteStatus = MissionCompleteStatus.CompletePendingReward;
            }
        }
        public static async Task<TableParser<YwpUserMission>> UpdateProgress(string gdkey, MissionType missionType, int progressToUpdate, 
            TableParser<YwpUserMission> paramUserMission = null, bool manualSave = false, int progressToUpdate2 = -1)
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
                    if(BasicProgress.Contains(missionType))
                    {
                        BasicProgressCheck(mission, progressToUpdate);
                    }
                    else if(BasicParam.Contains(missionType))
                    {
                        BasicParamCheck(mission, progressToUpdate, missionCfgItem.Params[0]);
                    }
                    else if(missionType == MissionType.GetSpecificYokaiToLevel)
                    {
                        //progressToUpdate is yokai ID
                        //progressToUpdate2 is yokai now level
                        if(progressToUpdate == missionCfgItem.Params[0])
                        {
                            mission.MissionParamProgress = progressToUpdate2;
                            if(mission.MissionParamProgress >= mission.MissionParamTarget)
                            {
                                mission.MissionCompleteStatus = MissionCompleteStatus.CompletePendingReward;
                            }
                        }
                    }
                    else if(missionType == MissionType.CompleteStageInSeconds)
                    {
                        var playerStageId = progressToUpdate;
                        var playerTime = progressToUpdate2;
                        var paramStageId = missionCfgItem.Params[0];
                        var paramTime = missionCfgItem.Params[1];

                        mission.MissionParamProgress = playerTime;

                        if (playerStageId == paramStageId && playerTime <= paramTime)
                        {
                            mission.MissionCompleteStatus = MissionCompleteStatus.CompletePendingReward;
                        }
                    }
                }
            }
            if (!manualSave)
                await SaveUserMission(gdkey, userMission);
            return userMission;
            
        }
    }
}
