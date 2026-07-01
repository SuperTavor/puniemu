using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.GetMission.DataClasses
{
    public class GetMissionResponse : CommonResponse
    {
        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }

        [JsonProperty("ywp_mst_mission")]
        public string MstMission { get; set; }

        [JsonProperty("ywp_user_mission")]

        public string UserMission { get; set; }

        [JsonProperty("ywp_mst_daily_event_mission")]
        public string MstDailyMission { get; set; }
    }
}
