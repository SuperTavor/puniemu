using Newtonsoft.Json;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.Server.GameServer.DataClasses;
namespace Puniemu.Src.Server.GameServer.Requests.GameStart.DataClasses
{
    public class GameStartResponse : PuniemuResponseBase
    {
        //Information about the user's youkai.
        [JsonProperty("userYoukaiList")]
        public List<UserYoukaiItem> UserYoukaiList = new();
        //is the staged alr cleared? True if it's the first time (1)
        [JsonProperty("firstClearItemFlg")]
        public int IsFirstClear { get; set; }

        //C bool. idk what it does
        [JsonProperty("youkaiHp")]
        public int YoukaiHp { get; set; }

        [JsonProperty("responseCodeTeamEvent")]
        public int ResponseCodeTeamEvent = int.Parse(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["responseCodeTeamEvent"]);

        [JsonProperty("ywp_use_data")]
        public YwpUserData UserData = new();

        //idk
        [JsonProperty("themeList")]
        public List<object> ThemeList = new();

        //idk
        [JsonProperty("scoreLogSendFlg")]
        public int ScoreLogSendFlag = 0;
        //idk
        [JsonProperty("themeScoreCoef")]
        public string ThemeScoreCoef = "";
        //idk
        [JsonProperty("chanceAddRateEventBlock")]
        public int ChanceAddRateEventBlock = 0;
        //idk
        [JsonProperty("addHPByWatchEffect")]
        public int AddHpByWatchEffect = 0;
        //IDK
        [JsonProperty("eventPointUpItemId")]
        public int EventPointUpItemId = 0;

        //Defines which graphic should be shown in the pause screen (e.g. event battle or smth)
        [JsonProperty("stageType")]
        public long StageType { get; set; }

        //info about the enemies
        [JsonProperty("enemyYoukaiList")]
        public List<EnemyYoukai> EnemyYoukaiList = new();

        //no idea
        [JsonProperty("battleType")]
        public long BattleType = 1;

        //no idea
        [JsonProperty("eventPointMaterial")]
        public string EventPointMaterial = "";

        //no idea
        [JsonProperty("addHPByGokuEffect")]
        public int AddHPByGokuEffect = 0;

        //no idea. maybe dictates if this is an event battle
        [JsonProperty("eventFlg")]
        public int EventFlag = 0;

        //no idea
        [JsonProperty("addAtkByGokuEffect")]
        public int AddAttackByGokuEffect = 0;

        //no idea
        [JsonProperty("eventStatus")]
        public int EventStatus = 0;

        //actually no idea. works as 0 i think
        [JsonProperty("requestId")]
        public string RequestID = "0";
        //no idea
        [JsonProperty("itemDropMaxCnt")]
        public int ItemDropMaxCount = 2;
        public GameStartResponse()
        {
            ResultCode = 0;
            ResultType = 0;
            NextScreenType = 0;
        }
        

    }
}

