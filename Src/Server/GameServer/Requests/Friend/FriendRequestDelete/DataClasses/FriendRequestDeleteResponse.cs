using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.FriendRequestDelete.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.FriendRequestDelete.DataClasses
{
    //Todo send ywp_user_friend
    public class FriendRequestDeleteResponse : CommonResponse
    {
        [JsonProperty("responseCode")]
        public int ResponseCode { get; set; }
        [JsonProperty("ywp_user_friend_request_recv")]
        public List<FriendRequestEntry>? YwpUserFriendRequestRecv { get; set; }
    }
}
