using Newtonsoft.Json;

// CREATED BY WIBWOB_YT

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class YwpUserGachaEntry
    {
        [JsonProperty("feverPctg")]
        public int FeverPctg { get; set; }

        [JsonProperty("gachaType")]
        public int GachaType { get; set; }
    }
}