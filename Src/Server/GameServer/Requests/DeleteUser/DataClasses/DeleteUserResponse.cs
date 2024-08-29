using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.ConfigManager;
namespace Puniemu.Src.Server.GameServer.Requests.DeleteUser.DataClasses
{
    public struct DeleteUserResponse
    {
        // Constant.
        [JsonProperty("shopSaleList")]
        public List<int> ShopSaleList { get; set; }

        // The unix time when the response was sent.
        [JsonProperty("serverDt")]
        public long ServerDate { get; set; }

        // Always empty.
        [JsonProperty("ywpToken")]
        public string YwpToken { get; set; }

        // Constant.
        [JsonProperty("ymoneyShopSaleList")]
        public List<int> YMoneyShopSaleList { get; set; }

        // Version of assets/data on the server.
        [JsonProperty("mstVersionMaster")]
        public int MstVersionMaster { get; set; }

        // 0 here.
        [JsonProperty("resultCode")]
        public int ResultCode { get; set; }

        // Response Code (0 sucess or 1 error)
        [JsonProperty("responseCode")]
        public int ResponseCode { get; set; }

        // 0 here.
        [JsonProperty("nextScreenType")]
        public int NextScreenType { get; set; }

        // null here.
        [JsonProperty("token")]
        public string?  Token { get; set; }
        // null here.
        
        [JsonProperty("dialogMsg")]
        public string DialogMsg { get; set; }
        // null here.
        
        [JsonProperty("webServerIp")]
        public string WebServerIp { get; set; }
        // null here.
        
        [JsonProperty("storeUrl")]
        public string StoreUrl { get; set; }

        [JsonProperty("dialogTitle")]
        public string DialogTitle { get; set; }

        // Constant.
        [JsonProperty("hitodamaShopSaleList")]
        public List<int> HitodamaShopSaleList { get; set; }


        // 0 here.
        [JsonProperty("resultType")]
        public int ResultType { get; set; }

        public DeleteUserResponse(int RespCode)
        {
            this.ShopSaleList = GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("shopSaleList");
            this.ServerDate = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            this.YwpToken = "";
            this.Token = "hello_dummy";
            this.YMoneyShopSaleList = GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("ymoneyShopSaleList");
            this.MstVersionMaster = int.Parse(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["mstVersionMaster"]);
            this.ResultCode = 0;
            this.DialogTitle = "";
            this.StoreUrl = "";
            this.DialogMsg = "";
            this.WebServerIp = "";
            this.ResponseCode = RespCode;
            this.NextScreenType = 0;
            this.HitodamaShopSaleList = GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("hitodamaShopSaleList");
            this.ResultType = 0;
        }
    }
}