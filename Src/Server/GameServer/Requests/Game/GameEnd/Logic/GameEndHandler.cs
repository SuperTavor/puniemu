using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Requests.GameEnd.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using Puniemu.Src.Utils.GeneralUtils;
using System.Buffers;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using UDM = Puniemu.Src.UserDataManager.Logic.UserDataManager;

namespace Puniemu.Src.Server.GameServer.Requests.GameEnd.Logic
{
    public static class GameEndHandler
    {
        public static void HandleUserYoukai(GameEndRequest deserialized, GameEndResponse res, TableParser<YwpUserYoukai> userYoukaiTable, TableParser<YwpUserYoukai> userYoukaiTableDiff)
        {
            var YoukaiMstTable = new TableParser<YwpMstYoukai>(
                JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai"]!
                    )!["tableData"]
            );
            var YoukaiLevelMstTable = new TableParser<YwpMstYoukaiLevel>(
                JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai_level"]!
                    )!["tableData"]
            );
            var YoukaiLevelOpenMstTable = new TableParser<YwpMstYoukaiLevelOpen>(
                JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai_level_open"]!
                    )!["tableData"]
            );

            foreach (UserYoukaiResultListReq i in deserialized!.UserYoukaiResultList!)
            {
                var youkaiIDx = userYoukaiTable.FindIndex([i.YoukaiId.ToString()]);
                var YoukaiMstIndex = MstYoukaiManager.GetYoukaiIndex(YoukaiMstTable, i.YoukaiId);
                if (youkaiIDx < 0 || YoukaiMstIndex < 0)
                {
                    continue;
                }

                UserYoukaiResultListRes item = new(userYoukaiTable.Items[youkaiIDx], YoukaiMstTable.Items[YoukaiMstIndex]);
                MoneyExpManager.GiveYoukaiExp(item, userYoukaiTable.Items[youkaiIDx], i.YoukaiId, res.UserGameResultData.Exp, YoukaiMstTable.Items[YoukaiMstIndex]);
                YwpUserYoukai yokai = userYoukaiTable.Items[youkaiIDx];
                userYoukaiTableDiff.AddItem(yokai);
                res.UserYoukaiResultList.Add(item);
            }
        }
        public static void HandleStage(GameEndRequest deserialized, GameEndResponse res, ref int FirstClear, TableParser<YwpUserStage> ywpUserStage, TableParser<YwpUserMap> ywpUserMap)
        {
            List<YwpMstMap> ywpMstMap = JsonConvert.DeserializeObject<List<YwpMstMap>>(
                JsonConvert.DeserializeObject<Dictionary<string, object>>(
                    DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_map"]
                )!["data"].ToString()!
            )!;
           
            var stageIndex = StageManager.GetStageIndex(ywpUserStage, deserialized.StageId);
            // create stage entry if it diden't exist yet
            if (stageIndex == -1)
            {
                FirstClear = 1;
                StageManager.AddStage(ywpUserStage, deserialized.StageId);
            }
            // check if it's first clear
            if (ywpUserStage.Items[stageIndex].StageStatus == 0)
            {
                FirstClear = 1;
            }
            // check if it's a new record
            res.UserGameResultData.PrevScore = (int)ywpUserStage.Items[stageIndex].Score;
            if (deserialized.Score > res.UserGameResultData.PrevScore)
            {
                res.UserGameResultData.ScoreUpdateFlg = 1;
            }

            int conditionCount = 1;

            while (true)
            {
                // used to differentiate locked levels from one same map
                int secretStageSkipp = 0;
                // compute the conditionId from stageId and conditionCount
                int tempConditionId = (deserialized.StageId * 10) + conditionCount;
                // get the array index of the computed conditionId in the table
                int tempIndex = MasterStageData.GetStageConditionIndex(tempConditionId);
                // if didn't exist we break cause this means we finished all conditions
                if (tempIndex == -1)
                {
                    break;
                }
                // we get the condition parameter
                long param1 = MasterStageData.ConditionItems[tempIndex].ConditionVal1;
                bool good = ConditionManager.ComputeStageCondition(MasterStageData.ConditionItems[tempIndex].ConditionType, deserialized, MasterStageData.ConditionItems[tempIndex].ConditionVal1, MasterStageData.ConditionItems[tempIndex].ConditionVal2, MasterStageData.ConditionItems[tempIndex].ConditionVal3);
                Console.WriteLine(tempConditionId.ToString() + " | " + good.ToString() + " | " + MasterStageData.ConditionItems[tempIndex].ConditionType.ToString());

                // 1-3 are generally the condition for stars, 4+ are generally locked levels condition
                if (conditionCount == 1)
                {
                    res.UserGameResultData.StarGetFlg1 = (good ? 1 : 0);
                }
                else if (conditionCount == 2)
                {
                    res.UserGameResultData.StarGetFlg2 = (good ? 1 : 0);
                }
                else if (conditionCount == 3)
                {
                    res.UserGameResultData.StarGetFlg3 = (good ? 1 : 0);
                }
                else if (conditionCount >= 4 && (good ? 1 : 0) == 1)
                {
                    // will store the unlocked stageId
                    long newAddedStage = -1;
                    bool isFinalStageMap = MasterStageData.GetNextStage(deserialized.StageId) == -1;
                    Console.WriteLine(MasterStageData.GetNextStage(deserialized.StageId).ToString());
                    var MapIndex = MstMapManager.GetMapIndex(ywpMstMap, (int)Math.Floor(deserialized.StageId / 1000.0));
                    if (isFinalStageMap && ywpMstMap[MapIndex].ReverseMapId != 0)
                    {
                        var MapIndex2 = MstMapManager.GetMapIndex(ywpMstMap, ywpMstMap[MapIndex].ReverseMapId);
                        if (MapIndex2 != -1)
                        {
                            int newMapIndex = MapManager.GetMapIndex(ywpUserMap, ywpMstMap[MapIndex2].MapId);
                            if (newMapIndex == -1)
                            {
                                MapManager.AddMap(ywpUserMap, ywpMstMap[MapIndex2].MapId);
                            }
                            MapManager.UpdateMap(ywpUserMap, ywpMstMap[MapIndex2].MapId, 1);

                            int newStageIdIndex = StageManager.GetStageIndex(ywpUserStage, (ywpMstMap[MapIndex2].MapId * 1000) + 1);
                            if (newStageIdIndex == -1)
                            {
                                StageManager.AddStage(ywpUserStage, (ywpMstMap[MapIndex2].MapId * 1000) + 1);
                                newAddedStage = (ywpMstMap[MapIndex2].MapId * 1000) + 1;
                            }
                        }
                    }
                    else
                    {
                        long newStageId = MasterStageData.GetUnlockedSecretStage(deserialized.StageId, secretStageSkipp);
                        if (newStageId != -1)
                        {
                            int newStageIdIndex = StageManager.GetStageIndex(ywpUserStage, newStageId);
                            if (newStageIdIndex == -1)
                            {
                                StageManager.AddStage(ywpUserStage, newStageId);
                                newAddedStage = newStageId;
                            }
                        }
                    }
                    if (newAddedStage != -1)
                    {
                        var secretStageItem = new LockedStageResultList()
                        {
                            StageId = newAddedStage,
                            Title = MasterStageData.ConditionItems[tempIndex].OpenStageIdList,
                            ConditionType = (int)MasterStageData.ConditionItems[tempIndex].ConditionType,
                            Description = MasterStageData.ConditionItems[tempIndex].Description,
                            OriginStageId = 0,
                        };
                        res.LockedStageResultList.Add(secretStageItem);
                    }
                    secretStageSkipp++;
                }
                conditionCount++;
            }

            StageManager.EditStage(ywpUserStage, deserialized.StageId, 1, deserialized.Score, res.UserGameResultData.StarGetFlg1, res.UserGameResultData.StarGetFlg2, res.UserGameResultData.StarGetFlg3, ywpUserStage.Items[stageIndex].NumClear + 1);


            // beta might only work for few maps
            bool mapLocked = false;
            long nextStage = MasterStageData.GetNextStage(deserialized.StageId);

            void UnlockMap(long mapId)
            {
                int mstIndex = MstMapManager.GetMapIndex(ywpMstMap, mapId);
                if (mstIndex == -1) return;

                int newMapIndex = MapManager.GetMapIndex(ywpUserMap, ywpMstMap[mstIndex].MapId);

                if (newMapIndex == -1)
                {
                    mapLocked = !ywpMstMap[mstIndex].TextUnlock.IsNullOrEmpty();
                    MapManager.AddMap(ywpUserMap, ywpMstMap[mstIndex].MapId);
                }

                MapManager.UpdateMap(ywpUserMap, ywpMstMap[mstIndex].MapId, 1);

                nextStage = (ywpMstMap[mstIndex].MapId * 1000) + 1;
            }
            if (nextStage == -1) // new map
            {
                var ogMapIndex = MstMapManager.GetMapIndex(ywpMstMap, (int)Math.Floor(deserialized.StageId / 1000.0));

                if (ogMapIndex != -1)
                {
                    var ogMap = ywpMstMap[ogMapIndex];

                    if (ogMap.NextMapId != 0)
                        UnlockMap(ogMap.NextMapId);

                    if (ogMap.ExtraMapId != 0)
                        UnlockMap(ogMap.ExtraMapId);
                }
            }
            if (nextStage != -1 && StageManager.GetStageIndex(ywpUserStage, nextStage) == -1)
            {
                if (mapLocked == false)
                {
                    StageManager.AddStage(ywpUserStage, nextStage);
                }
                res.LockedStageResultList.Add(new LockedStageResultList()
                {
                    StageId = nextStage,
                    Title = "",
                    ConditionType = 0,
                    Description = "",
                    OriginStageId = 0,
                });
            }
        }
        public static void HandleTutorial(GameEndRequest deserialized, GameEndResponse res, TutorialList? tutorialList, StageData LevelData, int FirstClear)
        {
            if (LevelData.TutorialEdit != null && LevelData.TutorialEdit.TutorialResp != null)
            {
                foreach (TutorialEntry item in LevelData.TutorialEdit.TutorialResp)
                {
                    if (tutorialList != null && (item.FirstClear == 0 || (item.FirstClear == 1 && FirstClear == 1)))
                    {
                        tutorialList.EditTutorialFlg(item.TutorialType, (int)item.TutorialId, item.TutorialStatus);
                    }
                }
            }
        }
        public static void HandleMenuFunc(GameEndRequest deserialized, GameEndResponse res, StageData LevelData, TableParser<YwpUserMenufunc> menufuncListTable, int FirstClear)
        {
            if (LevelData.Menufunc != null)
            {
                foreach (MenufuncEntry item in LevelData.Menufunc)
                {
                    if (FirstClear == 1)
                    {
                        MenufuncManager.AddApp(menufuncListTable, item.Id, (int)item.Value);
                    }
                }
            }
        }
        public static void HandleDrop(GameEndRequest deserialized, GameEndResponse res, TableParser<YwpUserDictionary> dictionaryYoukaiTable, TableParser<YwpUserDictionary> dictionaryDiff, 
            TableParser<YwpUserYoukai> userYoukaiTable, TableParser<YwpUserYoukai> youkaiDiff, TableParser<YwpUserYoukaiSkill> userYoukaiSkillTable, 
            TableParser<YwpUserYoukaiSkill> youkaiSkillDiff, TableParser<YwpUserItem> userItemTable, TableParser.Logic.TableParser playerIconTable, 
            YwpUserData userData, StageData LevelData, int FirstClear, TableParser<YwpUserYoukaiBonusEffect> userBonus)
        {
            var MstEnemyParam = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai_enemy_param"]!)!["tableData"]);

            foreach (EnemyYoukaiResultList i in deserialized!.EnemyYoukaiResultList!)
            {
                var MstEnemyParamIndex = MstEnemyParam.FindIndex([i.EnemyId.ToString()]);
                long YoukaiId = 0L;
                if (MstEnemyParamIndex != -1)
                    YoukaiId = long.Parse(MstEnemyParam.Table[MstEnemyParamIndex][1]);
                // add youkai to dictionary
                DictionaryManager.EditDictionary(ref dictionaryYoukaiTable, YoukaiId, true, false);
                DictionaryManager.EditDictionary(ref dictionaryDiff, YoukaiId, true, false);
                // add yokai if befriend
                if (i.DropYoukaiFlg == 1)
                {
                    if (YoukaiId != 0)
                    {
                        if (res.UserGameResultData.RewardYoukaiId == 0L)
                        {
                            res.YoukaiPopupResult = new YokaiWonPopup(YoukaiId, userYoukaiTable, userYoukaiSkillTable);
                            DictionaryManager.EditDictionary(ref dictionaryYoukaiTable, YoukaiId, true, true);
                            DictionaryManager.EditDictionary(ref dictionaryDiff, YoukaiId, true, true);
                            res.UserGameResultData.RewardYoukaiId = YoukaiId;
                            YoukaiManager.AddYoukai(userYoukaiTable, YoukaiId, userYoukaiSkillTable, userBonus);
                            YoukaiManager.AddYoukai(youkaiDiff, YoukaiId, youkaiSkillDiff, userBonus);
                        }
                    }
                }
                // add dropped item id
                if (i.DropItemFlg != 0)
                {
                    UserItemResultList val = new UserItemResultList();
                    val.ItemCnt = 1;
                    val.ItemId = i.DropItemId;
                    val.ItemType = 1;
                    val.FirstRewardFlg = 0;
                    val.NewFlg = 0;
                    val.ThemeBonusFlg = 0;
                    ItemManager.AddItem(userItemTable, i.DropItemId, 1);
                    res.UserItemResultList.Add(val);
                }
            }
            //add first reward item
            if (FirstClear == 1)
            {
                foreach (FirstRewardEntry entry in LevelData.FirstReward!)
                {
                    UserItemResultList val = new UserItemResultList();
                    val.ItemCnt = entry.ItemCount;
                    val.ItemId = entry.ItemId;
                    val.ItemType = entry.ItemType;
                    val.FirstRewardFlg = 1;
                    val.NewFlg = 0;
                    val.ThemeBonusFlg = 0;
                    if (entry.ItemType == 1)
                    {
                        userItemTable = ItemManager.AddItem(userItemTable, entry.ItemId, entry.ItemCount);
                    }
                    else if (entry.ItemType == 4)
                    {
                        userData.Hitodama += entry.ItemCount;
                    }
                    else if (entry.ItemType == 12)
                    {
                        var idx = playerIconTable.FindIndex([entry.ItemId.ToString()]);
                        if (idx == -1)
                        {
                            playerIconTable = PlayerIconManager.AddIcon(playerIconTable, (int)entry.ItemId);
                            val.NewFlg = 1;
                        }
                    }

                    res.UserItemResultList.Add(val);
                }
            }
        }
        public static async Task HandleAsync(HttpContext ctx, GameEndType gameEndType)
        {
            ctx.Response.ContentType = "application/json";
            ctx.Request.EnableBuffering();
            var buffer = new MemoryStream();
            await ctx.Request.Body.CopyToAsync(buffer);
            buffer.Seek(0, SeekOrigin.Begin);
            string? requestJsonString;
            using (var reader = new StreamReader(buffer, Encoding.UTF8))
            {
                var readResult = await reader.ReadToEndAsync();
                requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(readResult);
            }
            var deserialized = JsonConvert.DeserializeObject<GameEndRequest>(requestJsonString!);
            if (deserialized == null)
            {
                await GeneralUtils.SendBadRequest(ctx);
                return;
            }
            //its like this in the original game idk whys\
            deserialized.Score += 10000;

            var ReqId = await UDM.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_requestid");
            if ((ReqId == null || deserialized.RequestID == null || ReqId == "" || deserialized.RequestID == "") || (ReqId != deserialized.RequestID))
            {
                var errSession = new MsgBoxResponse("This session is invalid", "INVALID SESSION");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                return;
            }
            var jsonLevelData = JsonConvert.DeserializeObject<Dictionary<string, StageData>>(DataManager.Logic.DataManager.GameDataManager!.GamedataCache["stage_data"]);
            if (jsonLevelData == null || !(jsonLevelData.ContainsKey(deserialized.StageId.ToString())))
            {
                await GeneralUtils.SendBadRequest(ctx);
                return;
            }
            StageData LevelData = jsonLevelData[deserialized.StageId.ToString()];
            var userData = await UDM.GetYwpUserAsync<YwpUserData>(deserialized!.Gdkey!, "ywp_user_data");
            if (userData == null)
            {
                await GeneralUtils.SendBadRequest(ctx);
                return;
            }


            int newStarsCnt = 0;
            var youkaiDiff = new TableParser<YwpUserYoukai>("");
            var youkaiSkillDiff = new TableParser<YwpUserYoukaiSkill>("");
            var dictionaryDiff = new TableParser<YwpUserDictionary>("");
            var res = new GameEndResponse();

            res.UserGameResultData.Score = deserialized.Score;
            res.UserGameResultData.Exp = Server.GameServer.Logic.MoneyExpManager.ScoreToExp(deserialized.Score); // credit : Flamer12344
            res.UserGameResultData.Money = Server.GameServer.Logic.MoneyExpManager.ScoreToMoney(deserialized.Score); // credit : Flamer12344


            res.UserGameResultData.StageId = deserialized.StageId;

            // items data
            var itemsList = await UDM.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_item");
            var userItemTable = new TableParser.Logic.TableParser<YwpUserItem>(itemsList!);
            
            
            var ywpUserStage = new TableParser<YwpUserStage>(
                await UDM.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_stage")
            );
            var ywpUserMap = new TableParser<YwpUserMap>(
                await UDM.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_map")
            );

            // gameEnd for some ywp_user data, he send only modified row (ywp_user_..._diff)
            var userYoukai = await UDM.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_youkai");
            var userYoukaiSkill = await UDM.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_youkai_skill");
            var dictionaryYoukai = await UDM.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_dictionary");
            var userBonus = new TableParser<YwpUserYoukaiBonusEffect>(await UDM.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_youkai_bonus_effect"));
            var dictionaryYoukaiTable = new TableParser<YwpUserDictionary>(dictionaryYoukai!);
            var userYoukaiTable = new TableParser<YwpUserYoukai>(userYoukai!);
            TableParser<YwpUserYoukaiSkill> userYoukaiSkillTable = new(userYoukaiSkill!);
            var playerIcon = await UDM.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_player_icon");
            var playerIconTable = new TableParser.Logic.TableParser(playerIcon!);
            var menufuncList = await UDM.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_menufunc");
            var menufuncListTable = new TableParser<YwpUserMenufunc>(menufuncList!);
            var tutorialList = await UDM.GetYwpUserAsync<TutorialList>(deserialized!.Gdkey!, "ywp_user_tutorial_list");

            var MstEnemyParam = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai_enemy_param"]!)!["tableData"]);
            foreach (EnemyYoukaiResultList i in deserialized!.EnemyYoukaiResultList!)
            {
                var MstEnemyParamIndex = MstEnemyParam.FindIndex([i.EnemyId.ToString()]);
                long YoukaiId = 0L;
                if (MstEnemyParamIndex != -1)
                {
                    YoukaiId = long.Parse(MstEnemyParam.Table[MstEnemyParamIndex][1]);
                    DictionaryManager.EditDictionary(ref dictionaryYoukaiTable, YoukaiId, true, false);
                    DictionaryManager.EditDictionary(ref dictionaryDiff, YoukaiId, true, false);
                }
            }
            if (gameEndType == GameEndType.GameEnd)
            {
                int FirstClear = 0;
                HandleStage(deserialized, res, ref FirstClear, ywpUserStage, ywpUserMap);
                HandleDrop(deserialized, res, dictionaryYoukaiTable, dictionaryDiff, userYoukaiTable, youkaiDiff, userYoukaiSkillTable,  youkaiSkillDiff, userItemTable, playerIconTable, userData, LevelData, FirstClear, userBonus);
                HandleTutorial(deserialized, res, tutorialList, LevelData, FirstClear);
                HandleMenuFunc(deserialized, res, LevelData, menufuncListTable, FirstClear);
            }
            // yokai user list
            HandleUserYoukai(deserialized, res, userYoukaiTable, youkaiDiff);

            await UDM.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_requestid", "");
            await UDM.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_stage", ywpUserStage.ToString());
            await UDM.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_youkai", userYoukaiTable.ToString());
            await UDM.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_item", userItemTable.ToString());
            userData.YMoney += res.UserGameResultData.Money;
            await UDM.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_data", userData);
            await UDM.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_menufunc", menufuncListTable.ToString());
            await UDM.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_tutorial_list", tutorialList!);
            await UDM.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_dictionary", dictionaryYoukaiTable.ToString());
            await UDM.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_player_icon", playerIconTable.ToString());
            await UDM.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_map", ywpUserMap.ToString());
            await UDM.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_youkai_skill", userYoukaiSkillTable.ToString());
            await UDM.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_youkai_bonus_effect", userBonus.ToString());
            var resdict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(res));
            if (resdict == null)
            {
                await GeneralUtils.SendBadRequest(ctx);
                return;
            }
            string dictionaryDiffStr = dictionaryDiff.ToString();
            if (dictionaryDiffStr.StartsWith("*"))
            {
                dictionaryDiffStr = dictionaryDiffStr.Substring(1);
            }

            string youkaiDiffStr = youkaiDiff.ToString();
            if (youkaiDiffStr.StartsWith("*"))
            {
                youkaiDiffStr = youkaiDiffStr.Substring(1);
            }

            resdict["ywp_user_youkai_bonus_effect"] = userBonus.ToString();
            resdict["ywp_user_youkai_skill"] = userYoukaiSkillTable.ToString();
            resdict["ywp_user_youkai_bonus_effect_diff"] = ""; // TODO
            resdict["ywp_user_youkai_strong_skill_diff"] = ""; // TODO
            resdict["ywp_user_youkai"] = userYoukaiTable.ToString();
            resdict["ywp_user_dictionary"] = dictionaryYoukaiTable.ToString();

            await GeneralUtils.AddTablesToResponse(Consts.GAME_END_TABLES, resdict!, true, deserialized!.Gdkey!);
            var encryptedRes = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(resdict));
            ctx.Response.Headers.ContentType = "application/json";
            await ctx.Response.WriteAsync(encryptedRes);
            GenerateFriendData.RefreshYwpUserFriendRank(deserialized.Gdkey!, newStarsCnt, 0);
        }
    }
}
