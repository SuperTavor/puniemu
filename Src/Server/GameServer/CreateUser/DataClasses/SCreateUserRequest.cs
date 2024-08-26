using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.CreateUser.DataClasses
{
    public struct SCreateUserRequest
    {
        // Client version
        [JsonProperty("appVer")]
        public string AppVer { get; set; }

        // Equivalent to UDKey
        [JsonProperty("deviceID")]
        public string DeviceID { get; set; }

        // ID of the user icon
        [JsonProperty("iconID")]
        public int IconID { get; set; }

        // Interchangeable with gdkey
        [JsonProperty("level5UserID")]
        public string Level5UserID { get; set; }

        // Version of server data
        [JsonProperty("mstVersionVer")]
        public int MstVersionVer { get; set; }

        // OS type. 2 is Android.
        [JsonProperty("osType")]
        public int OsType { get; set; }

        // Player's name
        [JsonProperty("playerName")]
        public string PlayerName { get; set; }

        // Seems to be '\u0001' no matter which gender you choose.
        [JsonProperty("playerSexType")]
        public string PlayerSexType { get; set; }

        // Always 0 here
        [JsonProperty("userID")]
        public string UserID { get; set; }

        // Always 0
        [JsonProperty("ywpToken")]
        public string YwpToken { get; set; }
    }
}
