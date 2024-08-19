using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json;
using Puniemu.src.Utils.UserDataManager;
using Puniemu.Src.Server.L5ID.DataClasses;
using System.Collections;
namespace Puniemu.Src.Server.L5ID.API.V1.Active.DataClasses
{
    public class CGoodActiveResponse
    {
        //True if the response is good. Obviously true in our case
        [JsonProperty("result")]
        public bool Result { get; set; }
        //Contains some keys. Look in the Keyset struct for more infromation about them.
        [JsonProperty("keys")]
        public SKeySet[] UserKeys { get; set; }
        //These do the same thing as the UDKEY and gdkeys in the keyset, but they are placed directly in this struct, as sometimes the game requires it.
        [JsonProperty("udkey")]
        public CKey UnwrappedUDKey { get; set; }
        [JsonProperty("gdkeys")]
        public List<CKey> UnwrappedGDKeys { get; set; }
        //dictates if the udkey is connected to a L5ID account. We don't support those, so no.
        [JsonProperty("is_linked")]
        public bool IsLinked { get; set; }
        //Always 3.
        [JsonProperty("max_gdkeys")]
        public int MaxGDKeys { get; set; }
        //I'm not too sure what this does, but setting both variables to "" seems to work.
        [JsonProperty("rc_client_version")]
        public SRcClientVersion RCClientVersion { get; set; }
        //Account creation time in Unix time
        [JsonProperty("sign_timestamp")]
        public long SignTimestamp { get; set; }
        //Idk what this does, but setting it to anything but 0 works.
        [JsonProperty("sign_nonce")]
        public string SignNonce { get; set; }


        /*Using this instead of the constructors as constructors don't support async.
        Refer to comments on the actual class properties for explanations on the wrappedKeySet
        and anything else you are not sure about. If you still have questions, fire an issue!
        */
        public static async Task<CGoodActiveResponse> CreateAsync(string udkeyValue)
        {
            var gdkeys = new List<CKey>();
            var udkey = new CKey(udkeyValue);
            foreach (var key in await CUserDataManager.GetGdkeysFromUdkeyAsync(udkeyValue))
            {
                gdkeys.Add(udkey);
            }
            
            var wrappedKeySet = new SKeySet[]
            {
                new SKeySet
                {
                    GDKeys = gdkeys,
                    UDKey = udkey
                }
            };

            var instance = new CGoodActiveResponse
            {
                Result=true,
                UserKeys=wrappedKeySet,
                UnwrappedUDKey=udkey,
                UnwrappedGDKeys=gdkeys,
                IsLinked = false,
                MaxGDKeys = 3,
                RCClientVersion = new SRcClientVersion(),
                SignTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                SignNonce="123"
            };

            return instance;
        }
    }
}
