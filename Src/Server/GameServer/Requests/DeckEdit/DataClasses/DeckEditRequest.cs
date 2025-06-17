using Newtonsoft.Json;
namespace Puniemu.Src.Server.GameServer.Requests.DeckEdit.DataClasses
{
    //This is not the entire request structure, only the fields we want.
    public class DeckEditRequest
    {
        //Device ID
        [JsonProperty("deviceId")]
        public string? Udkey { get; set; }
        //Account ID
        [JsonProperty("level5UserId")]
        public string? Gdkey { get; set; }

        [JsonProperty("youkaiIdList")]
        public List<Dictionary<string,int>>? YoukaiIdList { get; set; }
    }
}
