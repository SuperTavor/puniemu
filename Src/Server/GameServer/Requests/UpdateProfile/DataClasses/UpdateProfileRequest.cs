using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.UpdateProfile.DataClasses
{
    public struct UpdateProfileRequest
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

        // ID of the user title
        [JsonProperty("titleID")]
        public int TitleID { get; set; }

        [JsonProperty("effectId")]
        public int EffectID { get; set; }

        [JsonProperty("plateId")]
        public int PlateID { get; set; }

        [JsonProperty("codenameId")]
        public int CodenameID { get; set; }

        // Interchangeable with gdkey
        [JsonProperty("level5UserID")]
        public string Level5UserID { get; set; }

        // Version of server data
        [JsonProperty("mstVersionVer")]
        public int MstVersionVer { get; set; }

        // OS type. 2 is Android.
        [JsonProperty("osType")]
        public int OsType { get; set; }

        // Always 0 here
        [JsonProperty("userID")]
        public string UserID { get; set; }

        // Always 0
        [JsonProperty("ywpToken")]
        public string YwpToken { get; set; }
    }
}
