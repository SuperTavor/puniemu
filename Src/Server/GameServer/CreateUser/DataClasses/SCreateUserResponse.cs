using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.ConfigManager;
namespace Puniemu.Src.Server.GameServer.CreateUser.DataClasses
{
    public struct SCreateUserResponse
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

        // Empty.
        [JsonProperty("rewardList")]
        public List<object> RewardList { get; set; }

        // 0 here.
        [JsonProperty("resultCode")]
        public int ResultCode { get; set; }

        // Table that dictates which tutorial flags the user has completed.
        [JsonProperty("ywp_user_tutorial_list")]
        public string UserTutorialList { get; set; }

        // 0 here.
        [JsonProperty("nextScreenType")]
        public int NextScreenType { get; set; }

        // Constant.
        [JsonProperty("hitodamaShopSaleList")]
        public List<int> HitodamaShopSaleList { get; set; }

        // Basic user data.
        [JsonProperty("ywp_user_data")]
        public SYwpUserData UserData { get; set; }

        // 0 here.
        [JsonProperty("resultType")]
        public int ResultType { get; set; }

        public SCreateUserResponse(string userTutorialList, SYwpUserData userData)
        {
            this.ShopSaleList = CGeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("shopSaleList");
            this.ServerDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            this.YwpToken = "";
            this.YMoneyShopSaleList = CGeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("ymoneyShopSaleList");
            this.MstVersionMaster = int.Parse(CConfigManager.GameDataManager.GamedataCache["mstVersionMaster"]);
            this.RewardList = new List<object>();
            this.ResultCode = 0;
            this.UserTutorialList = userTutorialList;
            this.NextScreenType = 0;
            this.HitodamaShopSaleList = CGeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("hitodamaShopSaleList");
            this.UserData = userData;
            this.ResultType = 0;
        }
    }
}
