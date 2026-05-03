using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.UpdateGokuStory.DataClasses;
using System.Collections.Generic;

namespace Puniemu.Src.Server.GameServer.Requests.UpdateGokuStory.DataClasses
{
    public class UpdateGokuStoryResponse : CommonResponse
    {
        [JsonProperty("ywp_user_icon_budge")]
        public string? YwpUserIconBudge { get; set; }

        [JsonProperty("ywp_user_data")]
        public YwpUserData? YwpUserData { get; set; }

        [JsonProperty("ywp_user_goku_story")]
        public List<YwpUserGokuStoryEntry>? YwpUserGokuStory { get; set; }
    }
}
