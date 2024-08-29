using Newtonsoft.Json;
using Puniemu.Src.ConfigManager.Logic;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.UpdateTutorialFlag.DataClasses
{
    public class UpdateTutorialFlagResponse
    {
        // Timestamp of when the response was sent.
        [JsonProperty("serverDt")]
        public long ServerDt { get; set; }

        // Version of assets on the server.
        [JsonProperty("mstVersionMaster")]
        public int MstVersionMaster { get; set; }

        // 0 here.
        [JsonProperty("resultCode")]
        public int ResultCode { get; set; }

        // Table of completed tutorials for the user.
        [JsonProperty("ywp_user_tutorial_list")]
        public string TutorialList { get; set; }

        // 0 here.
        [JsonProperty("nextScreenType")]
        public int NextScreenType { get; set; }

        // Basic user data.
        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }

        // 0 here.
        [JsonProperty("resultType")]
        public int ResultType { get; set; }

        public UpdateTutorialFlagResponse(string tutorialList,YwpUserData userData)
        {
            this.ServerDt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            this.MstVersionMaster = int.Parse(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["mstVersionMaster"]);
            this.ResultCode = 0;
            this.TutorialList = tutorialList;
            this.NextScreenType = 0;
            this.UserData = userData;
            this.ResultType = 0;
        }
    }


}
