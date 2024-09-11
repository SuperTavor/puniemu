using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class EnableFoodInfo
    {
        //item id for smth
        [JsonProperty("itemId")]
        public int ItemID = 0;

        //maybe dictates if hte item should be used?
        [JsonProperty("itemFlg")]
        public int ItemFlag = 0;
    }
}
