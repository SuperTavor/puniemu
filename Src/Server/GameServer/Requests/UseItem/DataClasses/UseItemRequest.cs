using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.UseItem.DataClasses
{
    public class UseItemRequest : CommonRequest
    {
        //Item to use
        [JsonProperty("itemId")]
        public int ItemID;


        //Yokai to use item on
        [JsonProperty("youkaiId")]
        public long YoukaiID;
    }
}
