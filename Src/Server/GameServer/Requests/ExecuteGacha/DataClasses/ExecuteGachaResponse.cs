using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Net.Http;
using System.Text;
using Supabase;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text.RegularExpressions;



namespace Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.DataClasses
{
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
