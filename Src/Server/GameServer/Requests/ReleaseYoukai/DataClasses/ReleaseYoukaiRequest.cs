using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.ReleaseYoukai.DataClasses
{
    public class ReleaseYoukaiRequest : CommonRequest
    {
        //id of the legends
        [JsonProperty("youkaiId")]
        public int YoukaiID { get; set; }
    }
}
