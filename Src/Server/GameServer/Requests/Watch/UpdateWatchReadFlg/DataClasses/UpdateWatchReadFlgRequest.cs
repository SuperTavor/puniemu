using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.Watch.UpdateWatchReadFlg.DataClasses
{
    public class UpdateWatchReadFlgRequest : CommonRequest
    {
        [JsonProperty("watchId")]
        public int WatchID;
    }
}
