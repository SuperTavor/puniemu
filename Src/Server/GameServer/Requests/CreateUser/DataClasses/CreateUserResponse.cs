using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.ConfigManager.Logic;
namespace Puniemu.Src.Server.GameServer.Requests.CreateUser.DataClasses
{
    public struct CreateUserResponse
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
        public YwpUserData UserData { get; set; }

        // 0 here.
        [JsonProperty("resultType")]
        public int ResultType { get; set; }

        public CreateUserResponse(string userTutorialList, YwpUserData userData)
        {
            ShopSaleList = CGeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("shopSaleList");
            ServerDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            YwpToken = "";
            YMoneyShopSaleList = CGeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("ymoneyShopSaleList");
            MstVersionMaster = int.Parse(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["mstVersionMaster"]);
            RewardList = new List<object>();
            ResultCode = 0;
            UserTutorialList = userTutorialList;
            NextScreenType = 0;
            HitodamaShopSaleList = CGeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("hitodamaShopSaleList");
            UserData = userData;
            ResultType = 0;
        }
    }
}
