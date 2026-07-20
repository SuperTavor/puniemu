using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.FriendSearch.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Buffers;
using System.Text;
using System.Diagnostics.Eventing.Reader;


namespace Puniemu.Src.Server.GameServer.Requests.FriendSearch.Logic
{
    public class FriendSearchHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);

            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<FriendSearchRequest>(requestJsonString!);

            var res = new FriendSearchResponse();

            string targetGdkey = await UserDataManager.Logic.UserDataManager.GetGdkeyFromCharacterId(deserialized!.TargetCharacterID!)!;

            res.Friend = null;
            res.ResponseCode = 1;
            if (!string.IsNullOrEmpty(targetGdkey))
            {
                YwpUserData? targetUserData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(targetGdkey!, "ywp_user_data");
                if (targetUserData != null)
                {
                    res.ResponseCode = 0;
                    res.Friend = new();
                    res.Friend.PlayerName = targetUserData.PlayerName;
                    res.Friend.LastPlayDt = await UserDataManager.Logic.UserDataManager.GetLastLoginTime(targetGdkey);
                    res.Friend.IconId = targetUserData.IconID;
                    res.Friend.YoukaiId = targetUserData.YoukaiId;
                    res.Friend.LastPlayDtSentence = "🥺​"; 
                    res.Friend.TitleId = targetUserData.CharacterTitleID;
                    res.Friend.CharacterId = targetUserData.CharacterID;
                    res.Friend.UserId = targetUserData.UserID;
                }
            }
            var outDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(res));
            var outResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(outDict));
            await ctx.Response.WriteAsync(outResponse);
        }
    }
}
