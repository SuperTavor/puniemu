using Newtonsoft.Json;

namespace Puniemu.Src.Server.L5ID.Requests.Active.DataClasses
{
    public struct RcClientVersion
    {
        [JsonProperty("1")]
        public string One { get; set; }
        [JsonProperty("2")]
        public string Two { get; set; }

        public RcClientVersion()
        {
            //Both of these are empty in the real response
            One = "";
            Two = "";
        }
    }
}
