﻿using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.BuyHitodama.DataClasses;
using System;
using System.Runtime.Intrinsics.Arm;
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

        public YwpUserData(PlayerIcon icon, PlayerTitle title, string gdkey, string playerName)
        {
            this.PlayerName = playerName;
            this.Birthday = "";
            this.IconID = (int)icon;
            this.CharacterTitleID = (int)title;
            this.FreeHitodama = 5;
            this.FriendMaxCount = 0;
            this.MedalPoint = 0;
            this.CurrentStageID = 1001001;
            this.GokuCollectCount = 0;
            this.YMoney = 3000;
            this.ReviewFlag = 0;
            this.MoveReason = 0;
            this.ChargeYmoney = 0;
            this.CrystalCollectCount = 0;
            this.EventPointUpItemID = 0;
            this.CharacterID = GenerateFriendCode();
            this.LimitTimeSaleRemainSec = 0;
            this.TotMedalPoint = 0;
            this.EventPointUpItemRemainSec = 0;
            this.TodaysRemainSec = GetRemainingSecondsInDay();
            this.WeeklyFreeFlag = 0;
            this.Hitodama = 0;
            this.UserID = System.IO.Hashing.Crc32.HashToUInt32(System.Text.Encoding.UTF8.GetBytes(this.CharacterID)).ToString();
            this.UsingItemList = new List<object>();
            this.HitodamaRecoverSec = 0;
            this.LimitTimeSaleEndDt = "";
            this.EquipWatchID = 0;
            this.PlateID = 1;
            this.EffectID = 1;
        }
        private static string GenerateFriendCode()
        {
            char[] LetterBytes = "abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
            const int CodeLength = 8;
            var code = new char[CodeLength];

            byte[] buffer = new byte[CodeLength];

            RandomNumberGenerator.Fill(buffer);

            for (int i = 0; i < code.Length; i++)
            {
                code[i] = LetterBytes[buffer[i] % LetterBytes.Length];
            }

            return new string(code);
        }

        private int GetRemainingSecondsInDay()
        {
            DateTime now = DateTime.Now;
            int secondsPassed = now.Hour * 3600 + now.Minute * 60 + now.Second;
            const int SECONDS_IN_DAY = 86400;
            return SECONDS_IN_DAY - secondsPassed;
        }

        public void BuyHitodamaGood(Good good)
        {
            this.YMoney -= good.Cost;
            this.Hitodama += (good.RewardedHitodama + good.BonusSalesHitodama);
        }
    }

    
}
