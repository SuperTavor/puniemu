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

        public MapUnLockResponse(YwpUserData userData)
        {
            this.UserData = userData;
        }
    }
}
