using Newtonsoft.Json;
using Puniemu.Src.DBService;

using Puniemu.Src.Server.GameServer.Requests.MapWarp.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;
using System.Buffers;
using Puniemu.Src.Server.GameServer.Logic;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Puniemu.Src.Utils.GeneralUtils;

namespace Puniemu.Src.Server.GameServer.Requests.MapWarp.Logic
{
    public class MapWarpHandler
    {
        //Puni puni now have a new profile system (with plate, effect and codename) but for some reason in the private server it's always the old app
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<MapWarpRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";

            var userData = await DBService.Logic.DBService.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID, "ywp_user_data");
            var userStage = await DBService.Logic.DBService.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_stage");
            var userMap = new TableParser.Logic.TableParser(await DBService.Logic.DBService.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_map"));


            var index = userMap.FindIndex([deserialized.MapId.ToString()]);
            if (index == -1)
            {
                var errSession = new MsgBoxResponse("Error", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                return;
            }

            /*index = userStage.FindIndex([deserialized.MapId.ToString() + "001"]);

            if (index == -1)
            {
                var errSession = new MsgBoxResponse("Error", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                return;
            }*/
            userData.CurrentStageID = int.Parse(deserialized.MapId.ToString() + "001");



            //event
            /*
            Modified 2001: 3|2001|0 -> 3|2001|1
            Modified 3023: 11|3023|0 -> 11|3023|1
            */





            GenerateFriendData.RefreshYwpUserFriend(deserialized.Level5UserID, -1, -1, userData!.PlayerName, -1, "");
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

            resdict!["ywp_user_stage"] = userStage.ToString();
            resdict!["ywp_user_map"] = userMap.ToString();
            await DBService.Logic.DBService.SetYwpUserAsync(deserialized!.Level5UserID!, "ywp_user_data", userData);

            await GeneralUtils.AddTablesToResponse(ywpKeys, resdict!, true, deserialized!.Level5UserID!);
            var marshalledResponse = JsonConvert.SerializeObject(resdict);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
        }
    }
}
