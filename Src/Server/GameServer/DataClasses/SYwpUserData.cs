namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public struct SYwpUserData
    {
        // User's birthday. Empty unless set.
        [JsonPropertyName("birthday")]
        public string Birthday { get; set; }

        // Free energy right now. Usually 5 at the start of the game.
        [JsonPropertyName("freeHitodama")]
        public int FreeHitodama { get; set; }

        // Have no idea. Initialized to 0.
        [JsonPropertyName("friendMaxCnt")]
        public int FriendMaxCount { get; set; }

        // Have no idea. Initialized to 0.
        [JsonPropertyName("medalPoint")]
        public int MedalPoint { get; set; }

        // ID of the current stage the user is at. Defaults to 1001001 (first level).
        [JsonPropertyName("nowStageId")]
        public int CurrentStageID { get; set; }

        // ID of the character's title (e.g., kun, chan).
        [JsonPropertyName("titleId")]
        public int CharacterTitleID { get; set; }

        // Have no idea. Initialized to 0.
        [JsonPropertyName("gokuCollectCnt")]
        public int GokuCollectCount { get; set; }

        // Amount of YMoney. Initialized at 3000.
        [JsonPropertyName("ymoney")]
        public int YMoney { get; set; }

        // Have no idea. Initialized to 0.
        [JsonPropertyName("reviewFlg")]
        public int ReviewFlag { get; set; }

        // Have no idea. Initialized to 0.
        [JsonPropertyName("moveReason")]
        public int MoveReason { get; set; }

        // Have no idea. Initialized to 0.
        [JsonPropertyName("chargeYmoney")]
        public int ChargeYmoney { get; set; }

        // Have no idea. Initialized to 0.
        [JsonPropertyName("crystalCollectCnt")]
        public int CrystalCollectCount { get; set; }

        // Have no idea. Initialized to 0.
        [JsonPropertyName("eventPointUpItemId")]
        public int EventPointUpItemID { get; set; }

        // Friend code of the current user. Example: "8wea1pxb".
        [JsonPropertyName("characterId")]
        public string CharacterID { get; set; }

        // ID of the user's icon. Initialized to 0.
        [JsonPropertyName("iconId")]
        public int IconID { get; set; }

        // Have no idea. Initialized to 0.
        [JsonPropertyName("limitTimeSaleRemainSec")]
        public int LimitTimeSaleRemainSec { get; set; }

        // Have no idea. Initialized to 0.
        [JsonPropertyName("totMedalPoint")]
        public int TotMedalPoint { get; set; }

        // Have no idea. Initialized to 0.
        [JsonPropertyName("eventPointUpItemRemainSec")]
        public int EventPointUpItemRemainSec { get; set; }

        // Player's name.
        [JsonPropertyName("playerName")]
        public string PlayerName { get; set; }

        // Remaining seconds for today. Initialized to 0.
        [JsonPropertyName("todaysRemainSec")]
        public int TodaysRemainSec { get; set; }

        // Weekly free flag. Initialized to 0.
        [JsonPropertyName("weeklyFreeFlg")]
        public int WeeklyFreeFlag { get; set; }

        // Energy count. Initialized to 0.
        [JsonPropertyName("hitodama")]
        public int Hitodama { get; set; }

        // User ID. 0 here, as this specific user ID is not yet used.
        [JsonPropertyName("userId")]
        public string UserID { get; set; }

        // List of items in use.
        [JsonPropertyName("usingItemList")]
        public List<int> UsingItemList { get; set; }

        // Seconds left until energy recovers.
        [JsonPropertyName("hitodamaRecoverSec")]
        public int HitodamaRecoverSec { get; set; }

        // End date of the limit time sale. Empty unless set.
        [JsonPropertyName("limitTimeSaleEndDt")]
        public string LimitTimeSaleEndDt { get; set; }

        // ID of the equipped watch. Initialized to 0.
        [JsonPropertyName("equipWatchId")]
        public int EquipWatchID { get; set; }
    }
}
