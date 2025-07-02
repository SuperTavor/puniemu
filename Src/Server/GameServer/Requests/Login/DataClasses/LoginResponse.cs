using Newtonsoft.Json;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.Server.GameServer.DataClasses;
namespace Puniemu.Src.Server.GameServer.Requests.Login.DataClasses
{
    public class LoginResponse : PuniResponse
    {

        // Flag for if the news page should be opened immediately after logging in. Constant.
        [JsonProperty("noticePageListFlg")]
        public int NoticePageListFlag = int.Parse(DataManager.Logic.DataManager.GameDataManager!.GamedataCache["noticePageListFlg"]);

        // Idk, should be good as 0.
        [JsonProperty("monthlyPurchasableLeft")]
        public long MonthlyPurchasableLeft { get; set; }

        [JsonProperty("teamEventButtonHiddenFlg")]
        public int TeamEventButtonHiddenFlag = int.Parse(DataManager.Logic.DataManager.GameDataManager.GamedataCache["teamEventButtonHiddenFlg"]);

        [JsonProperty("noticePageList")]
        public List<object> NoticePageList = GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<object>>("noticePageList");

        [JsonProperty("mstMapMobPeriodNoList")]
        public List<object> MstMapMobPeriodNoList = GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<object>>("mstMapMobPeriodNoList");

        [JsonProperty("responseCodeTeamEvent")] 
        public int ResponseCodeTeamEvent = int.Parse(DataManager.Logic.DataManager.GameDataManager.GamedataCache["responseCodeTeamEvent"]);


        public LoginResponse()
        {
            ResultCode = 0;
            ResultType = 0;
            NextScreenType = 0;
            MonthlyPurchasableLeft = 0;
        }
        public async Task<Dictionary<string, object>> ToDictionary(string gdkey)
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
                { "dialogMsg", DialogMsg },
                { "storeUrl", StoreUrl },
                {"teamEventButtonHiddenFlg",TeamEventButtonHiddenFlag },
                {"shopSaleList",this.ShopSaleList! },
                {"noticePageList",NoticePageList},
                {"mstMapMobPeriodNoList", MstMapMobPeriodNoList},
                {"hitodamaShopSaleList",this.HitodamaShopSaleList! },
                {"webServerIp",this.WebServerIp },
                {"dialogTitle",this.DialogTitle },
                {"responseCodeTeamEvent",ResponseCodeTeamEvent},
                //Had to do it directly as a value and not as a property first because it's async
                {"openingTutorialFlg", await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<int>(gdkey, "opening_tutorial_flg")},
                {"requireAgeConfirm",true }

            };
        }

    }
}

