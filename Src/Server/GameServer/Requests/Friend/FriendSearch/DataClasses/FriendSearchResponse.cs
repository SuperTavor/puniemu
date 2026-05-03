using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.FriendSearch.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.FriendSearch.DataClasses
{
    public class FriendSearchResponse : CommonResponse
    {
        [JsonProperty("friend")]
        public FriendsEntry? Friend { get; set; }
        [JsonProperty("responseCode")]
        public int ResponseCode { get; set; }
    }
}
