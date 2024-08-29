using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.ConfigManager;
namespace Puniemu.Src.Server.GameServer.Requests.UserStageRanking.DataClasses
{
    public struct UserStageRankingResponse
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

        // 0 here.
        [JsonProperty("nextScreenType")]
        public int NextScreenType { get; set; }

        // Constant.
        [JsonProperty("hitodamaShopSaleList")]
        public List<int> HitodamaShopSaleList { get; set; }

        // Basic user data.
        [JsonProperty("ywp_user_stage_rank")]
        public List<object> StageRankData { get; set; }

        // 0 here.
        [JsonProperty("resultType")]
        public int ResultType { get; set; }
        
        // empty
        [JsonProperty("dialogMsg")]
        public string DialogMsg { get; set; }

        [JsonProperty("webServerIp")]
        public string WebServerIp { get; set; }

        [JsonProperty("storeUrl")]
        public string StoreUrl { get; set; }

        [JsonProperty("dialogTitle")]
        public string DialogTitle { get; set; }
        public UserStageRankingResponse(List<object> newListData)
        {
            this.ShopSaleList = CGeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("shopSaleList");
            this.ServerDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            this.YwpToken = "";
            this.YMoneyShopSaleList = CGeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("ymoneyShopSaleList");
            this.MstVersionMaster = int.Parse(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["mstVersionMaster"]);
            this.ResultCode = 0;
            this.DialogMsg = "";
            this.WebServerIp = "";
            this.StoreUrl = "";
            this.DialogTitle = "";
            this.NextScreenType = 0;
            this.HitodamaShopSaleList = CGeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("hitodamaShopSaleList");
            this.StageRankData = newListData;
            this.ResultType = 0;
        }
    }
}
