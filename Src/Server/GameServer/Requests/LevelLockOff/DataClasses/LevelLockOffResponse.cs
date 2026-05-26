using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.LevelLockOff.DataClasses
{
    public class LevelLockOffResponse : CommonResponse
    {
        [JsonProperty("ywp_user_youkai")]
        public string UserYoukai { get; set; }

        [JsonProperty("ywp_user_youkai_skill")]
        public string UserYoukaiSkill { get; set; }

        [JsonProperty("ywp_user_youkai_bonus_effect")]
        public string UserYoukaiBonusEffect { get; set; }

        [JsonProperty("ywp_user_youkai_strong_skill")]
        public string UserYoukaiStrongSkill { get; set; }

        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }
    }
}
