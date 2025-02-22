using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.ConfigManager;
namespace Puniemu.Src.Server.GameServer.Requests.UpdateProfile.DataClasses
{
    public class UpdateProfileResponse : PuniemuResponseBase
    {

        // Table that dictates which icon that the user has unlocked.
        [JsonProperty("ywp_user_player_icon")]
        public string UserPlayerIcon { get; set; }

        // Table that dictates which title that the user has unlocked.
        [JsonProperty("ywp_user_player_title")]
        public string UserPlayerTitle { get; set; }

        // Table that dictates which plate that the user has unlocked.
        [JsonProperty("ywp_user_player_plate")]
        public string UserPlayerPlate { get; set; }

        // Table that dictates which codename that the user has unlocked.
        [JsonProperty("ywp_user_player_codename")]
        public string UserPlayerCodename { get; set; }

        // Table that dictates which effect that the user has unlocked.
        [JsonProperty("ywp_user_player_effect")]
        public string UserPlayerEffect { get; set; }

        // Basic user data.
        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }

        public UpdateProfileResponse(string userPlayerIcon, string userPlayerTitle, string userPlayerPlate, string userPlayerEffect, string userPlayerCodename,  YwpUserData userData)
        {
            this.ResultCode = 0;
            this.UserPlayerPlate = userPlayerPlate;
            this.UserPlayerEffect = userPlayerEffect;
            this.UserPlayerCodename = userPlayerCodename;
            this.UserPlayerIcon = userPlayerIcon;
            this.UserPlayerTitle = userPlayerTitle;
            this.NextScreenType = 0;
            this.UserData = userData;
            this.ResultType = 0;
        }
    }
}
