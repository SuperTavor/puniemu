using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.SerialConfirm.DataClasses
{
    public class SerialConfirmRequest : CommonRequest
    {
        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }
        
        [JsonProperty("serialCode")]
        public string SerialCode { get; set; }
    }
}
