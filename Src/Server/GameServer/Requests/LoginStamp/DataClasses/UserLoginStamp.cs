using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.LoginStamp.DataClasses
{
    public class UserLoginStamp
    {
        // item id
        [JsonProperty("loginDayCnt")]
        public int LoginDayCnt = 0;
        // type of the item
        [JsonProperty("userId")]
        public string ?UserId = "";
        // stamp id
        [JsonProperty("stampId")]
        public int StampId = 0;
        // reward day
        [JsonProperty("isStep")]
        public int IsStep = 0;
    }
}
