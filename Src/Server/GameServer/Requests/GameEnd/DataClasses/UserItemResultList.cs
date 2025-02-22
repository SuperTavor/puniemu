using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.GameEnd.DataClasses
{
    public class UserItemResultList
    {
        // item id
        [JsonProperty("itemId")]
        public long DamageMax = 0L;
        // type of the item
        [JsonProperty("itemType")]
        public int ItemType = 0;
        // number of item
        [JsonProperty("itemCnt")]
        public int ItemCnt = 0;
        // new item ?
        [JsonProperty("newFlg")]
        public int NewFlg = 0;
        // reward, first time stage cleared
        [JsonProperty("firstRewardFlg")]
        public int FirstRewardFlg = 0;
        // idk
        [JsonProperty("themeBonusFlg")]
        public int ThemeBonusFlg = 0;
    }
}
