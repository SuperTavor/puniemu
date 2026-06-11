using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.Conflate.DataClasses
{
    public class ConflateResponse : CommonResponse
    {
        [JsonProperty("ywp_user_youkai")]
        public string YwpUserYoukai { get; set; }

        [JsonProperty("youkai")]
        public YokaiWonPopup Youkai { get; set; }

        [JsonProperty("ywp_user_menufunc")]
        public string YwpUserMenuFunc { get; set; }

        [JsonProperty("ywp_user_dictionary")]
        public string YwpUserDictionary { get; set; }

        [JsonProperty("ywp_user_youkai_skill")]
        public string YwpUserYoukaiSkill { get; set; }

        [JsonProperty("ywp_user_youkai_bonus_effect")]
        public string YwpUserBonusEffect { get; set; }

        [JsonProperty("ywp_user_icon_budge")]
        public string YwpUserIconBudge { get; set; }

        [JsonProperty("ywp_user_data")]
        public YwpUserData YwpUserData { get; set; }

        [JsonProperty("ywp_user_youkai_deck")]
        public string YwpUserYoukaiDeck { get; set; }

        [JsonProperty("ywp_user_item")]
        public string YwpUserItem { get; set; }
    }
}
