using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.Map.Map.DataClasses
{
    public class MapResponse : CommonResponse
    {
        [JsonProperty("ywp_mst_map")]
        public List<object> YwpMstMap { get; set; }

        [JsonProperty("ywp_mst_event")]
        public List<object> YwpMstEvent { get; set;  }

        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }

        [JsonProperty("ywp_user_map")]
        public string YwpUserMap { get; set; }
    }
}
