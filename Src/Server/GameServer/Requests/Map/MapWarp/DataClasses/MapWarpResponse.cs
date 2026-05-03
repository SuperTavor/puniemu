using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;

namespace Puniemu.Src.Server.GameServer.Requests.MapWarp.DataClasses
{
    public class MapWarpResponse : CommonResponse
    {
        // Basic user data.
        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }

        [JsonProperty("teamEventButtonHiddenFlg")]
        public int TeamEventButtonHiddenFlg { get; set; }
        public MapWarpResponse(YwpUserData userData)
        {
            this.UserData = userData;
        }
    }
}
