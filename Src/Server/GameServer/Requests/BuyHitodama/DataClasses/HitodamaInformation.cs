using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.BuyHitodama.DataClasses
{
    public class HitodamaInformation
    {
        //Gotten from ywp_user_data
        [JsonProperty("freeHitodama")]
        public int FreeHitodama;
        [JsonProperty("hitodama")]
        public int Hitodama;

        public HitodamaInformation(int hito, int freeHito)
        {
            Hitodama = hito;
            FreeHitodama = freeHito;
        }
    }
}
