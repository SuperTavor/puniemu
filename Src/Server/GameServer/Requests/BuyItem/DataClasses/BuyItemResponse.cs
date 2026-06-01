using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.UserDataManager.Logic;

namespace Puniemu.Src.Server.GameServer.Requests.BuyItem.DataClasses
{
    public class BuyItemResponse : CommonResponse
    {
        [JsonProperty("ywp_user_icon_budge")]
        public string YwpUserIconBudge { get; set; }

        //you would *think* this is the item id that is being bought but its literally always 0 here
        [JsonProperty("itemId")]
        public int ItemId { get; set; } = 0;


        //Strangely also always zero
        [JsonProperty("cnt")]
        public int Count { get; set; } = 0;


        //Player inventory
        [JsonProperty("ywp_user_item")]
        public string YwpUserItem { get; set; }

        //How much stock remains for item
        [JsonProperty("ywp_user_shop_item_remain_cnt")]
        public string YwpUserShopItemRemainCount { get; set; }

        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }

        public async static Task<BuyItemResponse> BuildAsync(string gdkey)
        {
            var res = new BuyItemResponse();
            res.YwpUserIconBudge = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(gdkey, "ywp_user_icon_budge");
            res.YwpUserShopItemRemainCount = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(gdkey, "ywp_user_shop_item_remain_cnt");
            res.UserData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(gdkey, "ywp_user_data");
            return res;
        }

    }
}
