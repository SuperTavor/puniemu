using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.LoginStamp.DataClasses
{
    public class LoginStampReward
    {
        // item id
        [JsonProperty("rewardItemId")]
        public int RewardItemId = 0;
        // type of the item
        [JsonProperty("rewardItemType")]
        public RewardType RewardItemType = 0;
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
