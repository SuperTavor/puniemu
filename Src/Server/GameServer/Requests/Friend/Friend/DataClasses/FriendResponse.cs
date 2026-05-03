using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.Friend.DataClasses
{
    public class FriendsResponse : CommonResponse
    {
        [JsonProperty("ywp_user_data")]
        public YwpUserData? YwpUserData { get; set; }

        [JsonProperty("ywp_user_friend")]
        public List<FriendEntry>? YwpUserFriend { get; set; }

        [JsonProperty("ywp_user_friend_rank")]
        public List<FriendRankEntry>? YwpUserFriendRank { get; set; } = new();

        [JsonProperty("ywp_user_friend_star_rank")]
        public List<FriendRankEntry>? YwpUserFriendStarRank { get; set; } = new();
        [JsonProperty("ywp_user_friend_request_recv")]
        public List<FriendRequestEntry>? YwpUserFriendRequestRecv { get; set; }
    }
}
