using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
namespace Puniemu.Src.Server.GameServer.Requests.GameEnd.DataClasses
{
    //This is not the entire request structure, only the fields we want.
    public class GameEndRequest
    {
        // Request id
        [JsonProperty("requestId")]
        public string? RequestID { get; set; }
        //Device ID
        [JsonProperty("deviceId")]
        public string Udkey { get; set; }
        //Account ID
        [JsonProperty("level5UserId")]
        public string Gdkey { get; set; }
        // type of the battle
        [JsonProperty("battleType")]
        public int BattleType { get; set; }
        // no idea
        [JsonProperty("bonusBlockNum")]
        public int BonusBlockNum { get; set; }
        // maybe cheat flg to more check
        [JsonProperty("cheatFlg")]
        public int CheatFlg { get; set; }
        // maybe time clear for the level if too big for int
        [JsonProperty("clearTimeLongSec")]
        public long ClearTimeLongSec { get; set; }
        // time clear time for the level
        [JsonProperty("clearTimeSec")]
        public int ClearTimeSec { get; set; }
        // max value combo
        [JsonProperty("comboMax")]
        public int ComboMax { get; set; }
        // all damage of the battle
        [JsonProperty("damageTotal")]
        public int DamageTotal { get; set; }
        // enemy result list
        [JsonProperty("enemyYoukaiResultList")]
        public List<EnemyYoukaiResultList> EnemyYoukaiResultList { get; set; }
        // maybe number total of punis clicked
        [JsonProperty("eraseNumTotal")]
        public int EraseNumTotal { get; set; }
        // average size of puni clicked
        [JsonProperty("eraseSizeAve")]
        public double EreaseSizeAverage { get; set; }
        // maximum size of puni clicked
        [JsonProperty("eraseSizeMax")]
        public int EraseSizeMax { get; set; }
        //idk
        [JsonProperty("eventPoint")]
        public int EventPoint { get; set; }
        //idk
        [JsonProperty("eventTeamPoint")]
        public int EventTeamPoint { get; set; }
        //idk
        [JsonProperty("eventSubPoint")]
        public int EventSubPoint { get; set; }
        // maybe time in the fever
        [JsonProperty("feverTimeNum")]
        public int FeverTimeNum { get; set; }
        [JsonProperty("linkSizeMax")]
        //maybe maximum link size
        public int LinkSizeMax { get; set; }
        [JsonProperty("pauseAtkNum")]
        //idk
        public int PauseAttackNum { get; set; }
        // damage total recived
        [JsonProperty("recvDamageTotal")]
        public int RecivedDamageTotal { get; set; }
        //idk
        [JsonProperty("resultRecvAtkNum")]
        public int ResultRecivedAttackNum { get; set; }
        //idk
        [JsonProperty("resultYoukaiHP")]
        public int ResultYoukaiHP { get; set; }
        // score
        [JsonProperty("score")]
        public int Score { get; set; }
        //idk
        [JsonProperty("scoreLog")]
        public string ScoreLog { get; set; }
        //idk
        [JsonProperty("spMissionIntValue1")]
        public int spMissionIntValue1 { get; set; }
        //id of the level
        [JsonProperty("stageId")]
        public int StageId { get; set; }
        //idk
        [JsonProperty("themeResultList")]
        public List<object> ThemeResultList { get; set; }
        //userid
        [JsonProperty("userId")]
        public string UserId { get; set; }
        // user youkai result list
        [JsonProperty("userYoukaiResultList")]
        public List<UserYoukaiResultListReq> UserYoukaiResultList { get; set; }
        // ywp_mst
        [JsonProperty("ywp_mst_game_const")]
        public List<Dictionary<string,object>> YwpMstGameConst { get; set; }


    }
}
