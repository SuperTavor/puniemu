using Newtonsoft.Json;

namespace Puniemu.Src.Server.L5ID.API.V1.Active.DataClasses
{
    public struct SRcClientVersion
    {
        [JsonProperty("1")]
        public string One { get; set; }
        [JsonProperty("2")]
        public string Two { get; set; }

        public SRcClientVersion()
        {
            //Both of these are empty in the real response
            One = "";
            Two = "";
        }
    }
}
