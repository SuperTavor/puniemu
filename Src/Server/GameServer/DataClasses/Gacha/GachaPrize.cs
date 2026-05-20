using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class GachaPrize
    {
        [JsonProperty("item")]
        public object? Item { get; set; }

        [JsonProperty("capsuleColor")]
        public int CapsuleColor { get; set; }

        [JsonProperty("prizeType")]
        public int PrizeType { get; set; }

        [JsonProperty("icon")]
        public object? Icon { get; set; }

        [JsonProperty("ymoney")]
        public object? YMoney { get; set; }

        [JsonProperty("youkai")]
        public YokaiWonPopup? Youkai { get; set; }

        [JsonProperty("rarityType")]
        public RarityType RarityType { get; set; }

        [JsonProperty("convertItemInfo")]
        public object? ConvertItemInfo { get; set; }
    }
}
