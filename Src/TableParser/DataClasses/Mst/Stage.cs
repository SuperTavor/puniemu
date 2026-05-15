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
    public class PuniMstStageItem
    {
        public long StageId { get; set; }
        public long MapId { get; set; }
        public string? StageName { get; set; }
        public int StageType { get; set; } //Maybe not sure
        public long Unk1 { get; set; }
        public long Unk2 { get; set; }
        public long Unk3 { get; set; }
        public int BattleType { get; set; }
        public long Unk5 { get; set; }
        public long Unk6 { get; set; }
        public long Unk7 { get; set; }
        public long Unk8 { get; set; }
        public long Unk9 { get; set; }
        public long Unk10 { get; set; }
        public string? Unk11 { get; set; }
        public string? Unk12 { get; set; }
        public string? Unk13 { get; set; }
        public long Unk14 { get; set; }
        public long Unk15 { get; set; }
        public long Unk16 { get; set; }
        public long Unk17 { get; set; }
        public string? Unk18 { get; set; }
        public long Unk19 { get; set; }
        public string? Unk20 { get; set; }
        public long Unk21 { get; set; }
        public string? Unk22 { get; set; }
        public string? Unk23 { get; set; }
        public string? Unk24 { get; set; }
        public long Unk25 { get; set; }
        public long Unk26 { get; set; }
        public long Unk27 { get; set; }
    }
}