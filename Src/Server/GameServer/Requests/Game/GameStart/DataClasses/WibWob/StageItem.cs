using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.Game.GameStart.DataClasses.WibWob
{
    public class StageItem
    {
        //C bool
        [JsonProperty("backGroundAnimeFlg")]
        public int BackgroundAnimeFlag { get; set; }

        [JsonProperty("backGroundAnimeName")]
        public string BackgroundAnimeName { get; set; }

        [JsonProperty("backGroundName")]
        //path to background or soemthing
        public string BackgroundName { get; set; }

        [JsonProperty("bgmFileName")]
        //Path to bgm, but its empty in all of the wibwob stages for some reason
        public string BgmFileName { get; set; }

        //C bool. probably decides IsBoss
        [JsonProperty("bossFlg")]
        public int BossFlag { get; set; }

        [JsonProperty("clearSec")]
        public int ClearSec { get; set; }

        [JsonProperty("continueDisableFlg")]
        public int ContinueDisableFlag { get; set; }

        [JsonProperty("deckForbidFlags")]
        public string DeckForbidFlags { get; set; }

        [JsonProperty("enemySetId")]
        //ID of the Yo-kais in the level in enemy set
        public int EnemySetID { get; set; }

        [JsonProperty("firstStageFlg")]
        public int FirstStageFlag { get; set; }

        [JsonProperty("hideStageRelationCode")]
        public string HideStageRelationCode { get; set; }

        [JsonProperty("mapId")]
        public int MapID { get; set; }

        [JsonProperty("nextStageRelationCode")]
        public string NextStageRelationCode { get; set; }

        [JsonProperty("recommendLevel")]
        public int RecommendedLevel { get; set; }

        [JsonProperty("stageId")]
        public int StageID { get; set; }

        [JsonProperty("stageName")]
        public string StageName { get; set; }

        [JsonProperty("stageNo")]
        public int StageNo { get; set; }

        [JsonProperty("stageObjectType")]
        public int StageObjectType { get; set; }

        [JsonProperty("stageType")]
        public int StageType { get; set;  }

        [JsonProperty("starGetConditionId1")]
        public int StarCond1 { get; set; }

        [JsonProperty("starGetConditionId2")]
        public int StarCond2 { get; set; }

        [JsonProperty("starGetConditionId3")]
        public int StarCond3 { get; set; }

        [JsonProperty("storeReviewFlg")]
        public int StoreReviewFlag { get; set; }

        [JsonProperty("useActionId")]
        public int UseActionID { get; set; }

        [JsonProperty("useActionPoint")]
        public int UseActionPoint { get; set; }

        [JsonProperty("useActionType")]
        public int UseActionType { get; set; }
    }
}
