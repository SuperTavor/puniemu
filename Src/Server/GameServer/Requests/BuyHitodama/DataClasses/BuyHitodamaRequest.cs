using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.BuyHitodama.DataClasses
{
    public class BuyHitodamaRequest
    {
        //Device id
        [JsonProperty("deviceId")]
        public string Udkey { get; set; }
        //Account ID
        [JsonProperty("level5UserId")]
        public string Gdkey { get; set; }
        //ID of hitodama shop item to buy
        [JsonProperty("goodsId")]
        public int GoodsId { get; set; }
    }
}

