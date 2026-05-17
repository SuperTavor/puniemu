using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.Init.InitGacha.DataClasses.WibWob
{
    public class WibWobUserGachaEntry
    {
        [JsonProperty("gachaType")]
        public int GachaType { get; set; }

        [JsonProperty("feverPctg")]
        public int FeverPctg { get; set; }
    }
}
