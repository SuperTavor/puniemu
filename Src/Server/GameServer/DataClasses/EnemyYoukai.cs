using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class EnemyYoukai
    {
        //Seemingly ok as empty for now
        [JsonProperty("lotTreasureInfoList")]
        public List<object> LotTreasureInfoList = new();

        //idk. Seems to be related to item drops. Possibly like lot yokai for items?
        [JsonProperty("lotItemInfoList")]
        //also represented as a table btw
        public string LotItemInfoList = string.Empty;

        //Yokai hp
        [JsonProperty("hp")]
        public int Hp = 0;

        //No idea
        [JsonProperty("enemyId")]
        public long EnemyID = 0L;

        //Seems to represent yokai befriend chances.
        //table
        [JsonProperty("lotYoukaiInfoList")]
        public string LotYoukaiInfoList = string.Empty;

        //no idea
        [JsonProperty("actionTurn")]
        public int ActionTurn = 0;

        //no idea
        [JsonProperty("dropItemType")]
        public int DropItemType = 0;

        //No idea. I assume it dictates the amount of items to be dropped from the yokai
        [JsonProperty("dropItemCnt")]
        public int DropItemCount = 0;

        //no idea but i assume it's the id of the item to drop

        [JsonProperty("dropItemId")]
        public int DropItemID = 0;

        //no idea, but maybe foods that cannot be used on the yokai
        [JsonProperty("invalidFoodFlg")]
        public int InvalidFoodFlg = 0;

        //Probably dictates if lotTreasureInfoList is used
        [JsonProperty("lotTreasureFlg")]
        public int LotTreasureFlag = 0;

        //no idea
        [JsonProperty("enableFoodInfoList")]
        public List<EnableFoodInfo> EnableFoodInfoList = new();

        //enemy yokai attack power
        [JsonProperty("atkPower")]
        public int AttackPower = 0;

        //no idea
        [JsonProperty("replaceYoukaiId")]
        public int ReplaceYokaiID = 0;

    }
}
