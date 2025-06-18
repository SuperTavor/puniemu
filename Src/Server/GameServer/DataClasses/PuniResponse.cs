using Newtonsoft.Json;
using Puniemu.Src.Utils.GeneralUtils;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class PuniResponse
    {
        // The unix time when the response was sent.
        [JsonProperty("serverDt")]
        public long ServerDate = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        [JsonProperty("resultCode")]
        public int ResultCode { get; set; }

        [JsonProperty("nextScreenType")]
        public int NextScreenType { get; set; }

        [JsonProperty("resultType")]
        public int ResultType { get; set; }

        //Version of assets on the server.
        [JsonProperty("mstVersionMaster")]
        public int MstVersionMaster = int.Parse(ConfigManager.Logic.ConfigManager.GameDataManager!.GamedataCache["mstVersionMaster"]);
       
        // Constant.
        [JsonProperty("shopSaleList")]
        public List<int>? ShopSaleList { get; set; }

        // Constant.
        [JsonProperty("ymoneyShopSaleList")]
        public List<int> YMoneyShopSaleList = GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("ymoneyShopSaleList");

        // Constant.
        [JsonProperty("hitodamaShopSaleList")]
        public List<int>? HitodamaShopSaleList { get; set; }

        // Always empty.
        [JsonProperty("ywpToken")]
        public string YwpToken = "";

        [JsonProperty("token")]
        public string Token = "";

        [JsonProperty("dialogMsg")]
        public string DialogMsg = "";

        [JsonProperty("webServerIp")]
        public string WebServerIp = Consts.OG_GAMESERVER_URL;

        [JsonProperty("dialogTitle")]
        public string DialogTitle = "";

        [JsonProperty("storeUrl")]
        public string StoreUrl = "";

        [JsonProperty("mstVersionVer")]
        public int MstVersionVer { get; set; } // Version of assets on the server

    }
}
