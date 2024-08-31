using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.BuyHitodama.DataClasses
{
    public class BuyHitodamaResponse: PuniemuResponseBase
    {
        [JsonProperty("before")]
        public HitodamaInformation Before { get; set; }

        [JsonProperty("after")]
        public HitodamaInformation After { get; set; }

        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }

        public BuyHitodamaResponse(HitodamaInformation before, HitodamaInformation after, YwpUserData udata)
        {
            Before = before;
            After = after;
            UserData = udata;
        }
    }
}
