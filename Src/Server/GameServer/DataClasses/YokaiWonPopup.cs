using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Logic.Mst;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using System.Runtime.CompilerServices;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    // probably youkai popup, maybe also used for crank-a-kai
    public class YokaiWonPopup
    {
        [JsonIgnore]
        private const int MAX_SOULT = 7;

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
        public YokaiGetType GetTypes = 0;

        [JsonProperty("youkaiId")]
        public long YoukaiId = 0L;

        [JsonProperty("releaseType")]
        public int ReleaseType = 1;

        [JsonProperty("skill")]
        public SkillResult? Skill = null;

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

        public YokaiWonPopup(
            long yokaiId,
            TableParser<YwpUserYoukai> userYokai,
            TableParser<YwpUserYoukaiSkill> userSkill
            )
        {
            IsWBonusEffectOpen = false;
            BonusEffectLevelBefore = 0;
            BonusEffectLevelAfter = 0;
            StrongSkillLevelAfter = 0;
            StrongSkillLevelBefore = 0;
            LegendYoukaiId = MstLegendReleaseManager.CheckLegendYoukaiId(yokaiId);
            LevelBefore = 0;
            LevelAfter = 0;
            YoukaiId = yokaiId;
            ReleaseType = 0;
            ExchgYmoney = 0;
            LimitLevelAfter = 0;
            LimitLevelBefore = 0;
            ReleaseLevelType = 0;

            GetTypes = CheckGetType(yokaiId, userYokai, userSkill);

            if (GetTypes == YokaiGetType.SoultLevelUp)
            {
                Skill = YoukaiManager.AddExpToSkill(userSkill, yokaiId, 1000);
            }
        }
        public static YokaiGetType CheckGetType(long yokaiId, TableParser<YwpUserYoukai> userYokai, TableParser<YwpUserYoukaiSkill> userSkill)
        {
            YokaiGetType getType = 0;
            var yokaiIdx = YoukaiManager.GetYoukaiIndex(userYokai, yokaiId);
            if (yokaiIdx == -1)
            {
                getType = YokaiGetType.NewYokai;
            }
            else
            {
                var soultIdx = YoukaiManager.GetYoukaiSkillIndex(userSkill, yokaiId);
                if (soultIdx == -1) throw new Exception("No soult idx found for yokai");
                var soultLvl = userSkill.Items[soultIdx].Level;
                if (soultLvl == MAX_SOULT)
                {
                    getType = YokaiGetType.MaxLevel;
                }
                else getType = YokaiGetType.SoultLevelUp;
            }
            return getType;
        }
    }
}