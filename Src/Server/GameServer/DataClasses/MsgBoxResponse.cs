using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class MsgBoxResponse
    {
        public MsgBoxResponse(string dialogMsg, string dialogTitle)
        {
            this.DialogMessage = dialogMsg;
            this.DialogTitle = dialogTitle;
            this.GameServerURL = Consts.OG_GAMESERVER_URL;
            this.ResultCode = 0;
            this.StoreURL = "";
            this.ResultType = 2;
            this.NextScreenType = 1;
        }
        // The dialog message to be displayed
        [JsonProperty("dialogMsg")]
        public string DialogMessage { get; set; }

        // Can be gotten from the server's stored gameconsts.
        [JsonProperty("gameServerUrl")]
        public string GameServerURL { get; set; }

        [JsonProperty("resultCode")]
        public int ResultCode { get; set; }

        // should be an empty string to not send user to url
        [JsonProperty("storeUrl")]
        public string StoreURL { get; set; }

        // the title of the dialog box
        [JsonProperty("dialogTitle")]
        public string DialogTitle { get; set; }

        [JsonProperty("resultType")]
        public int ResultType { get; set; }

        [JsonProperty("nextScreenType")]
        public int NextScreenType { get; set; }
    }
}
