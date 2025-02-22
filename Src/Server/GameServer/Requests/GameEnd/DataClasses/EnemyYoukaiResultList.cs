using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.GameEnd.DataClasses
{
    public class EnemyYoukaiResultList
    {
        //Yokai dead order
        [JsonProperty("deadEndOrder")]
        public int DeadEndOrder = 0;
        //Yokai dead type
        [JsonProperty("deadEndType")]
        public int DeadEndType = 0;
        //no idea
        [JsonProperty("dropItemCheckKey")]
        public string DropItemCheckKey = string.Empty;
        // is item dropped
        [JsonProperty("dropItemFlg")]
        public int DropItemFlg = 0;
        // id of the dropped item
        [JsonProperty("dropItemId")]
        public long DropItemId = 0L;
        // is treasure dropped
        [JsonProperty("dropTreasureFlg")]
        public int DropTreasureFlg = 0;
        // no idea
        [JsonProperty("dropYoukaiCheckKey")]
        public string DropYoukaiCheckKey = string.Empty;
        // id of the dropped treasure
        [JsonProperty("dropTreasureId")]
        public long DropTreasureId = 0L;
        // is yokai dropped
        [JsonProperty("dropYoukaiFlg")]
        public int DropYoukaiFlg = 0;
        // id of the enemy (we need to search the youkaiID with this one)
        [JsonProperty("enemyId")]
        public long EnemyId = 0L;
        // id of used item (befriends)
        [JsonProperty("itemId")]
        public long ItemId = 0L;
        // Item level 4 (befriends)
        [JsonProperty("useItemLLarge")]
        public int UseItemLLarge = 0;
        // Item level 3 (befriends)
        [JsonProperty("useItemLarge")]
        public int UseItemLarge = 0;
        // Item level 2 (befriends)
        [JsonProperty("UseItemMiddle")]
        public int UseItemMiddle = 0;
        // Item level 1 (befriends)
        [JsonProperty("useItemSmall")]
        public int UseItemSmall = 0;

    }
}
