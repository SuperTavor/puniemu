using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.GameRetire.DataClasses
{
    public class UserYoukaiResultListReq
    {
        [JsonProperty("youkaiId")]
        public long YoukaiId = 0L;
        // Maybe max damage in one attack
        [JsonProperty("damageMax")]
        public int DamageMax = 0;
        // Total damage
        [JsonProperty("damageTotal")]
        public int DamageTotal = 0;
        // probably number of puni we click
        [JsonProperty("eraseNum")]
        public int EraseNum = 0;
        // idk
        [JsonProperty("eraseSize")]
        public int EraseSize = 0;
        // maybe the maximum size of the puni we click
        [JsonProperty("eraseSizeMax")]
        public int EraseSizeMax = 0;
        // maybe the maximum link
        [JsonProperty("linkSizeMax")]
        public int LinkSizeMax = 0;
        // no idea
        [JsonProperty("recoveryActual")]
        public int RecoveryActual = 0;
        // no idea
        [JsonProperty("recoveryMax")]
        public int RecoveryMax = 0;
        // second skill used num
        [JsonProperty("sSkillUseNum")]
        public int SSkillUseNum = 0;
        // Skill used num
        [JsonProperty("skillUseNum")]
        public int SkillUseNum = 0;
    }
}
