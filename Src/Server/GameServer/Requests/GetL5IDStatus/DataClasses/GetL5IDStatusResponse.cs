using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.ConfigManager;
namespace Puniemu.Src.Server.GameServer.Requests.GetL5IDStatus.DataClasses
{
    public class GetL5IDStatusResponse : PuniemuResponseBase
    {
        // Basic user data.
        [JsonProperty("L5IdStatus")]
        public L5IDStatusEntry L5IDStatus { get; set; }
        [JsonProperty("beforeCode")]
        public int BeforeCode { get; set; }
        [JsonProperty("afterCode")]
        public int AfterCode { get; set; }
        [JsonProperty("isChanged")]
        public bool IsChanged { get; set; }
        [JsonProperty("maxCode")]
        public int MaxCode { get; set; }
        public GetL5IDStatusResponse()
        {
            this.ResultCode = 0;
            this.NextScreenType = 0;
            this.L5IDStatus = new L5IDStatusEntry();
            this.ResultType = 0;
            this.BeforeCode = 1;
            this.AfterCode = 1;
            this.IsChanged = false;
            this.MaxCode = 1;

        }
    }
}
