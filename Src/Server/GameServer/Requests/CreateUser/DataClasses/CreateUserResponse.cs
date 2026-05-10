using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.UpdateTutorialFlag.DataClasses.WibWob;
namespace Puniemu.Src.Server.GameServer.Requests.CreateUser.DataClasses
{
    public class CreateUserResponse : CommonResponse
    {

        // Empty.
        [JsonProperty("rewardList")]
        public List<object> RewardList { get; set; }

        // Table that dictates which tutorial flags the user has completed.
        [JsonProperty("ywp_user_tutorial_list")]
        public object UserTutorialList { get; set; }

        // Basic user data.
        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }


        public CreateUserResponse(string userTutorialList, YwpUserData userData)
        {
            RewardList = new List<object>();
            ResultCode = 0;
            if (DataManager.Logic.DataManager.IsWibWob) UserTutorialList = JsonConvert.DeserializeObject<List<Tutorial>>(userTutorialList)!;
            else UserTutorialList = userTutorialList;
            NextScreenType = 0;
            UserData = userData;
            ResultType = 0;
        }
    }
}
