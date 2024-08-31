using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
namespace Puniemu.Src.Server.GameServer.Requests.CreateUser.DataClasses
{
    public class CreateUserResponse : PuniemuResponseBase
    {

        // Empty.
        [JsonProperty("rewardList")]
        public List<object> RewardList { get; set; }

        // Table that dictates which tutorial flags the user has completed.
        [JsonProperty("ywp_user_tutorial_list")]
        public string UserTutorialList { get; set; }

        // Basic user data.
        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }


        public CreateUserResponse(string userTutorialList, YwpUserData userData)
        {
            RewardList = new List<object>();
            ResultCode = 0;
            UserTutorialList = userTutorialList;
            NextScreenType = 0;
            UserData = userData;
            ResultType = 0;
        }
    }
}
