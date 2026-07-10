using Newtonsoft.Json;
using Puniemu.Src.UserDataManager;

using Puniemu.Src.Server.GameServer.Requests.MapWarp.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;
using System.Buffers;
using Puniemu.Src.Server.GameServer.Logic;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.TableParser.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.MapWarp.Logic
{
    public class MapWarpHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<MapWarpRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";

            if(deserialized.MapId == 1015)
            {
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(new MsgBoxResponse("Yopple Inc is under construction!", "Coming soon!"))));
                return;
            }
            if(!deserialized.MapId.ToString().StartsWith("1"))
            {
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(new MsgBoxResponse("The sewers are under construction", "Coming soon!"))));
                return;
            }
            var userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID, "ywp_user_data")!;
            var userStage = new TableParser.Logic.TableParser<YwpUserStage>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_stage")!);
            var userMap = new TableParser.Logic.TableParser<YwpUserMap>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_map")!);
            var mstMapObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_map"]);

            var mstMap = ((Newtonsoft.Json.Linq.JArray)mstMapObj["data"])
                .ToObject<List<YwpMstMap>>();
            var mapIdx = userMap.FindIndex([deserialized.MapId.ToString()]);
            if (mapIdx == -1)
            {
                MapManager.AddMap(userMap, deserialized.MapId);
                //var errSession = new MsgBoxResponse("Error", "Error");
                //await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                //return;
            }


            //Add stage if doesnt exist
            var stageId = long.Parse(deserialized.MapId.ToString() + "001");
            
            if (userStage.FindIndex([stageId.ToString()]) == -1)
            {
                //Stage status 2 = Need to be unlocked
                var map = mstMap.Where(x => x.MapId == deserialized.MapId).First();
                int status = 2;
                //needs no unlock
                if (map.NeedYmoney == 0 && map.NeedYoukaiId == 0 && map.NeedYoukaiLevel == 0 && map.NeedFriendPoint == 0)
                {
                    status = 0;
                }
                userStage.Items.Add(new YwpUserStage
                {
                    StageId = stageId,
                    StageStatus = status
                });
            }


            userData.CurrentStageID = int.Parse(deserialized.MapId.ToString() + "001");



            //event
            /*
            Modified 2001: 3|2001|0 -> 3|2001|1
            Modified 3023: 11|3023|0 -> 11|3023|1
            */





            await GenerateFriendData.RefreshYwpUserFriend(deserialized.Level5UserID, -1, -1, userData!.PlayerName, -1, "");
            var res = new MapWarpResponse(userData!);
            res.TeamEventButtonHiddenFlg = 1;
            var resdict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(res))!;

            // stocke la version corrigée dans resdict
            var ywpKeys = new List<string>
            {
                "ywp_mst_event_condition",
                "ywp_user_friend_stage",
                "ywp_user_treasure_series",
                "ywp_user_stage_rank",
                "ywp_user_tutorial_list",
                "ywp_user_raid_boss",
                "ywp_user_event_condition",
                "ywp_user_event",
                "ywp_user_mini_game_map_friend",
                "ywp_user_stage_relation_progress",
                "ywp_user_icon_budge",
                "ywp_user_steal_progress",
                "ywp_user_mini_game_map",
                "ywp_user_score_attack_reward",
                "ywp_user_event_tutorial",
                "ywp_user_event_ranking_reward",
                "ywp_mst_event"
            };
            List<Dictionary<string, object>> maps = new();
            var raw = DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_map"]!;
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(raw)!;
            var data = (JArray)dict["data"]!;
            var maps_list = data.ToObject<List<Dictionary<string, object>>>()!;

            foreach (Dictionary<string, object> map in maps_list)
            {
                if ((long)map["mapId"] == deserialized.MapId)
                {
                    maps.Add(map);
                }
            }
            if (maps.IsNullOrEmpty())
            {
                var errSession = new MsgBoxResponse("Error", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                return;
            }
            resdict["ywp_mst_map"] = maps;
            var compiledUserStage = userStage.ToString();
            resdict!["ywp_user_stage"] = compiledUserStage;
            resdict!["ywp_user_map"] = userMap.ToString();
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserID!, "ywp_user_data", userData);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserID!, "ywp_user_stage", compiledUserStage);
            await GeneralUtils.AddTablesToResponse(ywpKeys, resdict!, true, deserialized!.Level5UserID!);
            var marshalledResponse = JsonConvert.SerializeObject(resdict);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
        }
    }
}
