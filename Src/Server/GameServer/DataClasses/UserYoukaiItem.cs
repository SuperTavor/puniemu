using Newtonsoft.Json;
using Puniemu.Src.TableParser.Logic;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class UserYoukaiItem
    {
        //ID of the yokai
        [JsonProperty("youkaiId")]
        public int YoukaiId { get; set; }
        //Skill level of the yokai
        [JsonProperty("skillLv")]
        public int SkillLevel { get; set; }
        //Second skill level
        [JsonProperty("sSkillLv")]
        public int SSkillLevel { get; set; }
        //hp of the youkai
        [JsonProperty("hp")]
        public int Hp { get; set; }

        //Attack strength of the yokai
        [JsonProperty("atkPower")]
        public int AttackPower { get; set; }

        //timestamp of when the yokai was befriended. Only appears in ywp_user_youkai table
        [JsonIgnore]
        public long BefriendTimestamp { get; set; }

    }
}
