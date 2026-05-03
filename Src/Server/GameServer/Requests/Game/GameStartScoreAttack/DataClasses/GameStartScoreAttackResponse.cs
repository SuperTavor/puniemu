using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.StartScoreAttack.DataClasses
{
    public class StartScoreAttackResponse : CommonResponse
    {
        [JsonProperty("serverDt")]
        public long ServerDt { get; set; }

        [JsonProperty("itemValue")]
        public string ItemValue { get; set; } = "0|0|0|0";

        [JsonProperty("userYoukaiList")]
        public List<Dictionary<string, object>> UserYoukaiList { get; set; } = new();

        [JsonProperty("responseCodeTeamEvent")]
        public int ResponseCodeTeamEvent { get; set; } = 0;

        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData;

        [JsonProperty("scoreLogSendFlg")]
        public int ScoreLogSendFlg { get; set; } = 1;

        [JsonProperty("responseCode")]
        public int ResponseCode { get; set; } = 0;

        [JsonProperty("requestId")]
        public string RequestId { get; set; } = "0";

        [JsonProperty("addHPByWatchEffect")]
        public int AddHpByWatchEffect { get; set; } = 0;

        [JsonProperty("addHPByGokuEffect")]
        public int AddHPByGokuEffect { get; set; } = 0;

        [JsonProperty("freePlayFlg")]
        public int FreePlayFlg { get; set; } = 0;

        [JsonProperty("ymoneyShopSaleList")]
        public List<int> YmoneyShopSaleList { get; set; } = new() { 5 };

        [JsonProperty("enemyYoukaiOrderList")]
        public List<Dictionary<string, object>> EnemyYoukaiOrderList { get; set; } = new();

        [JsonProperty("continueInfoList")]
        public List<Dictionary<string, object>> ContinueInfoList { get; set; } = new();

        [JsonProperty("addAtkByGokuEffect")]
        public int AddAtkByGokuEffect { get; set; } = 0;

        [JsonProperty("scoreAttackId")]
        public int ScoreAttackId { get; set; } = 1068;

        public StartScoreAttackResponse(YwpUserData userData)
        {
            UserData = userData;
            ServerDt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}