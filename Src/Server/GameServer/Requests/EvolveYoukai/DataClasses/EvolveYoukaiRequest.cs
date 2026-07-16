using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.EvolveYoukai.DataClasses
{
    public class EvolveYoukaiRequest : CommonRequest
    {
        [JsonProperty("youkaiId")]
        public long YokaiID { get; set; }
    }
}
