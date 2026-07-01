using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.GetMissionAlreadyReward.DataClasses
{
    public class GetMissionAlreadyRewardResponse : CommonResponse
    {
        [JsonProperty("ywp_user_mission")]
        public string UserMission { get; set; }
    }
}
