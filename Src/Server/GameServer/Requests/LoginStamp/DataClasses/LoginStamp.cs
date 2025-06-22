using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.LoginStamp.DataClasses
{
    public class LoginStampType
    {
        [JsonProperty("footerResName")]
        public string FooterResName = "";
        [JsonProperty("stampId")]
        public int StampId = 0;
        [JsonProperty("description")]
        public string Description = "";
        [JsonProperty("endDt")]
        public string EndDt = "00/00";
        [JsonProperty("startDt")]
        public string StartDt = "00/00";
        [JsonProperty("titleResName")]
        public string TitleResName = "";
        [JsonProperty("cautionResName")]
        public string CautionResName = "";
        [JsonProperty("mainResName")]
        public string MainResName = "";
        [JsonProperty("headerResName")]
        public string HeaderResName = "";
    }
}
