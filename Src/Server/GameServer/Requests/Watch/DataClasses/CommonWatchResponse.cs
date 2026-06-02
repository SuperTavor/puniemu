using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses.Watch;

namespace Puniemu.Src.Server.GameServer.Requests.Watch.DataClasses
{
    public class CommonWatchResponse : CommonResponse
    {
        [JsonProperty("ywp_user_data")]
        public YwpUserData YwpUserData { get; set; }

        [JsonProperty("ywp_user_watch")]
        public List<YwpUserWatch> YwpUserWatch { get; set; }
    }
}
