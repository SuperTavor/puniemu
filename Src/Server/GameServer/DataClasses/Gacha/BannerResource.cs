using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses.Gacha
{
    public class BannerResource
    {
        //prob the path to the banner png
        [JsonProperty("resName")]
        public string? ResourceName { get; set; }

        [JsonProperty("gachaType")]
        public int GachaType { get; set; }


        //has a userId at the end seemingly? not sure if mistake, hopefully doesnt cause problems down the line but fixable fsr
        [JsonProperty("webviewId")]
        public string? WebViewId { get; set; }
    }
}
