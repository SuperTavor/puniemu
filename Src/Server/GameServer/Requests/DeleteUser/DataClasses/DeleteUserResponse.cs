using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.ConfigManager;
namespace Puniemu.Src.Server.GameServer.Requests.DeleteUser.DataClasses
{
    public class DeleteUserResponse: PuniemuResponseBase
    {

        // Response Code (0 sucess or 1 error)
        [JsonProperty("responseCode")]
        public int ResponseCode { get; set; }

        public DeleteUserResponse(int RespCode)
        {
            this.ResultCode = 0;
            this.ResponseCode = RespCode;
            this.NextScreenType = 0;
            this.ResultType = 0;
        }
    }
}