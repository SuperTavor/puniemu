using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.EvolveYoukai.DataClasses
{
    public class EvolveYoukaiResponse : CommonResponse
    {
        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }
        [JsonProperty("ywp_user_youkai")]
        public string UserYoukai { get; set; }

        [JsonProperty("ywp_user_dictionary")]
        public string UserDict { get; set; }

        [JsonProperty("ywp_user_youkai_bonus_effect")]
        public string UserBonus { get; set; }

        [JsonProperty("ywp_user_youkai_skill")]
        public string UserSkill { get; set; }

        [JsonProperty("ywp_user_youkai_deck")]
        public string UserDeck { get; set; }

        [JsonProperty("youkai")]
        public YokaiWonPopup Popup { get; set; }
    }
}
