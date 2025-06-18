using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class WrappedTable
    {
        [JsonProperty("tableData")]
        public string? TableData { get; set; }
        [JsonProperty("version")]
        public int Version { get; set; }
    }
}
