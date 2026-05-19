using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.BuyItem.DataClasses
{
    public class BuyItemRequest
    {
        [JsonProperty("level5UserId")]
        public string Gdkey { get; set; }

        //item id for what is being bought
        [JsonProperty("goodsId")]
        public int GoodsId { get; set; }

        //How much is being bought?
        [JsonProperty("cnt")]
        public int GoodsCount { get; set; }
    }
}
