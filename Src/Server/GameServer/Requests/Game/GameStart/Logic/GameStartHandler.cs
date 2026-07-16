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
using Puniemu.Src.UserDataManager.Logic;
using Puniemu.Src.Server.GameServer.Requests.GameEnd.DataClasses;
using Microsoft.AspNetCore.Mvc.Formatters;
namespace Puniemu.Src.Server.GameServer.Requests.Game.GameStart.Logic
{
    public static class GameStartHandler
    {
        static bool haveEnoughHitodama(ref YwpUserData userData)
        {
            bool moreOrEqualThan5 = ((userData.Hitodama + userData.FreeHitodama) >= 5);
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
                userData.FreeHitodama -= 1;
            }
            bool lessThan5 = ((userData.Hitodama + userData.FreeHitodama) < 5);
            if (moreOrEqualThan5 && lessThan5)
            {
                userData.HitodamaRecoverSec = 900;
            }
            return true;
        }


        static async Task<bool> isFirstClear(long stageId, string gdkey)
        {
            var UserStage = new TableParser<YwpUserStage>((await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(gdkey, "ywp_user_stage"))!);
            var stageIdx = UserStage.FindIndex([stageId.ToString()]);
            if (stageIdx == -1)
            {
                
                UserStage.AddItem(new YwpUserStage { StageId = stageId, StageStatus = 0, Star1 = 0, Star2 = 0, Star3 = 0, Score = 0, NumClear = 0, Unk2 = 0 });
                await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(gdkey, "ywp_user_stage", UserStage.ToString());
                return true;
            }
            else
            {
                if (UserStage.Items[stageIdx].StageStatus == 0)
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
            Console.WriteLine(deserialized.StageId);
            // send bad requests if bad requests send
            var userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized!.Gdkey!, "ywp_user_data");
            var UserStage = new TableParser.Logic.TableParser<YwpUserStage>((await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_stage"))!);
            
            if (deserialized == null || userData == null)
            {
                await GeneralUtils.SendBadRequest(ctx);
                return;
            }

            var stageInfoIdx = MasterStageData.StageItems.FindIndex(x => x.StageId == deserialized.StageId);
            //Use pass
            if (MasterStageData.StageItems[stageInfoIdx].UseActionType == 1)
            {
                var userItemRaw = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_item");
                var userItem = new TableParser<YwpUserItem>(userItemRaw);
                var passItemId = MasterStageData.StageItems[stageInfoIdx].UseActionID;
                var itemIdx = userItem.Items.FindIndex(x => x.ItemId == passItemId);
                //Check if have item
                if (itemIdx != -1 && userItem.Items[itemIdx].Count > 0)
                {
                    userItem.Items[itemIdx].Count--;
                    await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Gdkey, "ywp_user_item", userItem.ToString());
                }
                else
                {
                    var errRes = new MsgBoxResponse("You don't have the pass.", "Error");
                    await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errRes)));
                    return;
                }
            }
            else
            {
                if (!haveEnoughHitodama(ref userData))
                {
                    var errRes = new MsgBoxResponse("You don't have enough spirit.", "Not Enough spirit");
                    await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errRes)));
                    return;
                }
            }
            
            
            //Construct response
            var res = new GameStartResponse(userData);

            // Get mst and user table
            
            var enemyParams = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai_enemy_param"]!)!["tableData"]);
            
                
            var YwpUserYoukaiSkillTab = new TableParser.Logic.TableParser<YwpUserYoukaiSkill>((await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_youkai_skill"))!);
            var YwpUserYoukaiSSkillTab = new TableParser.Logic.TableParser((await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_youkai_strong_skill"))!);

            var mstYokai = new TableParser.Logic.TableParser<YwpMstYoukai>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai"]);
            var UserDeck = new TableParser.Logic.TableParser<YwpUserYoukaiDeck>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_youkai_deck"));
            var tutorialList = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<TutorialList>(deserialized!.Gdkey!, "ywp_user_tutorial_list");

            //get current stage info
            var jsonLevelData = JsonConvert.DeserializeObject<Dictionary<string, StageData>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["stage_data"]);
            

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
            var stageIdx = UserStage.FindIndex([deserialized.StageId.ToString()]);
            if (stageIdx == -1)
            {
                UserStage.AddItem(new YwpUserStage());
                await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_stage", UserStage.ToString());
                res.IsFirstClear = 1;
            }
            else
            {
                //user_stage status is true if completed, firstClear is the other way around
                if (UserStage.Items[stageIdx].StageStatus == 0)
                {
                    res.IsFirstClear = 1;
                }
                else
                {
                    res.IsFirstClear = 0;
                }
            }

            res.IsFirstClear = (await isFirstClear(deserialized.StageId, deserialized.Gdkey!)) ? 1 : 0;

            // Get deck and user_youkai to get : userYoukaiList info
            var YwpUserYoukaiTab = new TableParser<YwpUserYoukai>((await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_youkai"))!);

            var isSuperShrine = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<bool>(deserialized.Gdkey, "ywp_user_addition");
            // Create EnemyInfoList (Use an new (but imcomplete right now) format)

            bool isAfterJibanyan = tutorialList.GetStatus(1, 2) == 1;
            void AddEnemy(long enemyId, int isDefBefriend, int hp = -1, int atk = -1)
            {

                var enemyParamsIdx = enemyParams.FindIndex([enemyId.ToString()]);
                if (hp == -1) hp = int.Parse(enemyParams.Table[enemyParamsIdx][2]);
                if (atk == -1) atk = int.Parse(enemyParams.Table[enemyParamsIdx][3]);
                if (enemyParamsIdx != -1)
                {
                    var item = new EnemyYoukai()
                    {
                        Hp = hp,
                        AttackPower = atk,
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

                    var enemyMstYokai = mstYokai.Items.FirstOrDefault(x => x.YoukaiId == long.Parse(enemyParams.Table[enemyParamsIdx][1]));

                    bool befriendable = enemyMstYokai != null && enemyMstYokai.FoodType != 0;
                    
                    var yokaiId = int.Parse(enemyParams.Table[enemyParamsIdx][1]);
                    var skillIdx = YoukaiManager.GetYoukaiSkillIndex(YwpUserYoukaiSkillTab, enemyId);

                    //LB not implemented yet

                    bool isMaxSkill = false;
                    if (skillIdx != -1)
                    {
                        isMaxSkill = YwpUserYoukaiSkillTab.Items[skillIdx].Level >= 7;
                    }
                    var mstYokaiItem = mstYokai.Items.Where(x => x.YoukaiId == yokaiId).FirstOrDefault();
                    var t = YwpUserYoukaiTab.Items.FindIndex(x => x.YoukaiId == yokaiId);
                    var notHaveYokai = t == -1;

                    var autobefriend = isDefBefriend == 1 && notHaveYokai;
                    if (autobefriend)
                    {
                        var yokaiRank = mstYokaiItem.YoukaiRarity;
                        var befrienders = DeckManager.GetBefrienderSpots(UserDeck, mstYokaiItem, YwpUserYoukaiSkillTab);
                        item.LotYoukaiInfoList = LotYoukaiManager.GenerateLotYoukai(befrienders, yokaiRank, isSuperShrine, true);

                    }
                    else if (!(mstYokaiItem == null) && mstYokaiItem.YoukaiRarity != RarityType.RarityNone && befriendable && isAfterJibanyan && !isMaxSkill)
                    {
                        var yokaiRank = mstYokaiItem.YoukaiRarity;
                        var befrienders = DeckManager.GetBefrienderSpots(UserDeck, mstYokaiItem, YwpUserYoukaiSkillTab);
                        item.LotYoukaiInfoList = LotYoukaiManager.GenerateLotYoukai(befrienders, yokaiRank, isSuperShrine, false);
                        //Console.WriteLine(JsonConvert.SerializeObject(item.LotYoukaiInfoList));
                    }

                    res.EnemyYoukaiList.Add(item);
                }
            }
            foreach (EnemyStageEntry i in (LevelData.Enemy))
            {
                AddEnemy(i.EnemyId, i.DefaultBefriends);
            }

            //Check rare encounters and add, if there are already 3 yokai replace the weakest one
            var rareEnemyId = RareEnemyManager.GetDrop(deserialized.StageId);
            if(rareEnemyId != -1 && isAfterJibanyan)
            {
                //Rare encounter hp and attack are the average of all other stages
                int generalAttackAverage = (int)res.EnemyYoukaiList.Average(x => x.AttackPower);
                int generalHpAverage = (int)res.EnemyYoukaiList.Average(x => x.Hp);
                if (res.EnemyYoukaiList.Count == 3)
                {
                    float lowestStatAvg = Int32.MaxValue;
                    EnemyYoukai selectedItem = null;
                    foreach (var item in res.EnemyYoukaiList)
                    {
                        float statAvg = (item.AttackPower + item.Hp) / 2;
                        if (statAvg < lowestStatAvg)
                        {
                            selectedItem = item;
                            lowestStatAvg = statAvg;
                        }
                    }
                    res.EnemyYoukaiList.Remove(selectedItem);
                }

                AddEnemy(rareEnemyId, 0, generalHpAverage, generalAttackAverage);
            }
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Gdkey, "last_enemy", res.EnemyYoukaiList);
            void AddToUserYoukaiList(long youkaiId)
            {
                //get index of yokai info in general ywpuseryokai table
                var yokaiInfoIndex = YoukaiManager.GetYoukaiIndex(YwpUserYoukaiTab, youkaiId);
                var YwpUserYoukaiSkillIndex = YwpUserYoukaiSkillTab.FindIndex([youkaiId.ToString()]);
                var YwpUserYoukaiSSkillIndex = YwpUserYoukaiSSkillTab.FindIndex([youkaiId.ToString()]);
                var item = new UserYoukaiItem()
                {
                    YoukaiId = (int)YwpUserYoukaiTab.Items[yokaiInfoIndex].YoukaiId,
                    Hp = YwpUserYoukaiTab.Items[yokaiInfoIndex].Hp,
                    AttackPower = YwpUserYoukaiTab.Items[yokaiInfoIndex].Atk,
                };
                if (YwpUserYoukaiSSkillIndex != -1)
                    item.SSkillLevel = int.Parse(YwpUserYoukaiSSkillTab.Table[YwpUserYoukaiSSkillIndex][1]);
                if (YwpUserYoukaiSkillIndex != -1)
                    item.SkillLevel = YwpUserYoukaiSkillTab.Items[YwpUserYoukaiSkillIndex].Level;
                res.UserYoukaiList.Add(item);
            }

            var currentDeck = UserDeck.Items[0];
            AddToUserYoukaiList(currentDeck.MiddleYoukaiId);
            AddToUserYoukaiList(currentDeck.MiddleLeftYoukaiId);
            AddToUserYoukaiList(currentDeck.MiddleRightYoukaiId);
            AddToUserYoukaiList(currentDeck.FarLeftYoukaiId);
            AddToUserYoukaiList(currentDeck.FarRightYoukaiId);

            //Check tribe unity
            Dictionary<int,int> unities = new();
            foreach(var yokai in res.UserYoukaiList)
            {
                var mstItem = mstYokai.Items.FirstOrDefault(x => x.YoukaiId == yokai.YoukaiId);
                if (mstItem == null) continue;
                if(unities.ContainsKey(mstItem.YoukaiKind))
                {
                    unities[mstItem.YoukaiKind]++;
                }
                else
                {
                    unities.Add(mstItem.YoukaiKind, 1);
                }
                
            }

            //Apply tribe unity
            foreach (var yokai in res.UserYoukaiList)
            {
                var mstItem = mstYokai.Items.FirstOrDefault(x => x.YoukaiId == yokai.YoukaiId);
                if (mstItem == null) continue;
                var unitySize = unities[mstItem.YoukaiKind];
                /*
                 Unity bonuses without skills are as follows:
                10% for 2
                20% for 3
                25% for 4
                30% for 5
                note that different tribes ofc do different unities, so 2 brave 2 charming will be 2*10% boost 
                and they only apply on the yokai of said tribes
                 */

                int multiplier = unitySize switch
                {
                    2 => 10,
                    3 => 20,
                    4 => 25,
                    5 => 30,
                    _ => 0
                };
                if (multiplier > 0)
                {
                    Console.WriteLine("Applied tribe unity bonus for tribe " + mstItem.YoukaiKind + ": " + multiplier + "%.");
                    yokai.Hp += yokai.Hp * multiplier / 100;
                    yokai.AttackPower += yokai.AttackPower * multiplier / 100;
                }
               
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
            res.BattleType = deserialized.BattleType; //maybe, not sure
            //Console.WriteLine(deserialized.StageId);
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
