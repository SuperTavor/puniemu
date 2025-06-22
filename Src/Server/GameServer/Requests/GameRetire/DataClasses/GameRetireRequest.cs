using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
namespace Puniemu.Src.Server.GameServer.Requests.GameRetire.DataClasses
{
    //This is not the entire request structure, only the fields we want.
    public class GameRetireRequest
    {
        // Request id
        [JsonProperty("requestId")]
        public string? RequestID { get; set; }
        //Device ID
        [JsonProperty("deviceId")]
        public string? Udkey { get; set; }
        //Account ID
        [JsonProperty("level5UserId")]
        public string? Gdkey { get; set; }
        // type of the battle
        [JsonProperty("battleType")]
        public int BattleType { get; set; }
        // no idea
        [JsonProperty("bonusBlockNum")]
        public int BonusBlockNum { get; set; }
        // max value combo
        [JsonProperty("comboMax")]
        public int ComboMax { get; set; }
        // all damage of the battle
        [JsonProperty("damageTotal")]
        public int DamageTotal { get; set; }
        // enemy result list
        [JsonProperty("enemyYoukaiResultList")]
        public List<EnemyYoukaiResultList>? EnemyYoukaiResultList { get; set; }
        // maybe number total of punis clicked
        [JsonProperty("eraseNumTotal")]
        public int EraseNumTotal { get; set; }
        // average size of puni clicked
        // maximum size of puni clicked
        [JsonProperty("eraseSizeMax")]
        public int EraseSizeMax { get; set; }
        //idk
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
        // score
        [JsonProperty("score")]
        public int Score { get; set; }
        //idk
        [JsonProperty("scoreLog")]
        public string? ScoreLog { get; set; }
        //id of the level
        [JsonProperty("stageId")]
        public int StageId { get; set; }
        //userid
        [JsonProperty("userId")]
        public string? UserId { get; set; }
        // user youkai result list
        [JsonProperty("userYoukaiResultList")]
        public List<UserYoukaiResultListReq>? UserYoukaiResultList { get; set; }
        // ywp_mst
        [JsonProperty("ywp_mst_game_const")]
        public List<Dictionary<string,object>>? YwpMstGameConst { get; set; }


    }
}
