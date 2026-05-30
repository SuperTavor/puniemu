using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Reflection;
using System.Text;

namespace Puniemu.Src.TableParser.DataClasses
{
    public class WibwobMstStageItem
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
        public int StageType { get; set; }

        [JsonProperty("starGetConditionId1")]
        public long StarCond1 { get; set; }

        [JsonProperty("starGetConditionId2")]
        public long StarCond2 { get; set; }

        [JsonProperty("starGetConditionId3")]
        public long StarCond3 { get; set; }

        [JsonProperty("storeReviewFlg")]
        public int StoreReviewFlag { get; set; }

        [JsonProperty("useActionId")]
        public int UseActionID { get; set; }

        [JsonProperty("useActionPoint")]
        public int UseActionPoint { get; set; }

        [JsonProperty("useActionType")]
        public int UseActionType { get; set; }
    }
    public class PuniMstStageItem
    {
        public long StageId { get; set; }
        public long MapId { get; set; }
        public string? StageName { get; set; }
        public int StageType { get; set; } 
        public long StageNo { get; set; }
        public long FirstStageFlag { get; set; }
        public int BossFlag { get; set; }
        public int UseActionPoint { get; set; } //not sure
        public long StarCond1 { get; set; }
        public long StarCond2 { get; set; }
        public long StarCond3 { get; set; }
        public long Unk1 { get; set; }
        public long EnemySetID { get; set; }
        public long StoreReviewFlag { get; set; } //not sure
        public string? BackgroundName { get; set; }
        public string? NextStageRelationCode { get; set; }
        public string? HideStageRelationCode { get; set; }
        public long RecommendLevel { get; set; }
        public int UseActionType { get; set; }
        public int UseActionId { get; set; }
        public long StageObjectType { get; set; }
        public string? BgmFileName { get; set; }
        public long Unk2 { get; set; }
        public string? Unk3 { get; set; }
        public long Unk4 { get; set; }
        public string? DeckForbidFlags { get; set; }
        public string? Unk5 { get; set; }
        public string? Unk6 { get; set; }
        public long Unk7 { get; set; }
        public long Unk8 { get; set; }
        public long Unk9 { get; set; }
    }
}