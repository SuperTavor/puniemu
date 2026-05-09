using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.Friend.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Buffers;
using System.Text;


namespace Puniemu.Src.Server.GameServer.Requests.Friend.Logic
{
    public class FriendsHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);

            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<FriendsRequest>(requestJsonString!)!;

            var instance = new FriendsResponse();
            instance.YwpUserFriendRequestRecv = await UserDataManager.Logic.DBService.GetYwpUserAsync<List<FriendRequestEntry>>(deserialized.Level5UserID!, "ywp_user_friend_request_recv");
            foreach (FriendRequestEntry item in instance.YwpUserFriendRequestRecv!)
            {
                item.RequestDtSentence = GameServer.Logic.GenerateFriendData.GetTimeDifferenceString(item.RequestDt!);
            }
            instance.YwpUserFriend = await UserDataManager.Logic.DBService.GetYwpUserAsync<List<FriendEntry>>(deserialized.Level5UserID!, "ywp_user_friend");
            foreach (FriendEntry item in instance.YwpUserFriend!)
            {
                item.LastPlayDtSentence = GameServer.Logic.GenerateFriendData.GetTimeDifferenceString(item.LastPlayDt!);
            }

            instance.YwpUserFriendStarRank = await UserDataManager.Logic.DBService.GetYwpUserAsync<List<FriendRankEntry>>(deserialized.Level5UserID!, "ywp_user_friend_star_rank");
            instance.YwpUserFriendRank = await UserDataManager.Logic.DBService.GetYwpUserAsync<List<FriendRankEntry>>(deserialized.Level5UserID!, "ywp_user_friend_rank");

            var outDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(instance));
            var outResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(outDict));
            await ctx.Response.WriteAsync(outResponse);
        }
    }
}
