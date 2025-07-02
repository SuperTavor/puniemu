using Newtonsoft.Json;

using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses.Gacha;
using Puniemu.Src.Server.GameServer.DataClasses.Gacha.GachaStamp;
using Puniemu.Src.TableParser.Logic;
using Puniemu.Src.Utils.GeneralUtils;

namespace Puniemu.Src.Server.GameServer.Requests.InitGacha.DataClasses
{
    public class InitGachaResponse : PuniResponse
    {
        //Info about the cranks
        //theorized variables:
        //slot 1 - Gacha ID
        //slot 2 - Start date 
        //slot 3 - expiration date
        //slot 5 - payment type* (0 for ymoney, item id for any other type of payment for the crank)
        //im not sure about the payment type location because i have inconclusive results from reading a lot of cranks but it seems like that to me
        //slot 6 - cost 
        //slot 15 - crank description??
        [JsonProperty("ywp_mst_gacha")]
        public string? YwpMstGacha { get; set; }

        [JsonProperty("ywp_mst_item")]
        public string? YwpMstItem { get; set; }

        //Some static info from the server about the crank stamps
        [JsonProperty("gachaStampList")]
        public List<GachaStamp> GachaStampList { get; set; }

        [JsonProperty("ywp_user_event")]
        public string? YwpUserEvent { get; set; }

        [JsonProperty("ywp_mst_youkai_bonus_effect_exclude")]
        public string? YwpMstYoukaiBonusEffectExclude { get; set; }

        [JsonProperty("gachaStampIdList")]
        public List<GachaStampIdentifier>? GachaStampIdList { get; set; }

        [JsonProperty("ywp_mst_event_group_assist_disp")]
        public string? YwpMstEvenGroupAssistDisp { get; set; }

        [JsonProperty("canPossessionItemList")]
        //idk what this is but its important to the gacha i think
        public string? CanPossessItemList { get; set; }

        [JsonProperty("ywp_user_gacha_stamp")]
        public string? YwpUserGachaStamp { get; set; }

        [JsonProperty("ywp_mst_event")]
        public string? YwpMstEvent { get; set; }

        [JsonProperty("ywp_user_icon_budge")]
        public string? YwpUserIconBudge { get; set; }

        [JsonProperty("ywp_user_gacha")]
        public string? YwpUserGacha { get; set; }

        [JsonProperty("ywp_mst_event_youkai_assist_disp")]
        public string? YwpMstEventYoukaiAssistDisp { get; set; }


        //prob supposed to be a list of a GachaLotRule type but the list is empty in the real game servers at the moment so we have no way to recreate it. 
        //int list for now
        [JsonProperty("gachaLotRuleList")]
        public List<int>? GachaLotRuleList { get; set; }

        [JsonProperty("ywp_mst_gacha_convert_item")]
        public string? YwpMstGachaConvertItem { get; set; }

        [JsonProperty("ywp_user_data")]
        public YwpUserData? YwpUserData { get; set; }

        //prob supposed to be a list of a type but the list is empty in the real game servers at the moment so we have no way to recreate it. 
        //int list for now
        [JsonProperty("ywp_mst_youkai_pos_effect_exclude")]
        public List<int>? YwpMstYoukaiPosEffectExclude { get; set; }

        //Not sure what this is but it has something to do with multis
        //Should ask someone more knowlegable about Puni
        [JsonProperty("canUseMultiGachaCoinIdList")]
        public List<int>? CanUseMultiGachaCoinIDList { get; set; }

        [JsonProperty("ywp_mst_coin_purchase_master")]
        public WrappedTable? YwpMstCoinMasterPurchase { get; set; }

        [JsonProperty("bannerResourceList")]
        public List<BannerResource>? BannerResourceList { get; set; }

        public static async Task BuildAsync()
        {
            var instance = new InitGachaResponse();

            instance.YwpMstGacha = (string)JsonConvert.DeserializeObject<Dictionary<string,object>>(DataManager.Logic.DataManager.GameDataManager!.GamedataCache["ywp_mst_gacha"])["tableData"];
            instance.YwpMstItem = (string)JsonConvert.DeserializeObject<Dictionary<string, object>>(DataManager.Logic.DataManager.GameDataManager!.GamedataCache["ywp_mst_item"])["tableData"];
            instance.GachaStampList = JsonConvert.DeserializeObject<List<GachaStamp>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["gachaStampList"]);
        }

    }
}
 