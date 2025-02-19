using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.GameEnd.DataClasses
{
    public class LockedStageResultList
    {
        // stage id
        [JsonProperty("stageId")]
        public long StageId = 0L;
        // idk
        [JsonProperty("title")]
        public string Title = string.Empty;
        // idk
        [JsonProperty("conditionType")]
        public int ConditionType = 0;
        // idk
        [JsonProperty("description")]
        public string Description = string.Empty;
        // idk
        [JsonProperty("originStageId")]
        public long OriginStageId = 0L;
    }
}
