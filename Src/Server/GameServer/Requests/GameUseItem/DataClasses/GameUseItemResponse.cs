using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.ConfigManager;
namespace Puniemu.Src.Server.GameServer.Requests.GameUseItem.DataClasses
{
    public class GameUseItemResponse : PuniemuResponseBase
    {
        // Basic user data.
        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }
        [JsonProperty("itemId")]
        public long? ItemId { get; set; }
        [JsonProperty("ywp_user_item")]
        public string ItemData { get; set; }

        public GameUseItemResponse(YwpUserData userData, string itemData, long? item_id)
        {
            this.ResultCode = 0;
            this.NextScreenType = 0;
            this.UserData = userData;
            this.ResultType = 0;
            this.ItemId = item_id;
            this.ItemData = itemData;
        }
    }
}
