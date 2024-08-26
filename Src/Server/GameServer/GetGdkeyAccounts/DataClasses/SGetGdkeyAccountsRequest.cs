using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.GetGdkeyAccounts.DataClasses
{
    public struct SGetGdkeyAccountsRequest
    {
        // Client version
        [JsonProperty("appVer")]
        public string AppVer { get; set; }

        // UDKey
        [JsonProperty("deviceID")]
        public string DeviceID { get; set; }

        /* List of gdkeys in this format:
            [
                {
                    "gdkey" : "g-bvt9d702uuja2ipvlxepica4muohkxp2lf05hg9l8jnf0sg7fci2yrss7q2lho"
                }
            ]
        */
        [JsonProperty("gdkeys")]
        public List<Dictionary<string, string>> GDKeys { get; set; }

        // Always 0 here.
        [JsonProperty("level5UserID")]
        public string Level5UserID { get; set; }

        // Version of assets on the server
        [JsonProperty("mstVersionVer")]
        public int MstVersionVer { get; set; }

        // 2 = Android. We are not focusing on supporting iOS right now.
        [JsonProperty("osType")]
        public int OsType { get; set; }

        // 0 here.
        [JsonProperty("userID")]
        public string UserID { get; set; }

        // 0 here.
        [JsonProperty("ywpToken")]
        public string YWPToken { get; set; }
    }
}
