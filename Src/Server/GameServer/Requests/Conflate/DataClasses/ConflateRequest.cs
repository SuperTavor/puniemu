using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.Conflate.DataClasses
{
    public class ConflateRequest : CommonRequest
    {
        [JsonProperty("conflateId")]
        public int ConflateID { get; set; }
    }
}
