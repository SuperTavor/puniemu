using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    // item popup
    public class ItemWonPopup
    {
        [JsonProperty("itemId")]
        public long ItemId = 0L;
        [JsonProperty("isLimitOver")]
        public int IsLimitOver = 0;
        [JsonProperty("cnt")]
        public int Count = 0;
    }
}