using Newtonsoft.Json;
using Puniemu.Src.Server.L5ID.DataClasses;

namespace Puniemu.Src.Server.L5ID.API.V1.CreateGDKey.DataClasses
{
    public struct SCreateGDKeyGoodResponse
    {
        [JsonProperty("result")]
        public bool Result { get; set; }
        [JsonProperty("gdkey")]
        public CKey GDKey { get; set; }

        [JsonProperty("sign_nonce")]
        public string SignNonce { get; set; }

        [JsonProperty("sign_timestamp")]
        public long SignTimestamp { get; set; }

        public SCreateGDKeyGoodResponse(string gdkey)
        {
            Result = true;
            GDKey = new(gdkey);
            SignNonce = "123";
            SignTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        }
    }
}
