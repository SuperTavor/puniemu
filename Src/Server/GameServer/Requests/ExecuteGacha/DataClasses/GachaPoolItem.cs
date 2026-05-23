using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.DataClasses
{
    public class GachaPoolItem
    {
        [JsonProperty("gachaId")]
        public int GachaId { get; set; }

        [JsonProperty("weights")]
        public Dictionary<string, double> Weights { get; set; }

        [JsonProperty("youkai")]
        public Dictionary<string, List<long>> Yokais { get; set; }

        [JsonProperty("items")]
        public Dictionary<string, List<long>> Items { get; set; }
    }
}
