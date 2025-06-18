using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses.Gacha.GachaStamp
{
    public class GachaStampIdentifier
    {
        [JsonProperty("gachaStampId")]
        public int GachaStampId { get; set; }

        //maybe related to the ones in mst_gacha??? idk
        [JsonProperty("gachaId")]
        public int GachaID { get; set; }

        [JsonProperty("endDt")]
        public string? ExpirationDate { get; set; }

        [JsonProperty("startDt")]
        public string? StartDate { get; set; }

        [JsonProperty("colorType")]
        public int ColorType { get; set; }
    }
}
