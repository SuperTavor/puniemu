using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.GameStart.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
using System.Buffers;
using System.Runtime.CompilerServices;
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
            if (userData.Hitodama > 0) 
            {
                userData.Hitodama -= 1;
                var res = new GameStartResponse();

                var LevelData = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<string, object>>>(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["level_data"])[deserialized.StageId];
                // Get deck and user_youkai to get : userYoukaiList info
                string[] UserDeck = (await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_youkai_deck"))!.Split('|');
                var YwpUserYoukaiTab = new TableParser.Logic.TableParser((await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_youkai"))!);
                for (int i = 1; i < 1 + 5; i++)
                {
                    //get index of yokai info in general ywpuseryokai table
                    var yokaiInfoIndex = YwpUserYoukaiTab.FindIndex([UserDeck[i]]);
                    var item = new UserYoukaiItem()
                    {
                        YoukaiId = int.Parse(UserDeck[i]),
                        SkillLevel = int.Parse(YwpUserYoukaiTab.Table[yokaiInfoIndex][1]),
                        SSkillLevel = int.Parse(YwpUserYoukaiTab.Table[yokaiInfoIndex][2]),
                        Hp = int.Parse(YwpUserYoukaiTab.Table[yokaiInfoIndex][3]),
                        AttackPower = int.Parse(YwpUserYoukaiTab.Table[yokaiInfoIndex][4])
                    };
                    res.UserYoukaiList.Add(item);
                }

                // Determine if it's was first clear 
                var UserStage = new TableParser.Logic.TableParser((await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_stage"))!);
                var stageIdx = UserStage.FindIndex([deserialized.StageId.ToString()]);
                if(stageIdx == -1)
                {
                    await GeneralUtils.SendBadRequest(ctx);
                    return;
                }
                
                //user_stage status is true if completed, firstClear is the other way around
                if(int.Parse(UserStage.Table[stageIdx][1]) == 0)
                {
                    res.IsFirstClear = 1;
                }
                else
                {
                    res.IsFirstClear = 0;
                }

                //always seemingly OK on 0
                res.YoukaiHp = 0;

                // Enemy Info (we going to store some of these data for all stage)
                res.StageType = (long)LevelData["stageType"]; 
                res.EnemyYoukaiList = ((JArray)LevelData["enemyInfo"]).ToObject<List<EnemyYoukai>>();
                res.BattleType = (long)LevelData["battleType"];
                res.UserData = userData;
                var resdict = JsonConvert.DeserializeObject<Dictionary<string,object>>(JsonConvert.SerializeObject(res));
                await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Gdkey, "ywp_user_data", userData);
                await GeneralUtils.AddTablesToResponse(Consts.GAME_START_TABLES, resdict!, true, deserialized.Gdkey);
                var encryptedRes = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(resdict));
                ctx.Response.Headers.ContentType = "application/json";
                await ctx.Response.WriteAsync(encryptedRes);
            }
            else
            {
                var res = new MsgBoxResponse("You don't have enough spirit.", "Not Enough spirit");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));
            }


        }
    }
}
