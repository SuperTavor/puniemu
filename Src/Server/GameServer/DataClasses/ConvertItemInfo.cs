using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.DataClasses;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class ConvertItemInfo
    {
        //Prize type of the prize before the convert item
        [JsonProperty("originalPrizeType")]
        public PrizeType OGPrizeType;

        //Prize ID of the original prize 
        [JsonProperty("originalPrizeId")]
        public long OGPrizeID;

        //ID of the yokai that skill maxxed
        [JsonProperty("skillMaxYoukaiId")]
        public long SkillMaxYoukaiID;
    }
}
