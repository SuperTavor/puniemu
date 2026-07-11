using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.ReleaseYoukai.DataClasses
{
    public class ReleaseYoukaiResponse : CommonResponse
    {
        [JsonProperty("ywp_user_youkai")]
        public string UserYokai { get; set; }

        [JsonProperty("ywp_user_youkai_legend_release_history")]
        public string LegendReleaseHistory { get; set; }

        [JsonProperty("ywp_user_youkai_bonus_effect")]
        public string UserBonus { get; set; }

        [JsonProperty("ywp_user_youkai_skill")]
        public string UserSkill { get; set; }

        [JsonProperty("ywp_user_dictionary")]
        public string UserDict { get; set; }

        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }

        [JsonProperty("youkai")]
        public YokaiWonPopup Youkai { get; set; }
    }
}
