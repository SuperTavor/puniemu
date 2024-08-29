using Newtonsoft.Json;
using Puniemu.Src.Server.L5ID.DataClasses;

namespace Puniemu.Src.Server.L5ID.Requests.CreateGDKey.DataClasses
{
    public struct CreateGDKeyGoodResponse
    {
        [JsonProperty("result")]
        public bool Result { get; set; }
        [JsonProperty("gdkey")]
        public Key GDKey { get; set; }

        [JsonProperty("sign_nonce")]
        public string SignNonce { get; set; }

        [JsonProperty("sign_timestamp")]
        public long SignTimestamp { get; set; }

        public CreateGDKeyGoodResponse(string gdkey)
        {
            Result = true;
            GDKey = new(gdkey);
            SignNonce = "123";
            SignTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }
}
