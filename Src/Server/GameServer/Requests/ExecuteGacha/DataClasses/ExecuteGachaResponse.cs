using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.DataClasses
{
    //[JsonConverter(typeof(ExecuteGachaResponseConverter))]
    public class ExecuteGachaResponse : CommonResponse
    {
        [JsonProperty("ywp_user_tutorial_list")]
        public string? YwpUserTutorialList { get; set; }

        [JsonProperty("ywp_user_event")]
        public string? YwpUserEvent { get; set; }

        [JsonProperty("canPossessionItemList")]
        public string? CanPossessionItemList { get; set; }

        [JsonProperty("gachaPrizeList")]
        public GachaPrize[]? GachaPrizeList { get; set; }

        [JsonProperty("effectType")]
        public int EffectType { get; set; }

        [JsonProperty("ywp_user_youkai_strong_skill_diff")]
        public string? YwpUserYoukaiStrongSkillDiff { get; set; }
    }
}
