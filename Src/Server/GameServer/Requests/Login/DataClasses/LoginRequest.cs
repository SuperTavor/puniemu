using Newtonsoft.Json;
namespace Puniemu.Src.Server.GameServer.Requests.Login.DataClasses
{
    //This is not the entire request structure, only the fields we want.
    public class LoginRequest
    {
        //Device ID
        [JsonProperty("deviceId")]
        public string Udkey { get; set; }
        //Account ID
        [JsonProperty("gdkeyValue")]
        public string Gdkey { get; set; }
    }
}
