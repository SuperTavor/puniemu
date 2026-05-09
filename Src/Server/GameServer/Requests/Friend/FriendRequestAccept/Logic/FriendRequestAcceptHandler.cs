using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.FriendRequestAccept.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Buffers;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.Eventing.Reader;
using System.Xml.Linq;


namespace Puniemu.Src.Server.GameServer.Requests.FriendRequestAccept.Logic
{
    public class FriendRequestAcceptHandler
    {

        static void CreateUserRank(string targetUserId, List<FriendRankEntry> send, List<FriendRankEntry> recv)
        {
            bool found = false;
            foreach (FriendRankEntry element in send!)
            {
                if (element.Self == 1)
                {
                    found = false;
                    foreach (FriendRankEntry element2 in recv!)
                    {
                        if (element2?.UserId == targetUserId)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        recv.Add(new FriendRankEntry
                        {
                            IconId = element.IconId,
                            PlayerName = element.PlayerName,
                            TitleId = element.TitleId,
                            GetStar = element.GetStar,
                            UserId = element.UserId,
                            DicCnt = element.DicCnt,
                            Score = element.Score,
                            YoukaiId = element.YoukaiId,
                            GetStarModiDt = element.GetStarModiDt,
                            HitodamaSendFlg = 0,
                            OnedariSendFlg = 0,
                            Rank = 1,
                            Self = 0,
                        });
                    }
                    break;
                }
            }
        }
        public static async Task HandleAsync(HttpContext ctx)
        {
            // Todo add other ywp_user_friend_*
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);

            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest)!;
            var deserialized = JsonConvert.DeserializeObject<FriendRequestAcceptRequest>(requestJsonString)!;

            var res = new FriendRequestAcceptResponse();
            var my = (await DBService.Logic.DBService.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID!, "ywp_user_data"))!;
            var tgdkey = await DBService.Logic.DBService.GetGdkeyFromUserId(deserialized.TargetUserId!);
            var YwpUserFriendRequestRecvFriend = await DBService.Logic.DBService.GetYwpUserAsync<List<FriendRequestEntry>>(tgdkey, "ywp_user_friend_request_recv");
            res.YwpUserFriendRequestRecv = await DBService.Logic.DBService.GetYwpUserAsync<List<FriendRequestEntry>>(deserialized.Level5UserID!, "ywp_user_friend_request_recv");
            int idx = 0;
            bool found = false;
            res.YwpUserFriend = null;
            res.ResponseCode = 1;
            List<FriendEntry>? YwpUserFriendFriend = null;
            FriendEntry usr = new FriendEntry();
            foreach (FriendRequestEntry element in res.YwpUserFriendRequestRecv!)
            {
                if (element.UserId!.Equals(deserialized!.TargetUserId))
                {
                    var elemntUsrdata = await DBService.Logic.DBService.GetYwpUserAsync<YwpUserData>(tgdkey, "ywp_user_data");

                    res.YwpUserFriendRequestRecv.RemoveAt(idx);
                    found = true;
                    usr.YoukaiId = elemntUsrdata!.YoukaiId;
                    usr.UserId = elemntUsrdata.UserID;
                    usr.CharacterId = elemntUsrdata.CharacterID;
                    usr.OnedariSendFlg = 0;
                    usr.HitodamaSendFlg = 0;
                    usr.MapLockSendFlg = 0; 
                    usr.PlayerName = elemntUsrdata.PlayerName;
                    usr.IconId = elemntUsrdata.IconID;
                    usr.LastPlayDt = await DBService.Logic.DBService.GetLastLoginTime(tgdkey);
                    usr.LastPlayDtSentence = GameServer.Logic.GenerateFriendData.GetTimeDifferenceString(usr.LastPlayDt);
                    usr.TitleId = elemntUsrdata.CharacterTitleID;
                    break;
                }
                idx++;
            }
            if (found)
            {
                FriendEntry me = new FriendEntry();
                me.YoukaiId = my.YoukaiId;
                me.UserId = my.UserID;
                me.CharacterId = my.CharacterID;
                me.OnedariSendFlg = 0;
                me.HitodamaSendFlg = 0;
                me.MapLockSendFlg = 0;
                me.PlayerName = my.PlayerName;
                me.IconId = my.IconID;
                me.LastPlayDt = await DBService.Logic.DBService.GetLastLoginTime(deserialized.Level5UserID!);
                me.LastPlayDtSentence = GameServer.Logic.GenerateFriendData.GetTimeDifferenceString(usr.LastPlayDt!);
                me.TitleId = my.CharacterTitleID;

                res.YwpUserFriend = (await DBService.Logic.DBService.GetYwpUserAsync<List<FriendEntry>>(deserialized.Level5UserID!, "ywp_user_friend"))!;
                res.ResponseCode = 0;
                YwpUserFriendFriend = await DBService.Logic.DBService.GetYwpUserAsync<List<FriendEntry>>(tgdkey, "ywp_user_friend");
                found = false;
                foreach( FriendEntry item in res.YwpUserFriend!)
                {
                    if (item.UserId == usr.UserId)
                        found = true;
                }
                if (!found)
                    res.YwpUserFriend.Add(usr);
                found = false;
                foreach (FriendEntry item in YwpUserFriendFriend!)
                {
                    if (item.UserId == me.UserId)
                        found = true;
                }
                if (!found)
                    YwpUserFriendFriend.Add(me);
            }
            idx = 0;
            foreach (FriendRequestEntry element in YwpUserFriendRequestRecvFriend!)
            {
                if (element.UserId!.Equals(my!.UserID))
                {
                    YwpUserFriendRequestRecvFriend.RemoveAt(idx);
                    break;
                }
                idx++;
            }
            res.YwpUserFriendStarRank = await DBService.Logic.DBService.GetYwpUserAsync<List<FriendRankEntry>>(deserialized.Level5UserID!, "ywp_user_friend_star_rank");
            var otherYwpUserFriendStarRank = await DBService.Logic.DBService.GetYwpUserAsync<List<FriendRankEntry>>(tgdkey, "ywp_user_friend_star_rank");
            res.YwpUserFriendRank = await DBService.Logic.DBService.GetYwpUserAsync<List<FriendRankEntry>>(deserialized.Level5UserID!, "ywp_user_friend_rank");
            var otherYwpUserFriendRank = await DBService.Logic.DBService.GetYwpUserAsync<List<FriendRankEntry>>(tgdkey, "ywp_user_friend_rank");
            var YwpUserFriendDictionaryRank = await DBService.Logic.DBService.GetYwpUserAsync<List<FriendRankEntry>>(deserialized.Level5UserID!, "ywp_user_friend_dictionary_rank");
            var otherYwpUserFriendDictionaryRank = await DBService.Logic.DBService.GetYwpUserAsync<List<FriendRankEntry>>(tgdkey, "ywp_user_friend_dictionary_rank");
            
            CreateUserRank(deserialized.TargetUserId!, otherYwpUserFriendStarRank!, res.YwpUserFriendStarRank!);
            CreateUserRank(deserialized.Level5UserID!, res.YwpUserFriendStarRank!, otherYwpUserFriendStarRank!);

            CreateUserRank(deserialized.TargetUserId!, otherYwpUserFriendRank!, res.YwpUserFriendRank!);
            CreateUserRank(deserialized.Level5UserID!, res.YwpUserFriendRank!, otherYwpUserFriendRank!);

            CreateUserRank(deserialized.TargetUserId!, otherYwpUserFriendDictionaryRank!, YwpUserFriendDictionaryRank!);
            CreateUserRank(deserialized.Level5UserID!, YwpUserFriendDictionaryRank!, otherYwpUserFriendDictionaryRank!);

            await DBService.Logic.DBService.SetYwpUserAsync(deserialized!.Level5UserID!, "ywp_user_friend_star_rank", res.YwpUserFriendStarRank!);
            await DBService.Logic.DBService.SetYwpUserAsync(tgdkey, "ywp_user_friend_star_rank", otherYwpUserFriendStarRank!);
           
            await DBService.Logic.DBService.SetYwpUserAsync(deserialized!.Level5UserID!, "ywp_user_friend_rank", res.YwpUserFriendRank!);
            await DBService.Logic.DBService.SetYwpUserAsync(tgdkey, "ywp_user_friend_rank", otherYwpUserFriendRank!);

            await DBService.Logic.DBService.SetYwpUserAsync(deserialized!.Level5UserID!, "ywp_user_friend_dictionary_rank", YwpUserFriendDictionaryRank!);
            await DBService.Logic.DBService.SetYwpUserAsync(tgdkey, "ywp_user_friend_dictionary_rank", otherYwpUserFriendDictionaryRank!);

            await DBService.Logic.DBService.SetYwpUserAsync(deserialized!.Level5UserID!, "ywp_user_friend_request_recv", res.YwpUserFriendRequestRecv);
            if (res.YwpUserFriend != null)
                await DBService.Logic.DBService.SetYwpUserAsync(deserialized!.Level5UserID!, "ywp_user_friend", res.YwpUserFriend);
            if (YwpUserFriendFriend != null)
                await DBService.Logic.DBService.SetYwpUserAsync(tgdkey, "ywp_user_friend", YwpUserFriendFriend);
            await DBService.Logic.DBService.SetYwpUserAsync(tgdkey, "ywp_user_friend_request_recv", YwpUserFriendRequestRecvFriend);
            var outDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(res));
            var outResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(outDict));
            await ctx.Response.WriteAsync(outResponse);
        }
    }
}
