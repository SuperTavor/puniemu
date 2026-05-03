using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;

namespace Puniemu.Src.Server.GameServer.Requests.GameEndScoreAttack.DataClasses
{
    public class GameEndScoreAttackResponse : CommonResponse
    {
        [JsonProperty("serverDt")]
        public long ServerDt { get; set; }

        [JsonProperty("responseCode")]
        public int ResponseCode { get; set; } = 0;

        [JsonProperty("userGameResultData")]
        public UserGameScoreAttackResultData UserGameResultData { get; set; } = new();

        [JsonProperty("userYoukaiResultList")]
        public List<UserYoukaiScoreAttackResultRes> UserYoukaiResultList { get; set; } = new();

        [JsonProperty("ymoneyShopSaleList")]
        public List<int> YmoneyShopSaleList { get; set; } = new() { 5 };

        [JsonProperty("teamEventButtonHiddenFlg")]
        public int TeamEventButtonHiddenFlg { get; set; } = 1;

        [JsonProperty("responseCodeTeamEvent")]
        public int ResponseCodeTeamEvent { get; set; } = 0;

        public GameEndScoreAttackResponse()
        {
            ServerDt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }

    public class UserGameScoreAttackResultData
    {
        [JsonProperty("weekHighScore")]
        public long WeekHighScore { get; set; } = 0;

        [JsonProperty("totalHighScore")]
        public long TotalHighScore { get; set; } = 0;

        [JsonProperty("rewardYoukaiId")]
        public long RewardYoukaiId { get; set; } = 0;

        [JsonProperty("itemScoreBonus")]
        public int ItemScoreBonus { get; set; } = 0;

        [JsonProperty("hpScoreBonus")]
        public int HpScoreBonus { get; set; } = 0;

        [JsonProperty("itemScoreBonusPctg")]
        public int ItemScoreBonusPctg { get; set; } = 0;

        [JsonProperty("scoreUpdateFlg")]
        public int ScoreUpdateFlg { get; set; } = 0;

        [JsonProperty("youkaiScoreBonus")]
        public int YoukaiScoreBonus { get; set; } = 0;

        [JsonProperty("score")]
        public long Score { get; set; } = 0;

        [JsonProperty("money")]
        public long Money { get; set; } = 0;

        [JsonProperty("leagueId")]
        public int LeagueId { get; set; } = 5;

        [JsonProperty("prevRank")]
        public int PrevRank { get; set; } = 0;

        [JsonProperty("youkaiScoreBonusPctg")]
        public int YoukaiScoreBonusPctg { get; set; } = 0;

        [JsonProperty("hpScoreBonusPctg")]
        public int HpScoreBonusPctg { get; set; } = 0;

        [JsonProperty("rank")]
        public int Rank { get; set; } = 6;

        [JsonProperty("exp")]
        public long Exp { get; set; } = 0;

        [JsonProperty("groupNo")]
        public int GroupNo { get; set; } = 403;
    }

    public class UserYoukaiScoreAttackResultRes
    {
        [JsonProperty("haveFlg")]
        public bool HaveFlg { get; set; } = false;

        [JsonProperty("isMaxLevel")]
        public bool IsMaxLevel { get; set; } = false;

        [JsonProperty("isLockLevel")]
        public bool IsLockLevel { get; set; } = false;

        [JsonProperty("before")]
        public UserYoukaiScoreAttackResultExp Before { get; set; } = new();

        [JsonProperty("youkaiId")]
        public long YoukaiId { get; set; } = 0;

        [JsonProperty("canEvolve")]
        public bool CanEvolve { get; set; } = false;

        [JsonProperty("after")]
        public UserYoukaiScoreAttackResultExp After { get; set; } = new();
    }

    public class UserYoukaiScoreAttackResultExp
    {
        [JsonProperty("level")]
        public int Level { get; set; } = 0;

        [JsonProperty("exp")]
        public int Exp { get; set; } = 0;

        [JsonProperty("expBar")]
        public UserYoukaiScoreAttackResultExpBar ExpBar { get; set; } = new();
    }

    public class UserYoukaiScoreAttackResultExpBar
    {
        [JsonProperty("pctg")]
        public int Percentage { get; set; } = 0;

        [JsonProperty("denominator")]
        public int Denominator { get; set; } = 0;

        [JsonProperty("numerator")]
        public int Numerator { get; set; } = 0;
    }
}