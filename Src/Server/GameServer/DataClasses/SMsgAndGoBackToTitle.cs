using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public struct SMsgAndGoBackToTitle
    {
        public SMsgAndGoBackToTitle(string dialogMsg, string dialogTitle)
        {
            this.DialogMessage = dialogMsg;
            this.DialogTitle = dialogTitle;
            this.GameServerURL = CConsts.OG_GAMESERVER_URL;
            this.ResultCode = 1;
            this.StoreURL = "";
            this.ResultType = 1;
            this.NextScreenType = 3;
        }
        // The dialog message to be displayed
        [JsonProperty("dialogMsg")]
        public string DialogMessage { get; set; }

        // Can be gotten from the server's stored gameconsts.
        [JsonProperty("gameServerUrl")]
        public string GameServerURL { get; set; }

        // always 1 when returning to title
        [JsonProperty("resultCode")]
        public int ResultCode { get; set; }

        // should be an empty string to not send user to url
        [JsonProperty("storeUrl")]
        public string StoreURL { get; set; }

        // the title of the dialog box
        [JsonProperty("dialogTitle")]
        public string DialogTitle { get; set; }

        // always 1 when returning to title
        [JsonProperty("resultType")]
        public int ResultType { get; set; }

        // Should be 3 when returning to title
        [JsonProperty("nextScreenType")]
        public int NextScreenType { get; set; }
    }
}
