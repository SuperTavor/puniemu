using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.GameRetire.DataClasses
{
    public class UserGameResultData
    {
        // id of the befriend youkai
        [JsonProperty("rewardYoukaiId")]
        public long RewardYoukaiId = 0L;
        // is best score changed ?
        [JsonProperty("scoreUpdateFlg")]
        public int ScoreUpdateFlg = 0;
        [JsonProperty("score")]
        public int Score = 0;
        [JsonProperty("isMaxItemFlg")]
        public int IsMaxItemFlg = 0;
        [JsonProperty("money")]
        public int Money = 0;
        [JsonProperty("prevRank")]
        public int PrevRank = 0;
        [JsonProperty("rank")]
        public int Rank = 0;
        //star/challente flg
        [JsonProperty("starGetFlg3")]
        public int StarGetFlg3 = 0;
        [JsonProperty("starGetFlg2")]
        public int StarGetFlg2 = 0;
        [JsonProperty("starGetFlg1")]
        public int StarGetFlg1 = 0;
        [JsonProperty("prevScore")]
        public int PrevScore = 0;
        [JsonProperty("exp")]
        public int Exp = 0;
        [JsonProperty("stageId")]
        public long StageId = 0L;
    }
}
