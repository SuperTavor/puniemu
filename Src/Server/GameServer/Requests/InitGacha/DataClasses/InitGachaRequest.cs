using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.InitGacha.DataClasses
{
    public class InitGachaRequest
    {

        [JsonProperty("deviceId")]
        public string UdKey { get; set; }

        [JsonProperty("level5UserId")]
        public string GdKey { get; set; }

        public InitGachaRequest(string udkey, string gdkey)
        {
            GdKey = gdkey;
            UdKey = udkey;
        }

    }
}
