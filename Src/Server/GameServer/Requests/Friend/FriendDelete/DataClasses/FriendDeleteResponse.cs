using System.Collections.Generic;
using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.FriendRequestAccept.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.FriendDelete.DataClasses
{
    public class FriendDeleteResponse : CommonResponse
    {
        [JsonProperty("responseCode")]
        public int ResponseCode { get; set; }

        [JsonProperty("ywp_user_data")]
        public YwpUserData? YwpUserData { get; set; }

        [JsonProperty("ywp_user_friend")]
        public List<FriendEntry>? YwpUserFriend { get; set; }

        [JsonProperty("ywp_user_friend_rank")]
        public List<FriendEntry>? YwpUserFriendRank { get; set; }

        [JsonProperty("ywp_user_friend_star_rank")]
        public List<FriendRankEntry>? YwpUserFriendStarRank { get; set; }

        [JsonProperty("ywp_user_friend_request_recv")]
        public List<FriendRequestEntry>? YwpUserFriendRequestRecv { get; set; }
    }
}
