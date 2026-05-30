using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class YoukaiSkillExp
    {
        [JsonProperty("isMaxLevel")]
        public bool IsMaxLevel;

        [JsonProperty("before")]
        public ExpInfo Before = new();

        [JsonProperty("after")]
        public ExpInfo After = new();

        [JsonProperty("youkaiId")]
        public long YoukaiID;
    }
}
