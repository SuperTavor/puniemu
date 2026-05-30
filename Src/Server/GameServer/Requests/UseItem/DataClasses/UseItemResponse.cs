using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.UseItem.DataClasses
{
    public class UseItemResponse : CommonResponse
    {
        [JsonProperty("itemType")]
        public ItemType ItemType;

        [JsonProperty("ywp_user_dictionary")]
        public string YwpUserDictionary;

        [JsonProperty("ywp_user_icon_budge")]
        public string YwpUserIconBudge;

        [JsonProperty("ywp_user_youkai_skill")]
        public string YwpUserYoukaiSkill;

        [JsonProperty("ywp_user_youkai")]
        public string YwpUserYoukai;

        [JsonProperty("ywp_user_item")]
        public string YwpUserItem;

        [JsonProperty("youkaiExp")]
        public UserYoukaiResultListRes? YoukaiExp = null;

        [JsonProperty("youkaiSkillExp")]
        public UseItemSkillResult? YoukaiSkillExp = null;
    }
}
