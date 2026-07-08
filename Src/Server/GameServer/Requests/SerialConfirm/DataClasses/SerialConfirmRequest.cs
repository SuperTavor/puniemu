using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.SerialConfirm.DataClasses
{
    public class SerialConfirmRequest : CommonRequest
    {
        [JsonProperty("serialCode")]
        public string SerialCode { get; set; }
    }
}
