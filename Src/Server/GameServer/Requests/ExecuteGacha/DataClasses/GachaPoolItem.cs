using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.DataClasses
{
    public class GachaPoolItem
    {
        [JsonProperty("gachaId")]
        public int GachaId { get; set; }

        [JsonProperty("weights")]
        public Dictionary<RarityType, double> Weights { get; set; }

        [JsonProperty("youkai")]
        public Dictionary<RarityType, List<long>> Yokais { get; set; }
    }
}
