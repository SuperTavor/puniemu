using Newtonsoft.Json;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.Server.GameServer.DataClasses;
namespace Puniemu.Src.Server.GameServer.Requests.Login.DataClasses
{
    public class LoginResponse : PuniemuResponseBase
    {

        // Flag for if the news page should be opened immediately after logging in. Constant.
        [JsonProperty("noticePageListFlg")]
        public int NoticePageListFlag { get; set; }


        // Idk, should be good as 0.
        [JsonProperty("monthlyPurchasableLeft")]
        public long MonthlyPurchasableLeft { get; set; }


        public LoginResponse()
        {
            NoticePageListFlag = int.Parse(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["noticePageListFlg"]);
            ResultCode = 0;
            ResultType = 0;
            NextScreenType = 0;
            MonthlyPurchasableLeft = 0;
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

