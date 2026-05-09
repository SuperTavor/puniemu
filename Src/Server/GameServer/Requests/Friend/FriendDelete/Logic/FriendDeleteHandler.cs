using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Buffers;
using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.FriendRequestAccept.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.FriendRequestAccept.Logic;
using Puniemu.Src.Server.GameServer.Requests.FriendDelete.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.FriendDelete.Logic
{
    public class FriendDeleteHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);

            // Décryptage comme dans les autres handlers
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<FriendDeleteRequest>(requestJsonString!);

            var res = new FriendDeleteResponse { ResponseCode = 1 }; // défaut = échec

            if (deserialized == null || string.IsNullOrEmpty(deserialized.Level5UserID) || string.IsNullOrEmpty(deserialized.TargetUserId))
            {
                // Requête invalide
                var outErr = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res));
                await ctx.Response.WriteAsync(outErr);
                return;
            }

            // Récupère mes données (demandeur)
            var myData = await DBService.Logic.DBService.GetYwpUserAsync<YwpUserData>(
                deserialized.Level5UserID, "ywp_user_data"
            );

            if (myData == null || string.IsNullOrEmpty(myData.UserID))
            {
                // Impossible de retrouver le demandeur
                res.ResponseCode = 1;
                var outErr = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res));
                await ctx.Response.WriteAsync(outErr);
                return;
            }

            string myUserId = myData.UserID;
            string myGdkey = deserialized.Level5UserID;
            string targetUserId = deserialized.TargetUserId;

            // Récupère le gdkey du joueur ciblé
            string? targetGdkey = null;
            try
            {
                targetGdkey = await DBService.Logic.DBService.GetGdkeyFromUserId(targetUserId);
            }
            catch
            {
                targetGdkey = null;
            }

            // --- Suppression côté demandeur ---
            var myFriends = await DBService.Logic.DBService.GetYwpUserAsync<List<FriendEntry>>(
                myGdkey, "ywp_user_friend"
            ) ?? new List<FriendEntry>();

            // retire targetUserId de ma liste d'amis
            myFriends.RemoveAll(f => f != null && f.UserId == targetUserId);

            await DBService.Logic.DBService.SetYwpUserAsync(myGdkey, "ywp_user_friend", myFriends);

            // Supprime aussi dans mes autres tables (rank / star_rank / request_recv)
            await RemoveFriendFromAllTables(myGdkey, targetUserId);

            // --- Suppression côté cible (si on a son gdkey) ---
            if (!string.IsNullOrEmpty(targetGdkey))
            {
                var targetFriends = await DBService.Logic.DBService.GetYwpUserAsync<List<FriendEntry>>(
                    targetGdkey, "ywp_user_friend"
                ) ?? new List<FriendEntry>();

                // retire mon UserID de sa liste d'amis (ATTENTION : on compare avec myData.UserID)
                targetFriends.RemoveAll(f => f != null && f.UserId == myUserId);

                await DBService.Logic.DBService.SetYwpUserAsync(targetGdkey, "ywp_user_friend", targetFriends);

                // Supprime aussi dans ses autres tables
                await RemoveFriendFromAllTables(targetGdkey, myUserId);
            }

            // Recharger les tables mises à jour côté demandeur (pour la réponse)
            res.YwpUserData = await DBService.Logic.DBService.GetYwpUserAsync<YwpUserData>(myGdkey, "ywp_user_data");
            res.YwpUserFriend = await DBService.Logic.DBService.GetYwpUserAsync<List<FriendEntry>>(myGdkey, "ywp_user_friend") ?? new List<FriendEntry>();
            res.YwpUserFriendRank = await DBService.Logic.DBService.GetYwpUserAsync<List<FriendEntry>>(myGdkey, "ywp_user_friend_rank") ?? new List<FriendEntry>();
            res.YwpUserFriendStarRank = await DBService.Logic.DBService.GetYwpUserAsync<List<FriendRankEntry>>(myGdkey, "ywp_user_friend_star_rank") ?? new List<FriendRankEntry>();
            res.YwpUserFriendRequestRecv = await DBService.Logic.DBService.GetYwpUserAsync<List<FriendRequestEntry>>(myGdkey, "ywp_user_friend_request_recv") ?? new List<FriendRequestEntry>();

            res.ResponseCode = 0;

            var outDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(res));
            var outResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(outDict));
            await ctx.Response.WriteAsync(outResponse);
        }

        private static async Task RemoveFriendFromAllTables(string gdkey, string removedUserId)
        {
            // ywp_user_friend (FriendEntry)
            var friends = await DBService.Logic.DBService.GetYwpUserAsync<List<FriendEntry>>(gdkey, "ywp_user_friend") ?? new List<FriendEntry>();
            friends.RemoveAll(f => f != null && f.UserId == removedUserId);
            await DBService.Logic.DBService.SetYwpUserAsync(gdkey, "ywp_user_friend", friends);

            // ywp_user_friend_rank (FriendEntry)
            var rank = await DBService.Logic.DBService.GetYwpUserAsync<List<FriendEntry>>(gdkey, "ywp_user_friend_rank") ?? new List<FriendEntry>();
            rank.RemoveAll(f => f != null && f.UserId == removedUserId);
            await DBService.Logic.DBService.SetYwpUserAsync(gdkey, "ywp_user_friend_rank", rank);

            // ywp_user_friend_star_rank (FriendStarRankEntry from FriendRequestAccept.Logic)
            var starRank = await DBService.Logic.DBService.GetYwpUserAsync<List<FriendRankEntry>>(gdkey, "ywp_user_friend_star_rank") ?? new List<FriendRankEntry>();
            starRank.RemoveAll(f => f != null && f.UserId == removedUserId);
            await DBService.Logic.DBService.SetYwpUserAsync(gdkey, "ywp_user_friend_star_rank", starRank);

            // ywp_user_friend_request_recv (FriendRequestEntry)
            var recv = await DBService.Logic.DBService.GetYwpUserAsync<List<FriendRequestEntry>>(gdkey, "ywp_user_friend_request_recv") ?? new List<FriendRequestEntry>();
            recv.RemoveAll(f => f != null && f.UserId == removedUserId);
            await DBService.Logic.DBService.SetYwpUserAsync(gdkey, "ywp_user_friend_request_recv", recv);

            // Note: on ne supprime pas l'objet ywp_user_data lui-même.
        }
    }
}
