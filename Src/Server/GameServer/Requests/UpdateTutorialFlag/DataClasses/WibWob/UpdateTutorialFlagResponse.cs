using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.UpdateTutorialFlag.DataClasses.WibWob
{
    public class UpdateTutorialFlagResponse: CommonResponse
    {

        // Table of completed tutorials for the user.
        [JsonProperty("ywp_user_tutorial_list")]
        public List<Tutorial> TutorialList { get; set; }

        // Basic user data.
        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }

        public UpdateTutorialFlagResponse(List<Tutorial> tutorialList,YwpUserData userData)
        {
            this.ResultCode = 0;
            this.TutorialList = tutorialList;
            this.NextScreenType = 0;
            this.UserData = userData;
            this.ResultType = 0;
        }
    }


}
