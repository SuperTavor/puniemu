using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.GetL5IDStatus.DataClasses
{
    public class L5IDStatusEntry
    {
        // code
        [JsonProperty("code")]
        public int DamageMax = 1;
        // name
        [JsonProperty("name")]
        public string ItemType = "???????????????";
        // point
        [JsonProperty("point")]
        public int ItemCnt = 0;
    }
}
