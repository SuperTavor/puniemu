using Newtonsoft.Json;
using Puniemu.Src.Utils.GeneralUtils;

namespace Puniemu.Src.Server.GameServer.Requests.Login.DataClasses
{
    public class LoginResponse
    {
        // Time when the res was sent out
        [JsonProperty("serverDt")]
        public long ServerDate { get; set; }

        // Flag for if the news page should be opened immediately after logging in. Constant.
        [JsonProperty("noticePageListFlg")]
        public int NoticePageListFlag { get; set; }

        // Server assets version. Constant.
        [JsonProperty("mstVersionMaster")]
        public int MstVersionMaster { get; set; }

        // 0 here
        [JsonProperty("resultCode")]
        public int ResultCode { get; set; }

        //0 here
        [JsonProperty("resultType")]
        public int ResultType { get; set; }

        // 0 here
        [JsonProperty("nextScreenType")]
        public int NextScreenType { get; set; }

        // Idk, should be good as 0.
        [JsonProperty("monthlyPurchasableLeft")]
        public long MonthlyPurchasableLeft { get; set; }

        // Constant.
        [JsonProperty("ymoneyShopSaleList")]
        public List<object> YMoneyShopSaleList { get; set; }

        // Empty.
        [JsonProperty("ywpToken")]
        public string YwpToken { get; set; }

        // Should be empty.
        [JsonProperty("token")]
        public string Token { get; set; }

        // Empty.
        [JsonProperty("dialogMsg")]
        public string DialogMsg { get; set; }

        public LoginResponse()
        {
            ServerDate = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            NoticePageListFlag = int.Parse(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["noticePageListFlg"]);
            MstVersionMaster = int.Parse(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["mstVersionMaster"]);
            ResultCode = 0;
            ResultType = 0;
            NextScreenType = 0;
            MonthlyPurchasableLeft = 0;
            YMoneyShopSaleList = GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<object>>("ymoneyShopSaleList");
            YwpToken = "";
            Token = "";
            DialogMsg = "";
        }
        public async Task<Dictionary<string, object>> ToDictionary()
        {
            return new()
            {
                { "serverDt", ServerDate },
                { "noticePageListFlg", NoticePageListFlag },
                { "mstVersionMaster", MstVersionMaster },
                { "resultCode", ResultCode },
                { "resultType", ResultType },
                { "nextScreenType", NextScreenType },
                { "monthlyPurchasableLeft", MonthlyPurchasableLeft },
                { "ymoneyShopSaleList", YMoneyShopSaleList },
                { "ywpToken", YwpToken },
                { "token", Token },
                { "dialogMsg", DialogMsg }
            };
        }

    }
}

