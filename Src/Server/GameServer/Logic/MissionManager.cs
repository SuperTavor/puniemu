using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses.Mission;
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

        private static readonly Dictionary<int, List<ExtraParamMissionItem>> _missionCfg = JsonConvert.DeserializeObject<Dictionary<int, List<ExtraParamMissionItem>>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["mission_cfg"])!;
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
        //Param is the mission target param since some mission types can for example be - Buy item at shop, the param will be which item!
        //Param == -1 means no extra param
        public static async Task UpdateProgress(string gdkey, MissionType missionType, int progress, int param = -1)
        {
            var userMission = await GetUserMission(gdkey);
            var mstMission = GetMstMission();
            //Get latest mission for type
            foreach(var mission in userMission.Items)
            {
                var mstEntry = mstMission.Items.Where(x => x.MissionID == mission.MissionID).FirstOrDefault();
                var missionFirstParam = mission.MissionParamTarget;
                ExtraParamMissionItem? extraParamCfgEntry = null;
                if(param != -1)
                    //extraParamCfgEntry = _missionCfg.Values.Where(x.wh)
                if(mstEntry.MissionType == missionType)
                {
                    
                }
            }
            
        }
    }
}
