using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.FriendRequestDelete.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Buffers;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.Eventing.Reader;


namespace Puniemu.Src.Server.GameServer.Requests.FriendRequestDelete.Logic
{
    public class FriendRequestDeleteHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);

            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<FriendRequestDeleteRequest>(requestJsonString!);

            var res = new FriendRequestDeleteResponse();
            res.YwpUserFriendRequestRecv = await UserDataManager.Logic.DBService.GetYwpUserAsync<List<FriendRequestEntry>>(deserialized!.Level5UserID!, "ywp_user_friend_request_recv");
            int idx = 0;
            int to_delete = -1;
            foreach (FriendRequestEntry element in res.YwpUserFriendRequestRecv!)
            {
                if (element.UserId!.Equals(deserialized!.TargetUserId))
                {
                    if (to_delete == -1)
                    {
                        to_delete = idx;
                    }
                } else
                {
                    element.RequestDtSentence = GameServer.Logic.GenerateFriendData.GetTimeDifferenceString(element.RequestDt!);
                }
                idx++;
            }
            if (to_delete != -1)
            {
                res.YwpUserFriendRequestRecv.RemoveAt(to_delete);
            }
            await UserDataManager.Logic.DBService.SetYwpUserAsync(deserialized.Level5UserID!, "ywp_user_friend_request_recv", res.YwpUserFriendRequestRecv);
            var outDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(res));
            var outResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(outDict));
            await ctx.Response.WriteAsync(outResponse);
        }
    }
}
