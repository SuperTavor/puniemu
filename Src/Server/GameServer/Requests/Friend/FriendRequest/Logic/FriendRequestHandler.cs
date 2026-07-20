using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.FriendRequest.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Buffers;
using System.Text;
using System.Diagnostics.Eventing.Reader;


namespace Puniemu.Src.Server.GameServer.Requests.FriendRequest.Logic
{
    public class FriendRequestHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);

            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<FriendRequestRequest>(requestJsonString!);

            var res = new FriendRequestResponse();

            string targetGdkey = await UserDataManager.Logic.UserDataManager.GetGdkeyFromUserId(deserialized!.TargetUserId!)!;

            res.ResponseCode = 0;
            bool good = false;
            if (!string.IsNullOrEmpty(targetGdkey))
            {
                YwpUserData? myUserData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized!.Level5UserID!, "ywp_user_data");
                YwpUserData? targetUserData = (await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(targetGdkey!, "ywp_user_data"))!;
                good = (targetUserData != null && myUserData != null);
                if (good)
                {
                    good = (deserialized!.TargetUserId != myUserData!.UserID);
                }
                if (good)
                {
                    var myFriendList = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<object>>(deserialized.Level5UserID!, "ywp_user_friend");
                    var targetFriendList = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<object>>(targetGdkey!, "ywp_user_friend");
                    good = true;
                    if (targetFriendList!.Count > targetUserData!.FriendMaxCount)
                    {
                        res.ResponseCode = 2;
                    }
                    else if (myFriendList!.Count > myUserData!.FriendMaxCount)
                    {
                        res.ResponseCode = 1;
                    }
                    else
                    {
                        var targetFriendRequestList = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<FriendRequestEntry>>(targetGdkey!, "ywp_user_friend_request_recv");
                        FriendRequestEntry entry = new();
                        int idx = 0;
                        foreach (FriendRequestEntry element in targetFriendRequestList!)
                        {
                            if (element.UserId!.Equals(myUserData.UserID))
                            {
                                targetFriendRequestList.RemoveAt(idx);
                                break;
                            }
                            idx++;
                        }
                        while (targetFriendList.Count >= 50)
                        {
                            targetFriendList.RemoveAt(0);
                        }
                        entry.PlayerName = myUserData.PlayerName;
                        entry.RequestDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        entry.IconId = myUserData.IconID;
                        entry.YoukaiId = myUserData.YoukaiId;
                        entry.RequestDtSentence = "​";
                        entry.TitleId = myUserData.CharacterTitleID;
                        entry.CharacterId = myUserData.CharacterID;
                        entry.UserId = myUserData.UserID;
                        targetFriendRequestList.Add(entry);
                        await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(targetGdkey!, "ywp_user_friend_request_recv", targetFriendRequestList);
                    }
                }
            }
            if (!good)
            {
                var errSession = new MsgBoxResponse("Error occured", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                return;
            }
            res.YwpUserFriendRequestRecv = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<FriendRequestEntry>>(deserialized.Level5UserID!, "ywp_user_friend_request_recv");
            foreach (FriendRequestEntry item in res.YwpUserFriendRequestRecv!)
            {
                item.RequestDtSentence = GameServer.Logic.GenerateFriendData.GetTimeDifferenceString(item.RequestDt!);
            }
            var outDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(res));
            var outResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(outDict));
            await ctx.Response.WriteAsync(outResponse);
        }
    }
}
