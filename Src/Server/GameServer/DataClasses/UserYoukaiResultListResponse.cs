using Newtonsoft.Json;
using Puniemu.Src.TableParser.DataClasses;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class UserYoukaiResultListRes
    {
        // IDK but maybe has level up flg
        [JsonProperty("haveFlg")]
        public bool HaveFlg = false;
        // Max level flg
        [JsonProperty("isMaxLevel")]
        public bool IsMaxLevel = false;
        // level lock (paid) flg
        [JsonProperty("isLockLevel")]
        public bool IsLockLevel = false;
        // old yokai info
        [JsonProperty("before")]
        public ExpInfo Before = new();
        // new yokai info
        [JsonProperty("after")]
        public ExpInfo After = new();
        // yokai id
        [JsonProperty("youkaiId")]
        public long YoukaiId = 0L;
        // can evolve flg
        [JsonProperty("canEvolve")]
        public bool CanEvolve = false;

        public UserYoukaiResultListRes()
        {}
        public UserYoukaiResultListRes(YwpUserYoukai userYoukai, YwpMstYoukai masterYoukai)
        {
            HaveFlg = false;
            IsMaxLevel = (userYoukai.Level >= masterYoukai.MaxLevel);
            CanEvolve = (userYoukai.Level >= masterYoukai.EvolutionLevel);
            YoukaiId = userYoukai.YoukaiId;
            IsLockLevel = (userYoukai.IsLockedLevel == 1);
        }
    }
}
