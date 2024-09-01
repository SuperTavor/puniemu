using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.GameStart.DataClasses;
using System.Buffers;
using System.Text;
namespace Puniemu.Src.Server.GameServer.Requests.GameStart.Logic
{
    public static class GameStartHandler
    {

        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<GameStartRequest>(requestJsonString!);


            //Construct response
            var userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Gdkey, "ywp_user_data");
            if (userData.Hitodama > 0 || userData.FreeHitodama > 0) {
                if (userData.FreeHitodama > 0)
                {
                    userData.FreeHitodama -= 1;
                } else
                {
                    userData.Hitodama -= 1;
                }
                var res = new GameStartResponse();

                var resdict = await res.ToDictionary();
                var LevelData = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<string, object>>>(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["level_data"])[deserialized.StageId];
                // Get deck and user_youkai to get : userYoukaiList info
                string[] UserDeck = ((await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_youkai_deck"))!).Split('|');
                var YwpUserYoukaiTab = new TableParser.Logic.TableParser((await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_youkai"))!);
                List<object> FinalUserYoukai = new List<object>();
                for (int i = 1; i < 1 + 5; i++)
                {
                    Dictionary<string, object> tempDict = new Dictionary<string, object>();
                    var index = YwpUserYoukaiTab.FindIndex([UserDeck[i]]);
                    tempDict["youkaiId"] = int.Parse(UserDeck[i]);
                    tempDict["skillLv"] = int.Parse(YwpUserYoukaiTab.Table[index][1]);
                    tempDict["sSkillLv"] = int.Parse(YwpUserYoukaiTab.Table[index][2]); // I'm not sure for this
                    tempDict["hp"] = int.Parse(YwpUserYoukaiTab.Table[index][3]);
                    tempDict["atkPower"] = int.Parse(YwpUserYoukaiTab.Table[index][4]);
                    FinalUserYoukai.Add(tempDict);
                }
                resdict["userYoukaiList"] = FinalUserYoukai;

                // Determine if it's was first clear 
                var UserStage = new TableParser.Logic.TableParser((await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_stage"))!);
                var stageIdx = UserStage.FindIndex([deserialized.StageId.ToString()]);
                if (int.Parse(UserStage.Table[stageIdx][1]) == 0)
                {
                    resdict["firstClearItemFlg"] = 1;
                }
                else
                {
                    resdict["firstClearItemFlg"] = 0;
                }


                // Idk or need to be stored (like ywp_mst)
                resdict["themeList"] = new List<object>();
                resdict["scoreLogSendFlg"] = 0;
                resdict["themeScoreCoef"] = "";
                resdict["chanceAddRateEventBlock"] = 0;
                resdict["addHPByWatchEffect"] = 0;
                resdict["eventPointUpItemId"] = 0;
                
                resdict["youkaiHP"] = 0;
                resdict["responseCodeTeamEvent"] = 0;
                // Maybe need to inspect ywp_user_stage

                // Enemy Info (we going to store some of these data for all stage)
                resdict["stageType"] = LevelData["stageType"]; ;
                resdict["enemyYoukaiList"] = LevelData["enemyInfo"];
                resdict["battleType"] = LevelData["battleType"];



                resdict["eventPointMaterial"] = "";
                resdict["addHPByGokuEffect"] = 0;
                resdict["eventFlg"] = 0;
                resdict["addAtkByGokuEffect"] = 0;
                resdict["eventStatus"] = 0;
                resdict["requestId"] = "1725134219559";

                // This data can be different even if it's the same StageId
                resdict["itemDropMaxCnt"] = 2;
                resdict["ywp_user_data"] = userData;
                await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Gdkey, "ywp_user_data", userData);
                foreach (var table in Consts.GAME_START_TABLES)
                {
                    string? tableText = null!;
                    if (table.StartsWith("ywp_user"))
                    {
                        var gotten = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<object>(deserialized.Gdkey, table);
                        tableText = JsonConvert.SerializeObject(gotten);
                    }
                    else tableText = ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache[table];
                    //if we can't deserialize json it means it's not a json and we store it as is
                    object tableObj = new();
                    try
                    {
                        tableObj = JsonConvert.DeserializeObject<object>(tableText);
                        if (tableObj is JObject)
                        {
                            var jObject_temp = (JObject)tableObj;
                            if (jObject_temp.ContainsKey("data"))
                            {
                                tableObj = jObject_temp["data"];
                            }
                            else if (jObject_temp.ContainsKey("tableData"))
                            {
                                tableObj = jObject_temp["tableData"];
                            }

                        }

                    }
                    catch
                    {
                        tableObj = tableText;
                    }
                    if (tableObj == null)
                    {
                        tableObj = new List<object>();
                    }
                    resdict[table] = tableObj;

                }
                var encryptedRes = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(resdict));
                ctx.Response.Headers.ContentType = "application/json";
                await ctx.Response.WriteAsync(encryptedRes);
            }
            else
            {
                var res = new MsgBoxResponse("You don't have enough Spirit !", "Not Enough Spirit");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));
            }


        }
    }
}
