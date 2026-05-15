using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Reflection;
using System.Text;

namespace Puniemu.Src.TableParser.DataClasses
{
    public class StageConditionItem
    {
        [JsonProperty("conditionId")]
        public long ConditionId { get; set; }
        [JsonProperty("conditionType")]
        public ConditionType ConditionType { get; set; }
        [JsonProperty("description")]
        public string? Description { get; set; }
        [JsonProperty("conditionVal1")]
        public int ConditionVal1 { get; set; }
        [JsonProperty("conditionVal2")]
        public int ConditionVal2 { get; set; }
        [JsonProperty("conditionVal3")]
        public int ConditionVal3 { get; set; }

        [JsonProperty("openStageIdList")]
        public string OpenStageIdList = ""; //used in very few conditions
    }

}