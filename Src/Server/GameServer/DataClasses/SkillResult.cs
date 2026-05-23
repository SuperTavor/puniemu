using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class SkillResult
    {
        [JsonProperty("skillId")]
        public long SkillID = 0;
        // Max level flg, iirc it's 7
        [JsonProperty("isMaxLevel")]
        public bool isMaxLevel = false;
        // old yokai info
        [JsonProperty("before")]
        public ExpInfo Before = new();
        // new yokai info
        [JsonProperty("after")]
        public ExpInfo After = new();
    }
}
