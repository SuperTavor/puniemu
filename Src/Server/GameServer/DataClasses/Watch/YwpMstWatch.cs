using Newtonsoft.Json;
namespace Puniemu.Src.Server.GameServer.DataClasses.Watch
{
    public class YwpMstWatch
    {
        [JsonProperty("conditionId")]
        public int ConditionId { get; set; }

        [JsonProperty("conditionType")]
        public int ConditionType { get; set; }

        [JsonProperty("firstGradFlg")]
        public int? FirstGradFlg { get; set; }

        [JsonProperty("firstGradeFlg")]
        public int FirstGradeFlg { get; set; }

        [JsonProperty("iconFileName")]
        public string IconFileName { get; set; } = string.Empty;

        [JsonProperty("manufactMoney")]
        public int ManufactMoney { get; set; }

        [JsonProperty("mapEffectFileName")]
        public string MapEffectFileName { get; set; } = string.Empty;

        [JsonProperty("miniIconFileName")]
        public string MiniIconFileName { get; set; } = string.Empty;

        [JsonProperty("nextWatchId")]
        public int NextWatchId { get; set; }

        [JsonProperty("puzzleFlameBGName")]
        public string PuzzleFlameBGName { get; set; } = string.Empty;

        [JsonProperty("puzzleFlameBottomName")]
        public string PuzzleFlameBottomName { get; set; } = string.Empty;

        [JsonProperty("puzzleFlameSkillGaugeName1")]
        public string PuzzleFlameSkillGaugeName1 { get; set; } = string.Empty;

        [JsonProperty("puzzleFlameSkillGaugeName2")]
        public string PuzzleFlameSkillGaugeName2 { get; set; } = string.Empty;

        [JsonProperty("puzzleFlameTopName")]
        public string PuzzleFlameTopName { get; set; } = string.Empty;

        [JsonProperty("puzzleGeneralName1")]
        public string PuzzleGeneralName1 { get; set; } = string.Empty;

        [JsonProperty("puzzleGeneralName2")]
        public string PuzzleGeneralName2 { get; set; } = string.Empty;

        [JsonProperty("puzzleGeneralName3")]
        public string PuzzleGeneralName3 { get; set; } = string.Empty;

        [JsonProperty("watchDescription")]
        public string WatchDescription { get; set; } = string.Empty;

        [JsonProperty("watchGrade")]
        public int WatchGrade { get; set; }

        [JsonProperty("watchId")]
        public int WatchId { get; set; }

        [JsonProperty("watchName")]
        public string WatchName { get; set; } = string.Empty;

        [JsonProperty("watchType")]
        public int WatchType { get; set; }
    }
}
