using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.GameEnd.DataClasses
{
    public class UserYoukaiResultListResExpBar
    {
        // percentage of denominator and numerator
        [JsonProperty("pctg")]
        public int Percentage = 0;
        // denominator | number of exp max for this level
        [JsonProperty("denominator")]
        public int Denominator = 0;
        // numerator | number of exp actually gained for this level
        [JsonProperty("numerator")]
        public int Numerator = 0;
    }
    public class UserYoukaiResultListResExpInfo
    {
        //Yokai level
        [JsonProperty("level")]
        public int Level = 0;
        //Yokai exp
        [JsonProperty("exp")]
        public int Exp = 0;
        //Exp Bar
        [JsonProperty("expBar")]
        public UserYoukaiResultListResExpBar ExpBar = new();
    }
    public class UserYoukaiResultListRes
    {
        // IDK but maybe has level up flg
        [JsonProperty("haveFlg")]
        public bool HaveFlg = false;
        // Max level flg
        [JsonProperty("isMaxLevel")]
        public bool isMaxLevel = false;
        // level lock (paid) flg
        [JsonProperty("isLockLevel")]
        public bool IsLockLevel = false;
        // old yokai info
        [JsonProperty("before")]
        public UserYoukaiResultListResExpInfo Before = new();
        // new yokai info
        [JsonProperty("after")]
        public UserYoukaiResultListResExpInfo After = new();
        // yokai id
        [JsonProperty("youkaiId")]
        public long youkaiId = 0L;
        // can evolve flg
        [JsonProperty("canEvolve")]
        public bool CanEvolve = false;

    }
}
