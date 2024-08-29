using Newtonsoft.Json;
namespace Puniemu.Src.Server.GameServer.Requests.UpdateTutorialFlag.DataClasses
{
    public class UpdateTutorialFlagRequest
    {
        //udkey
        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }
        //gdkey
        [JsonProperty("level5UserId")]
        public string Level5UserId { get; set; }
        //ID of the tutorial to be set 
        [JsonProperty("tutorialId")]
        public int TutorialId { get; set; }
        //Status to give that tutorial (e.g. enabled disabled)
        [JsonProperty("tutorialStatus")]
        public int TutorialStatus { get; set; }
        //Type of that tutorial. idk what this is
        [JsonProperty("tutorialType")]
        public int TutorialType { get; set; }
    }
}
