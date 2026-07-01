using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses.Mission
{
    public class MissionReward
    {
        [JsonProperty("itemId")]
        public long RewardID { get; set; } //Not strictly an item ID, a reward ID

        [JsonProperty("itemType")]
        public RewardType RewardType { get; set; } //a reward type. not itemType

        [JsonProperty("itemCnt")]
        public int RewardCount { get; set; } //for items its item count, for ymoney its the amoutn of ymoney etc.

        [JsonProperty("isLimitOver")]
        public bool IsLimitOver = false; //Unk

    }
}
