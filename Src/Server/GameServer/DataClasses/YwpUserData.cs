using Newtonsoft.Json;
using ManageData = Puniemu.Src.UserDataManager.Logic.UserDataManager;
using Puniemu.Src.Server.GameServer.Requests.BuyHitodama.DataClasses;
using System;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class YwpUserData
    {
        // User's birthday. Empty unless set.
        [JsonProperty("birthday")]
        public string Birthday { get; set; }

        // Free energy right now. Usually 5 at the start of the game.
        [JsonProperty("freeHitodama")]
        public int FreeHitodama { get; set; }

        // Have no idea. Initialized to 0.
        [JsonProperty("friendMaxCnt")]
        public int FriendMaxCount { get; set; }

        // Not in ywp_user_data but we put it to easily get it
        [JsonProperty("youkaiId")]
        public long YoukaiId { get; set; }

        // Have no idea. Initialized to 0.
        [JsonProperty("medalPoint")]
        public int MedalPoint { get; set; }

        // ID of the current stage the user is at. Defaults to 1001001 (first level).
        [JsonProperty("nowStageId")]
        public int CurrentStageID { get; set; }

        // ID of the character's title (e.g., kun, chan).
        [JsonProperty("titleId")]
        public int CharacterTitleID { get; set; }

        // Have no idea. Initialized to 0.
        [JsonProperty("gokuCollectCnt")]
        public int GokuCollectCount { get; set; }

        // Amount of YMoney. Initialized at 3000.
        [JsonProperty("ymoney")]
        public int YMoney { get; set; }

        // Have no idea. Initialized to 0.
        [JsonProperty("reviewFlg")]
        public int ReviewFlag { get; set; }

        // Have no idea. Initialized to 0.
        [JsonProperty("moveReason")]
        public int MoveReason { get; set; }

        // Have no idea. Initialized to 0.
        [JsonProperty("chargeYmoney")]
        public int ChargeYmoney { get; set; }

        // Have no idea. Initialized to 0.
        [JsonProperty("crystalCollectCnt")]
        public int CrystalCollectCount { get; set; }

        // Have no idea. Initialized to 0.
        [JsonProperty("eventPointUpItemId")]
        public int EventPointUpItemID { get; set; }

        // Friend code of the current user. Example: "8wea1pxb".
        [JsonProperty("characterId")]
        public string CharacterID { get; set; }

        // ID of the user's icon. Initialized to 0.
        [JsonProperty("iconId")]
        public int IconID { get; set; }

        // ID of the user's plate. Initialized to 1.
        [JsonProperty("plateId")]
        public int PlateID { get; set; }

        // ID of the user's codeName. Initialized to 1.
        [JsonProperty("codenameId")]
        public int CodeNameID { get; set; }

        // ID of the user's effect. Initialized to 1.
        [JsonProperty("effectId")]
        public int EffectID { get; set; }

        // Have no idea. Initialized to 0.
        [JsonProperty("limitTimeSaleRemainSec")]
        public int LimitTimeSaleRemainSec { get; set; }

        // Have no idea. Initialized to 0.
        [JsonProperty("totMedalPoint")]
        public int TotMedalPoint { get; set; }

        // Have no idea. Initialized to 0.
        [JsonProperty("eventPointUpItemRemainSec")]
        public int EventPointUpItemRemainSec { get; set; }

        // Player's name.
        [JsonProperty("playerName")]
        public string PlayerName { get; set; }

        // Remaining seconds for today. Initialized to 0.
        [JsonProperty("todaysRemainSec")]
        public int TodaysRemainSec { get; set; }

        // Weekly free flag. Initialized to 0.
        [JsonProperty("weeklyFreeFlg")]
        public int WeeklyFreeFlag { get; set; }

        // Energy count. Initialized to 0.
        [JsonProperty("hitodama")]
        public int Hitodama { get; set; }

        // User ID. 0 here, as this specific user ID is not yet used.
        [JsonProperty("userId")]
        public string UserID { get; set; }

        // List of items in use.
        [JsonProperty("usingItemList")]
        public List<object> UsingItemList { get; set; }

        // Seconds left until energy recovers.
        [JsonProperty("hitodamaRecoverSec")]
        public int HitodamaRecoverSec { get; set; }

        // End date of the limit time sale. Empty unless set.
        [JsonProperty("limitTimeSaleEndDt")]
        public string LimitTimeSaleEndDt { get; set; }

        // ID of the equipped watch. Initialized to 0.
        [JsonProperty("equipWatchId")]
        public int EquipWatchID { get; set; }

        public async Task HitodamaRecover(string gdkey)
        {
            DateTimeOffset result;
            DateTimeOffset now = DateTimeOffset.UtcNow;
            const string TIME_FORMAT = "yyyy-MM-dd HH:mm:ss zzz";
            const string HITODAMA_RECOVER_TABLE = "last_hitodama_recover";
            try
            {
                string? time = await ManageData.GetYwpUserAsync<string>(gdkey, HITODAMA_RECOVER_TABLE);

                if (time == null || !DateTimeOffset.TryParse(time, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out result))
                {
                    result = now;
                    await ManageData.SetYwpUserAsync(gdkey, HITODAMA_RECOVER_TABLE, result.ToString(TIME_FORMAT));
                }
            }
            catch
            {
                result = now;
                await ManageData.SetYwpUserAsync(gdkey, HITODAMA_RECOVER_TABLE, result.ToString(TIME_FORMAT));
            }
            const int HITODAMA_RECOVER_SEC = 900;
            const int FREE_HITODAMA_MAX = 5;
            long seconds = result.ToUnixTimeSeconds();
            long nowSeconds = now.ToUnixTimeSeconds();
            long diff = nowSeconds - seconds;
            int current = this.Hitodama + this.FreeHitodama;
            int recovered = (int)(diff / HITODAMA_RECOVER_SEC);
            int maxCanRecover = FREE_HITODAMA_MAX - current;
            int applied = Math.Max(0, Math.Min(recovered, maxCanRecover));
            this.FreeHitodama += applied;
            if (current + applied >= FREE_HITODAMA_MAX)
            {
                await ManageData.SetYwpUserAsync(gdkey, HITODAMA_RECOVER_TABLE, now.ToString(TIME_FORMAT));
                this.HitodamaRecoverSec = 0;
            }
            else
            {
                this.HitodamaRecoverSec = HITODAMA_RECOVER_SEC - ((int)diff % HITODAMA_RECOVER_SEC);
            }
            if (recovered != 0)
            {
                if (this.FreeHitodama + this.Hitodama < FREE_HITODAMA_MAX)
                {
                    now = now.AddSeconds(-((int)diff % HITODAMA_RECOVER_SEC));
                }
                await ManageData.SetYwpUserAsync(gdkey, HITODAMA_RECOVER_TABLE, now.ToString(TIME_FORMAT));
            }
            await ManageData.SetYwpUserAsync(gdkey, "ywp_user_data", this);
        }
        public YwpUserData(PlayerIcon icon, PlayerTitle title, string playerName)
        {
            this.YoukaiId = 2235000;
            this.PlayerName = playerName;
            this.Birthday = "";
            this.IconID = (int)icon;
            this.CharacterTitleID = (int)title;
            this.FreeHitodama = 5;
            this.FriendMaxCount = 10;
            this.MedalPoint = 0;
            this.CurrentStageID = 1001001;
            this.GokuCollectCount = 0;
            this.YMoney = 3000;
            this.ReviewFlag = 0;
            this.MoveReason = 0;
            this.ChargeYmoney = 0;
            this.CrystalCollectCount = 0;
            this.EventPointUpItemID = 0;
            this.CharacterID = "";
            this.LimitTimeSaleRemainSec = 0;
            this.TotMedalPoint = 0;
            this.Hitodama = 30;
            this.EventPointUpItemRemainSec = 0;
            this.TodaysRemainSec = GetRemainingSecondsInDay();
            this.WeeklyFreeFlag = 0;
            this.Hitodama = 0;
            this.UsingItemList = new List<object>();
            this.HitodamaRecoverSec = 0;
            this.LimitTimeSaleEndDt = "";
            this.EquipWatchID = 10101;
            this.PlateID = 1;
            this.EffectID = 1;
        }

        private int GetRemainingSecondsInDay()
        {
            DateTime now = DateTime.Now;
            int secondsPassed = now.Hour * 3600 + now.Minute * 60 + now.Second;
            const int SECONDS_IN_DAY = 86400;
            return SECONDS_IN_DAY - secondsPassed;
        }

        public void BuyHitodamaGood(YwpMstShopHitodama good)
        {
            this.YMoney -= good.Price;
            this.Hitodama += (good.SellCount + good.bonuy);
        }
    }

    
}
