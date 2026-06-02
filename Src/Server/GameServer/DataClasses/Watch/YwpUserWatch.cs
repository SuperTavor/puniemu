using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses.Watch
{
    public class YwpUserWatch
    {
        [JsonProperty("watchStatus")]
        public int WatchStatus { get; set; } //idk

        [JsonProperty("watchId")]
        public int WatchID { get; set; } //id of the watch

        [JsonProperty("equipFlg")]
        public int EquipFlag { get; set; } //is equipped

        [JsonProperty("readFlg")]
        public int ReadFlag { get; set; } //was the watch build menu for the watchId ever looked into?
    }
}
