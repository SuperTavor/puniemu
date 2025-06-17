using Newtonsoft.Json;
namespace Puniemu.Src.Server.GameServer.Requests.GameStart.DataClasses
{
    //This is not the entire request structure, only the fields we want.
    public class GameStartRequest
    {
        //Device ID
        [JsonProperty("deviceId")]
        public string? Udkey { get; set; }
        //Account ID
        [JsonProperty("level5UserId")]
        public string? Gdkey { get; set; }

        [JsonProperty("stageId")]
        public int StageId { get; set; }
    }
}
