using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;

namespace Puniemu.Src.Server.GameServer.Requests.MapUnLock.DataClasses
{
    public class MapUnLockResponse : CommonResponse
    {
        // Basic user data.
        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }

        [JsonProperty("ywp_user_stage")]
        public string YwpUserStage { get; set; }

        [JsonProperty("ywp_user_map")]
        public string YwpUserMap { get; set; }
        public MapUnLockResponse(YwpUserData userData, string userStage, string userMap)
        {
            this.YwpUserMap = userMap;
            this.YwpUserStage = userStage;
            this.UserData = userData;
        }
    }
}
