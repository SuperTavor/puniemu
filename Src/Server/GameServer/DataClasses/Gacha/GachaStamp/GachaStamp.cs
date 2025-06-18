using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses.Gacha.GachaStamp
{
    public class GachaStamp
    {
        //all of these are unknown except the really obvious ones. 
        [JsonProperty("rewardSetId")]
        public int RewardSetId;
        [JsonProperty("gachaStampId")]
        public int GachaStampId;
        [JsonProperty("cnt")]
        public int Count;
        [JsonProperty("description")]
        public string? Description;
        [JsonProperty("resourceName")]
        public string? ResourceName;
    }
}
