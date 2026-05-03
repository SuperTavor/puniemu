using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.UpdateCollectMenu.DataClasses
{
    public class UpdateCollectMenuResponse : CommonResponse
    {
        // Basic user data.
        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }

        [JsonProperty("truncateItemList")]
        public object? TruncateItemList { get; set; } //IDK

        public UpdateCollectMenuResponse(YwpUserData userData)
        {
            this.ResultCode = 0;
            this.NextScreenType = 0;
            this.UserData = userData;
            this.ResultType = 0;
        }
    }


}
