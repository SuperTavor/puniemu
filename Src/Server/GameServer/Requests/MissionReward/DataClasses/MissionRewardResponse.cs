using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text.Json.Serialization;

namespace Puniemu.Src.Server.GameServer.Requests.MissionReward.DataClasses
{
    public class MissionRewardResponse
    {
        [JsonProperty("ywp_mst_mission")]
        public string MstMission { get; set; }

        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }

        [JsonProperty("responseRewardStatus")]
        public int ResponseRewardStatus = 1;

        [JsonProperty("item")]
        public ItemWonPopup Item = null;

        [JsonProperty("youkai")]
        public YokaiWonPopup Youkai = null;

        [JsonProperty("ywp_user_mission")]
        public string UserMission { get; set; }

        [JsonProperty("ywp_user_item")]
        public string UserItem { get; set; }

        [JsonProperty("ywp_user_player_icon")]
        public string UserIcon { get; set; }

        [JsonProperty("ywp_user_player_title")]
        public string UserTitle { get; set; }

        [JsonProperty("ywp_user_youkai")]
        public string UserYoukai { get; set; }

        [JsonProperty("ywp_user_youkai_skill")]
        public string UserSkill { get; set; }

        [JsonProperty("ywp_user_shop_item_unlock")]
        public string UserShop { get; set; }
        [JsonProperty("ywp_user_youkai_bonus_effect")]
        public string UserBonus { get; set; }

        [JsonProperty("rewardUpMissionIdList")]
        public List<int> RewardUpMissionIDList = [];
    }
}
