using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.MissionReward.DataClasses
{
    public class MissionRewardRequest : CommonRequest
    {
        [JsonProperty("forceFlg")]
        public int ForceFlag { get; set; } //Unk

        [JsonProperty("missionId")]
        public int MissionID { get; set; } //MIssion id to get the rewarxd


    }
}
