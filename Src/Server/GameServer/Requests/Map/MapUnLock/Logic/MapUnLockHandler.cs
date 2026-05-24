using Newtonsoft.Json;
using Puniemu.Src.UserDataManager;

using Puniemu.Src.Server.GameServer.Requests.MapUnLock.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;
using System.Buffers;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.UserDataManager.Logic;

namespace Puniemu.Src.Server.GameServer.Requests.MapUnLock.Logic
{
    public class MapUnLockHandler
    {
        //Puni puni now have a new profile system (with plate, effect and codename) but for some reason in the private server it's always the old app
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<MapUnLockRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";
            var UserTables = await UserDataManager.Logic.UserDataManager.GetEntireUserData(deserialized!.Level5UserID!);

            var userData = UserDataManager.Logic.UserDataManager.GetYwpUserFromJson<YwpUserData>("ywp_user_data", UserTables)!;
            var userStage = new TableParser<YwpUserStage>(UserDataManager.Logic.UserDataManager.GetYwpUserFromJson<string>("ywp_user_stage", UserTables)!);
            var userMap = new TableParser<YwpUserMap>(UserDataManager.Logic.UserDataManager.GetYwpUserFromJson<string>("ywp_user_map", UserTables)!);

            List<YwpMstMap> mstMap = JsonConvert.DeserializeObject<List<YwpMstMap>>(
                JsonConvert.DeserializeObject<Dictionary<string, object>>(
                    DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_map"]
                )!["data"].ToString()!
            )!;

            var userMapIndex = MapManager.GetMapIndex(userMap, deserialized.MapId);
            var mstMapIndex = MstMapManager.GetMapIndex(mstMap, deserialized.MapId);
            var userStageIndex = StageManager.GetStageIndex(userStage, (((int)deserialized.MapId * 1000) + 1));
            if (userMapIndex == -1 || mstMapIndex == -1)
            {
                var errSession = new MsgBoxResponse("Map don't exist or unknown", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                return;
            }

            if (mstMap[mstMapIndex].TextUnlock.IsNullOrEmpty())
            {
                var errSession = new MsgBoxResponse("This map is not unlockable", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                return;
            }
            if (mstMap[mstMapIndex].NeedYmoney > userData.YMoney)
            {
                var errSession = new MsgBoxResponse("You don't have enough Y-money", "Not enough Y-money");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                return;
            }
            userData.YMoney -= (int)mstMap[mstMapIndex].NeedYmoney;
            //unlcok stage
            if(userStageIndex == -1)
            {
                var errSession = new MsgBoxResponse("You don't have enough Y-money", "Not enough Y-money");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                return;
            }
            GenerateFriendData.RefreshYwpUserFriend(deserialized.Level5UserID, -1, -1, userData!.PlayerName, -1, "");
            if(userStageIndex == -1)
            {
                var errSession = new MsgBoxResponse("Stage not found in userStage", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                return;
            }
            else
            {
                userStage.Items[userStageIndex].StageStatus = 0;
            }
            userMap.Items[userMapIndex].IsUnlocked = 1;
            var compiledUserStage = userStage.ToString();
            var compiledUserMap = userMap.ToString();
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserID!, "ywp_user_data", userData);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserID!, "ywp_user_stage", compiledUserStage);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_map", compiledUserMap);
            var res = new MapUnLockResponse(userData!);
            var resdict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(res))!;
            resdict["ywp_user_map"] = compiledUserMap;
            resdict["ywp_user_stage"] = compiledUserStage;
            var marshalledResponse = JsonConvert.SerializeObject(resdict);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
        }
    }
}
