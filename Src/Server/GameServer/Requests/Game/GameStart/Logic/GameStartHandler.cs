using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.GameStart.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.TableParser.Logic;
using Puniemu.Src.TableParser.DataClasses;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using Puniemu.Src.Server.GameServer.Logic;
namespace Puniemu.Src.Server.GameServer.Requests.Game.GameStart.Logic
{
    public static class GameStartHandler
    {
        static bool haveEnoughHitodama(ref YwpUserData userData)
        {
            if (userData.Hitodama <= 0 && userData.FreeHitodama <= 0)
            {
                return false;
            }
            if (userData.Hitodama > 0)
            {
                userData.Hitodama -= 1;
            }
            else
            {
                if (userData.FreeHitodama == 5)
                {
                    userData.HitodamaRecoverSec = 900;
                }
                userData.FreeHitodama -= 1;
            }
            return true;
        }

        static async Task<bool> isFirstClear(long stageId, string gdkey)
        {
            var UserStage = new TableParser<YwpUserStage>((await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(gdkey, "ywp_user_stage"))!);
            var stageIdx = UserStage.FindIndex([stageId.ToString()]);
            if (stageIdx == -1)
            {
                
                UserStage.AddItem(new YwpUserStage { StageId = stageId, IsClear = 0, Star1 = 0, Star2 = 0, Star3 = 0, Score = 0, NumClear = 0, Unk2 = 0 });
                await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(gdkey, "ywp_user_stage", UserStage.ToString());
                return true;
            }
            else
            {
                if (UserStage.Items[stageIdx].IsClear == 0)
                {
                    return true;
                }
            }
            return false;
        }


        public static async Task HandleAsync(HttpContext ctx)
        {
            // WIP : the code is rather messy because this .nhn is incomplete and still under development

            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<GameStartRequest>(requestJsonString!);

            // send bad requests if bad requests send
            var userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized!.Gdkey!, "ywp_user_data");
            
            if (deserialized == null || userData == null)
            {
                await GeneralUtils.SendBadRequest(ctx);
                return;
            }
            
            if (!haveEnoughHitodama(ref userData))
            {
                var errRes = new MsgBoxResponse("You don't have enough spirit.", "Not Enough spirit");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errRes)));
                return;
            }
            
            //Construct response
            var res = new GameStartResponse(userData);

            // Get mst and user table
            
            var enemyParams = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai_enemy_param"]!)!["tableData"]);
            
                
            var YwpUserYoukaiSkillTab = new TableParser.Logic.TableParser((await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_youkai_skill"))!);
            var YwpUserYoukaiSSkillTab = new TableParser.Logic.TableParser((await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_youkai_strong_skill"))!);


            var UserDeck = new TableParser.Logic.TableParser(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_youkai_deck"));
            var tutorialList = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<TutorialList>(deserialized!.Gdkey!, "ywp_user_tutorial_list");

            //get current stage info
            var jsonLevelData = JsonConvert.DeserializeObject<Dictionary<string, StageData>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["stage_data"]);
            var stageInfoIdx = MasterStageData.StageItems.FindIndex(x => x.StageId == deserialized.StageId);

            // Throw error if stage dosent have config info
            if (stageInfoIdx == -1 || (jsonLevelData == null || !(jsonLevelData.ContainsKey(deserialized.StageId.ToString()))))
            {
                var err = new MsgBoxResponse("Puniemu dosent have\nconfig data for this stageId : " + deserialized.StageId.ToString(), "Missing stageId");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(err)));
                return;
            }
            StageData LevelData = jsonLevelData[deserialized.StageId.ToString()];
                
            // Throw error if enemy list is empty
            if (LevelData.Enemy == null || LevelData.Enemy.Count == 0)
            {
                await GeneralUtils.SendBadRequest(ctx);
                return;
            }

            // Determine if it's was first clear 
            var UserStage = new TableParser.Logic.TableParser((await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_stage"))!);
            var stageIdx = UserStage.FindIndex([deserialized.StageId.ToString()]);
            if (stageIdx == -1)
            {
                UserStage.AddRow([deserialized.StageId.ToString(), "0", "0", "0", "0", "0", "0", "0"]);
                await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_stage", UserStage.ToString());
                res.IsFirstClear = 1;
            }
            else
            {
                //user_stage status is true if completed, firstClear is the other way around
                if (int.Parse(UserStage.Table[stageIdx][1]) == 0)
                {
                    res.IsFirstClear = 1;
                }
                else
                {
                    res.IsFirstClear = 0;
                }
            }

            res.IsFirstClear = (await isFirstClear(deserialized.StageId, deserialized.Gdkey!)) ? 1 : 0;

            // Create EnemyInfoList (Use an new (but imcomplete right now) format)
            foreach (EnemyStageEntry i in (LevelData.Enemy))
            {
                long enemyId = i.EnemyId;
                var enemyParamsIdx = enemyParams.FindIndex([enemyId.ToString()]);
                if (enemyParamsIdx != -1)
                {
                    var item = new EnemyYoukai()
                    {
                        Hp = int.Parse(enemyParams.Table[enemyParamsIdx][2]),
                        AttackPower = int.Parse(enemyParams.Table[enemyParamsIdx][3]),
                        ActionTurn = int.Parse(enemyParams.Table[enemyParamsIdx][4]),
                        DropItemCount = 0, //todo
                        DropItemID = 0, //todo
                        DropItemType = 0, //todo
                        EnemyID = enemyId,
                        InvalidFoodFlg = 0, //todo
                        ReplaceYokaiID = 0, //todo
                        LotTreasureInfoList = new(), //todo
                        LotTreasureFlag = 0, //todo
                        LotItemInfoList = "0000", //todo
                        EnableFoodInfoList = new(), //todo
                    };
                    if (i.DefaultBefriends == 1 && res.IsFirstClear == 1)
                    {
                        item.LotYoukaiInfoList.Entries.Add(new LotYoukaiInfo { LotPattern = "00000", LotResult = "1111" });
                    }
                    else
                    {
                        item.LotYoukaiInfoList.Entries.Add(new LotYoukaiInfo { LotPattern = "00000", LotResult = "0000" }); //todo
                    }
                    res.EnemyYoukaiList.Add(item);
                }
            }

            // Get deck and user_youkai to get : userYoukaiList info
            var YwpUserYoukaiTab = new TableParser<YwpUserYoukai>((await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_youkai"))!);
            for (int i = 1; i < 1 + 5; i++)
            {
                //get index of yokai info in general ywpuseryokai table
                var yokaiInfoIndex = YoukaiManager.GetYoukaiIndex(ref YwpUserYoukaiTab, int.Parse(UserDeck.Table[0][i]));
                var YwpUserYoukaiSkillIndex = YwpUserYoukaiSkillTab.FindIndex([UserDeck.Table[0][i]]);
                var YwpUserYoukaiSSkillIndex = YwpUserYoukaiSSkillTab.FindIndex([UserDeck.Table[0][i]]);
                var item = new UserYoukaiItem()
                {
                    YoukaiId = (int)YwpUserYoukaiTab.Items[yokaiInfoIndex].YoukaiId,
                    Hp = YwpUserYoukaiTab.Items[yokaiInfoIndex].Hp,
                    AttackPower = YwpUserYoukaiTab.Items[yokaiInfoIndex].Atk,
                };
                if (YwpUserYoukaiSSkillIndex != -1)
                    item.SSkillLevel = int.Parse(YwpUserYoukaiSSkillTab.Table[YwpUserYoukaiSSkillIndex][1]);
                if (YwpUserYoukaiSkillIndex != -1)
                    item.SkillLevel = int.Parse(YwpUserYoukaiSkillTab.Table[YwpUserYoukaiSkillIndex][1]);
                res.UserYoukaiList.Add(item);
            }


            // Edit tutorial flg
            if (LevelData.TutorialEdit != null && LevelData.TutorialEdit.TutorialReq != null)
            {
                foreach (TutorialEntry item in LevelData.TutorialEdit.TutorialReq)
                {
                    if (item.FirstClear == 0 || (item.FirstClear == 1 && res.IsFirstClear == 1))
                    {
                        var index = tutorialList.GetTutorialFlgIndex(item.TutorialId, item.TutorialType);
                        if (index == -1)
                        {
                            tutorialList.Entries.Add(item);
                        }
                        //Set the tutorial status
                        tutorialList.EditTutorialFlg(item.TutorialType, item.TutorialId, item.TutorialStatus);
                    }
                }
            }
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_tutorial_list", tutorialList);

            //always seemingly OK on 0
            res.YoukaiHp = 0;

            res.StageType = MasterStageData.StageItems[stageInfoIdx].StageType; //maybe, not sure
            res.BattleType = MasterStageData.StageItems[stageInfoIdx].BattleType; //maybe, not sure


            // save userdata and send response
            res.RequestID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            var dictionary = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_dictionary");
            var dictionaryTable = new TableParser.Logic.TableParser(dictionary!);
            res.DictionaryDiff = dictionaryTable.ToString();
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_requestid", res.RequestID);
            res.UserData = userData;
            var resdict = JsonConvert.DeserializeObject<Dictionary<string,object?>>(JsonConvert.SerializeObject(res));
            
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_data", userData);
            
            await GeneralUtils.AddTablesToResponse(Consts.GAME_START_TABLES, resdict!, true, deserialized!.Gdkey!);
            
            var encryptedRes = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(resdict));
            
            ctx.Response.Headers.ContentType = "application/json";
            
            await ctx.Response.WriteAsync(encryptedRes);

        }
    }
}
