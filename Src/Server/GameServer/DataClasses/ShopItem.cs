using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class ShopItem
    {
        [JsonProperty("baseGoodsId")]
        public int BaseGoodsId { get; set; }

        [JsonProperty("bonusPctg")]
        public int BonusPctg { get; set; }

        [JsonProperty("goodsId")]
        public int GoodsId { get; set; }

        [JsonProperty("itemId")]
        public int ItemId { get; set; }

        [JsonProperty("limitCnt")]
        public int LimitCnt { get; set; }

        [JsonProperty("lockConditionFlg")]
        public int LockConditionFlg { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("salePeriodNo")]
        public int SalePeriodNo { get; set; }

        [JsonProperty("sort")]
        public int Sort { get; set; }
    }
}
