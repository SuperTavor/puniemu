using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.UserDataManager.Logic;

namespace Puniemu.Src.Server.GameServer.Requests.Init.InitGacha.DataClasses.WibWob
{
    /*
     * from:
        ReResponseCall<GenScatterHierarchy<TypeList<ResponseInitGacha, TypeList<ResponseUserGacha,
           TypeList<ResponseGachaMaster, TypeList<ResponseAllCommon, TypeList<ResponseUserIconBudgeList,
           TypeList<ResponseMasterVersionMaster, NullType> > > > > >, Responser>,
           4>::call(GenScatterHierarchy<TypeList<ResponseInitGacha, TypeList<ResponseUserGacha,
           TypeList<ResponseGachaMaster, TypeList<ResponseAllCommon, TypeList<ResponseUserIconBudgeList,
           TypeList<ResponseMasterVersionMaster, NullType> > > > > >, Responser>&, PicoJSONAnalyze&)
    */
    public class InitGachaResponse : CommonResponse
    {

        //probably 0 or 1
        [JsonProperty("oldDataFlg")]
        public int OldDataFlag = 0;

        //genuinely no idea wtf this is
        [JsonProperty("gachaAppVerAlert")]
        public string GachaAppVerAlert = "";

        //probably the resource name from bannerResourceList
        [JsonProperty("bannerResourceName")]
        public string BannerResourceName = "gg002";

        [JsonProperty("ywp_mst_gacha")]
        public string? YwpMstGacha;

        [JsonProperty("ywp_user_gacha")]
        public List<WibWobUserGachaEntry>? YwpUserGacha { get; set; }


        [JsonProperty("ywp_user_icon_budge")]
        public string? YwpUserIconBudge { get; set; }

        public static async Task<InitGachaResponse> ConstructAsync(string gdkey)
        {
            var res = new InitGachaResponse();
            res.YwpMstGacha = JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager!.GamedataCache["ywp_mst_gacha"])!["tableData"];
            res.YwpUserIconBudge = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(gdkey, "ywp_user_icon_budge");
            res.YwpUserGacha = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<WibWobUserGachaEntry>>(gdkey, "ywp_user_gacha");
            return res;
        }


    }
}
