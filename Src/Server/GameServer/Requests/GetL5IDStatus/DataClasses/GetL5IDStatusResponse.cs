using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.GetL5IDStatus.DataClasses
{
    public class GetL5IDStatusResponse : PuniemuResponseBase
    {
        //it's always just the default constructor, since we do not support linking L5 accounts everything should be unlocked with the default 9999.... points
        [JsonProperty("L5IdStatus")]
        public L5IDStatus L5IDStatus = new();

        //Unknown. Works as 1 in the tweaked L5IdStatus thing.
        [JsonProperty("beforeCode")]
        public int BeforeCode = 1;

        //Unknown. Works as 1 in the tweaked L5IdStatus thing.
        [JsonProperty("afterCode")]
        public int AfterCode = 1;

        //Unknown. Works as 1 in the tweaked L5IdStatus thing.
        [JsonProperty("maxCode")]
        public int MaxCode = 1;

        //Likely decides if the status of the account has changed since the last check.
        [JsonProperty("isChanged")]
        public bool IsChanged = false;
        public GetL5IDStatusResponse()
        {
            NextScreenType = 0;
            ResultType = 0;
            ResultCode = 0;

        }
    }
}
