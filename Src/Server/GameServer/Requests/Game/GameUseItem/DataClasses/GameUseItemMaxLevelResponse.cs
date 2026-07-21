using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.GameUseItem.DataClasses
{

    //CREATED BY WibWob_YT
    public class GameUseItemMaxLevelResponse : GameUseItemResponse
    {
        [JsonProperty("dialogMsg")]
        public string DialogMsg { get; set; }

        [JsonProperty("dialogTitle")]
        public string DialogTitle { get; set; }

        public GameUseItemMaxLevelResponse(YwpUserData userData, string itemData, long? item_id)
            : base(userData, itemData, item_id)
        {
            this.ResultType = 2;
            this.NextScreenType = 1;
            this.DialogMsg = "This Yo-kai's Amiability is\nalready at its maximum level.";
            this.DialogTitle = "Error";
        }
    }
}