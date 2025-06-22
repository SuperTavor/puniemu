using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.GameRetire.DataClasses
{
    public class EnemyYoukaiResultList
    {
        //Yokai dead order
        [JsonProperty("defeatFlg")]
        public int DefeatFLG = 0;
        //Yokai dead type
        [JsonProperty("hp")]
        public int Hp = 0;
        // id of the enemy (we need to search the youkaiID with this one)
        [JsonProperty("enemyId")]
        public long EnemyId = 0L;
    }
}
