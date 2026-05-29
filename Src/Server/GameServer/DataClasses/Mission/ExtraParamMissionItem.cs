using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses.Mission
{
    public class ExtraParamMissionItem
    {
        [JsonProperty("id")]
        public int ID;

        [JsonProperty("param2")]
        public int ExtraParam;
    }
}
