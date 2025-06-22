using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.ConfigManager;
namespace Puniemu.Src.Server.GameServer.Requests.LoginStamp.DataClasses
{
    public class LoginStampResponse : PuniemuResponseBase
    {
        // Basic user data.
        [JsonProperty("ywp_user_data")]
        public YwpUserData ?UserData { get; set; }
        [JsonProperty("ywp_mst_login_stamp_reward")]
        public List<LoginStampReward> ?LoginStampReward { get; set; }
        [JsonProperty("ywp_mst_login_stamp")]
        public List<LoginStampType>? LoginStampRes { get; set; }
        [JsonProperty("ywp_user_login_stamp_list")]
        public List<UserLoginStamp>? UserLoginStamp { get; set; }
        [JsonProperty("stampDt")]
        public string ?StampDt { get; set; }
        [JsonProperty("responseCode")]
        public int ResponseCode = 0;
        [JsonProperty("directDistFlg")]
        public int DirectDistFlg = 1;
        [JsonProperty("youkai")]
        public YokaiWonPopup? YoukaiPopupResult { get; set; }
        [JsonProperty("item")]
        public ItemWonPopup? ItemPopupResult { get; set; }

        public LoginStampResponse() 
        {
            this.ResultCode = 0;
            this.NextScreenType = 0;
            this.ResultType = 0;
        }
    }
}
