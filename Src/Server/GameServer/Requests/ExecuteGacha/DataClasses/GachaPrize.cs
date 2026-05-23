using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.DataClasses
{
    public class GachaPrize
    {
        [JsonProperty("item")]
        public ItemWonPopup? Item { get; set; }

        [JsonProperty("capsuleColor")]
        public CapsuleColor CapsuleColor { get; set; }

        [JsonProperty("prizeType")]
        public PrizeType PrizeType { get; set; }

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
