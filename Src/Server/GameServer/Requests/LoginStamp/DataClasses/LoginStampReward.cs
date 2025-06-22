using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.LoginStamp.DataClasses
{
    public class LoginStampReward
    {
        // item id
        [JsonProperty("rewardItemId")]
        public long RewardItemId = 0L;
        // type of the item
        [JsonProperty("rewardItemType")]
        public int RewardItemType = 0;
        // stamp id
        [JsonProperty("stampId")]
        public int StampId = 0;
        // reward day
        [JsonProperty("rewardDayCnt")]
        public int RewardDayCnt = 0;
        // number of item
        [JsonProperty("rewardItemCnt")]
        public int RewardItemCnt = 0;
    }
}
