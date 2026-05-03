using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.InitGoku.DataClasses
{
    public class YwpUserGokuYoukaiIntroReleaseEntry
    {
        [JsonProperty("updateClearFlg")]
        public int UpdateClearFlg { get; set; }
        [JsonProperty("update")]
        public bool Update { get; set; }
        [JsonProperty("updateNowValue")]
        public int UpdateNowValue { get; set; }
        [JsonProperty("introReleaseId")]
        public long IntroReleasedId { get; set; }
        [JsonProperty("userId")]
        public string? UserId { get; set; }
        [JsonProperty("clearFlg")]
        public int ClearFlg { get; set; }
        [JsonProperty("readFlg")]
        public int ReadFlg { get; set; }
        [JsonProperty("missionType")]
        public int MissionType { get; set; }
        [JsonProperty("nowValue")]
        public int NowValue { get; set; }
        [JsonProperty("createRecord")]
        public bool CreateRecord { get; set; }
        [JsonProperty("targetValue")]
        public long TargetValue { get; set; }
        [JsonProperty("updateReadFlg")]
        public int UpdateReadFlg { get; set; }
    }
    public class InitGokuResponse : CommonResponse
    {
        [JsonProperty("ywp_mst_goku_story")]
        public List<Dictionary<string,object?>>? YwpMstGokuStory { get; set; }
        [JsonProperty("ywp_mst_goku_youkai_intro")]
        public List<Dictionary<string, object>>? YwpMstGokuYoukaiIntro { get; set; }
        [JsonProperty("ywp_mst_goku_youkai_intro_release")]
        public List<Dictionary<string, object>>? YwpMstGokuYoukaiIntroRelease { get; set; }
        [JsonProperty("ywp_mst_goku_menu")]
        public List<Dictionary<string, object>>? YwpMstGokuMenu { get; set; }
        [JsonProperty("ywp_user_icon_budge")]
        public string? YwpUserIconBudge { get; set; }
        [JsonProperty("ywp_user_goku_youkai_intro_release")]
        public List<YwpUserGokuYoukaiIntroReleaseEntry>? YwpUserGokuYoukaiIntroRelease { get; set; }
        [JsonProperty("ywp_user_data")]
        public YwpUserData? YwpUserData { get; set; }
        [JsonProperty("ywp_user_goku_story")]
        public List<Dictionary<string, object>>? YwpUserGokuStory { get; set; }

        

        private static void InitializeUserEventTables(JObject jo)
        {
            // Tables utilisateur d'événement - initialisées vides pour un nouvel utilisateur
            jo["ywp_user_goku_story"] = new JArray();
            jo["ywp_user_goku_youkai_intro_release"] = new JArray();
        }
    }
}