using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.UseAddition.DataClasses
{
    public class UseAdditionResponse : CommonResponse
    {
        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }

        [JsonProperty("responseCode")]
        public int ResponseCode { get; set; }

        [JsonProperty("responseDetailCode")]
        public int ResponseDetailCode { get; set; }


        public void SetUsedToday()
        {
            ResponseCode = 1;
            ResponseDetailCode = 1;
        }

        public void SetNormalShrine()
        {
            ResponseCode = 0;
            ResponseDetailCode = 1;
        }

        public void SetSuperShrine()
        {
            ResponseCode = 0;
            ResponseDetailCode = 0;
        }
    }
}
