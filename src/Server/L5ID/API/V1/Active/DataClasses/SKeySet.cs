using Newtonsoft.Json;
using Puniemu.Src.Server.L5ID.DataClasses;

namespace Puniemu.Src.Server.L5ID.API.V1.Active.DataClasses
{
    public struct SKeySet
    {
        //ID for the player's client
        [JsonProperty("udkey")]
        public CKey UDKey { get; set; }

        //Save file IDs associated with the udkey
        [JsonProperty("gdkeys")]
        public List<CKey> GDKeys { get; set; }
    }
}
