using Newtonsoft.Json;
using Puniemu.Src.DataManager.Logic;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.UserDataManager.Logic;
using Puniemu.Src.Utils.GeneralUtils;

public class CommonLoginResponse : CommonResponse
{
    [JsonProperty("noticePageListFlg")]
    public int NoticePageListFlag { get; set; }

    [JsonProperty("monthlyPurchasableLeft")]
    public long MonthlyPurchasableLeft { get; set; }

    [JsonProperty("teamEventButtonHiddenFlg")]
    public int TeamEventButtonHiddenFlag { get; set; }

    [JsonProperty("ywp_user_shop_item_remain_cnt")]
    public string YwpUserShopItemRemainCnt { get; set;
   }
    [JsonProperty("noticePageList")]
    public List<object> NoticePageList { get; set; }

    [JsonProperty("mstMapMobPeriodNoList")]
    public List<object> MstMapMobPeriodNoList { get; set; }

    [JsonProperty("responseCodeTeamEvent")]
    public int ResponseCodeTeamEvent { get; set; }

    [JsonProperty("requireAgeConfirm")]
    public int RequireAgeConfirm { get; set; } = 2;

    [JsonProperty("openingTutorialFlg")]
    public int OpeningTutorialFlag { get; set; }

    public CommonLoginResponse()
    {
        ResultCode = 0;
        ResultType = 0;
        NextScreenType = 0;
    }

    public virtual async Task ConstructAsync(string gdkey)
    {
        var gm = DataManager.GameDataManager!;

        NoticePageListFlag =
            int.Parse(gm.GamedataCache["noticePageListFlg"]);

        TeamEventButtonHiddenFlag =
            int.Parse(gm.GamedataCache["teamEventButtonHiddenFlg"]);

        NoticePageList =
            GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<object>>("noticePageList");

        MstMapMobPeriodNoList =
            GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<object>>("mstMapMobPeriodNoList");

        ResponseCodeTeamEvent =
            int.Parse(gm.GamedataCache["responseCodeTeamEvent"]);

        MonthlyPurchasableLeft = 0;

        OpeningTutorialFlag =
            await UserDataManager
                .GetYwpUserAsync<int>(gdkey, "opening_tutorial_flg");
    }
}