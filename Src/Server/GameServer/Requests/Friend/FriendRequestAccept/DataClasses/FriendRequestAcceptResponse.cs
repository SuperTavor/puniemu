using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.Friend.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.FriendRequestAccept.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.FriendRequestAccept.DataClasses
{
    //Todo send ywp_user_friend
    public class FriendRequestAcceptResponse : CommonResponse
    {
        [JsonProperty("responseCode")]
        public int ResponseCode { get; set; }
        [JsonProperty("ywp_user_friend_request_recv")]
        public List<FriendRequestEntry>? YwpUserFriendRequestRecv { get; set; }
        [JsonProperty("ywp_user_friend_star_rank")]
        public List<FriendRankEntry>? YwpUserFriendStarRank { get; set; }
        [JsonProperty("ywp_user_friend_rank")]
        public List<FriendRankEntry>? YwpUserFriendRank { get; set; }
        [JsonProperty("ywp_user_friend")]
        public List<FriendEntry?>? YwpUserFriend { get; set; }
    }
}
