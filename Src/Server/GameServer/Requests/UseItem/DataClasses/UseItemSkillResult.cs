using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.UseItem.DataClasses
{
    public class UseItemSkillResult
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
