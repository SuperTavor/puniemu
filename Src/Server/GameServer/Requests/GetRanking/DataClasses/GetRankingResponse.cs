using System.Collections.Generic;
using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.FriendRequestAccept.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.FriendRequestAccept.Logic;

namespace Puniemu.Src.Server.GameServer.Requests.GetRanking.DataClasses
{
    public class GetRankingResponse : CommonResponse
    {
        [JsonProperty("rankType")]
        public int RankType { get; set; }
        [JsonProperty("responseCode")]
        public int ResponseCode { get; set; }
    }
}
