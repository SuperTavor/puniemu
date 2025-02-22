using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    // probably youkai popup, maybe also used for crank-a-kai
    public class YokaiWonPopup
    {
        [JsonProperty("bonusEffectLevelAfter")]
        public int BonusEffectLevelAfter = 0;
        [JsonProperty("strongSkillLevelBefore")]
        public int StrongSkillLevelBefore = 0;
        [JsonProperty("bonusEffectLevelBefore")]
        public int BonusEffectLevelBefore = 0;
        // lengendary youkai popup flg
        [JsonProperty("legendYoukaiId")]
        public long LegendYoukaiId = 0L;
        [JsonProperty("strongSkillLevelAfter")]
        public int StrongSkillLevelAfter = 0;
        [JsonProperty("isWBonusEffectOpen")]
        public bool IsWBonusEffectOpen = false;
        [JsonProperty("levelAfter")]
        public int LevelAfter = 1;
        [JsonProperty("levelBefore")]
        public int LevelBefore = 1;
        [JsonProperty("getType")]
        public int GetTypes = 0;
        [JsonProperty("youkaiId")]
        public long YoukaiId = 0L;
        [JsonProperty("releaseType")]
        public int ReleaseType = 1;

        // idk cause was set to null in the response
        [JsonProperty("skill")]
        public object ?skill = null;

        [JsonProperty("exchgYmoney")]
        public int ExchgYmoney = 0;
        [JsonProperty("exp")]
        public int Exp = 0;
        [JsonProperty("limitLevelBefore")]
        public int LimitLevelBefore = 0;
        [JsonProperty("limitLevelAfter")]
        public int LimitLevelAfter = 0;

        [JsonProperty("releaseLevelType")]
        public int ReleaseLevelType = 0;
    }
}