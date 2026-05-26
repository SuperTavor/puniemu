using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.LevelLockOff.DataClasses
{
    public class LevelLockOffRequest : CommonRequest
    {
        //YokaiID of yokai to remove lock from
        [JsonProperty("youkaiId")]
        public long YokaiID;

        //Cost to lock off
        [JsonProperty("dispCost")]
        public int LockOffCost;
    }
}
