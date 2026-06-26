using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class YwpMstShopHitodama
    {
        [JsonProperty("bonusCnt")]
        public int BonusCount { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("goodsId")]
        public int GoodsID { get; set; }

        [JsonProperty("limitCnt")]
        public int LimitCount { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("sellCnt")]
        public int SellCount { get; set; }

        [JsonProperty("sort")]
        public int Sort { get; set; }
    }
}
