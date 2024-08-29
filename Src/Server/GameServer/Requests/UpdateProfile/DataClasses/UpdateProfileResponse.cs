using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.ConfigManager;
namespace Puniemu.Src.Server.GameServer.UpdateProfile.DataClasses
{
    public struct UpdateProfileResponse
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

        // Table that dictates which icon that the user has unlocked.
        [JsonProperty("ywp_user_player_icon")]
        public string UserPlayerIcon { get; set; }

        // Table that dictates which title that the user has unlocked.
        [JsonProperty("ywp_user_player_title")]
        public string UserPlayerTitle { get; set; }

        // 0 here.
        [JsonProperty("nextScreenType")]
        public int NextScreenType { get; set; }

        // Constant.
        [JsonProperty("hitodamaShopSaleList")]
        public List<int> HitodamaShopSaleList { get; set; }

        // Basic user data.
        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }

        // 0 here.
        [JsonProperty("resultType")]
        public int ResultType { get; set; }

        public UpdateProfileResponse(string userPlayerIcon, string userPlayerTitle, YwpUserData userData)
        {
            this.ShopSaleList = CGeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("shopSaleList");
            this.ServerDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            this.YwpToken = "";
            this.YMoneyShopSaleList = CGeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("ymoneyShopSaleList");
            this.MstVersionMaster = int.Parse(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["mstVersionMaster"]);
            this.ResultCode = 0;
            this.UserPlayerIcon = userPlayerIcon;
            this.UserPlayerTitle = userPlayerTitle;
            this.NextScreenType = 0;
            this.HitodamaShopSaleList = CGeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("hitodamaShopSaleList");
            this.UserData = userData;
            this.ResultType = 0;
        }
    }
}
