using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;

namespace Puniemu.Src.Server.GameServer.Requests.Rename.DataClasses
{
    public class RenameResponse : PuniResponse
    {
        // Basic user data.
        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }

        public RenameResponse(YwpUserData userData)
        {
            this.ResultCode = 0;
            this.NextScreenType = 0;
            this.UserData = userData;
            this.ResultType = 0;
        }
    }
}
