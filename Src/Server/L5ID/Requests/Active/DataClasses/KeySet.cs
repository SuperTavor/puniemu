using Newtonsoft.Json;
using Puniemu.Src.Server.L5ID.DataClasses;

namespace Puniemu.Src.Server.L5ID.Requests.Active.DataClasses
{
    public struct KeySet
    {
        //ID for the player's client
        [JsonProperty("udkey")]
        public Key UDKey { get; set; }

        //Save file IDs associated with the udkey
        [JsonProperty("gdkeys")]
        public List<Key> GDKeys { get; set; }
    }
}
