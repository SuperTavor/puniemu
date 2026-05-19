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
                UserYoukaiResultListRes item = new();
                item.HaveFlg = false; // TODO
                item.CanEvolve = false; //todo
                item.IsLockLevel = false; //todo (need to paid to be able to level up)
                item.isMaxLevel = false;
                item.youkaiId = i.YoukaiId;
                item.Before.Level = userYoukaiTable.Items[youkaiIDx].Level;
                item.Before.Exp = (int)userYoukaiTable.Items[youkaiIDx].Exp;
                item.Before.ExpBar.Denominator = userYoukaiTable.Items[youkaiIDx].ExpDenominator;
                item.Before.ExpBar.Numerator = userYoukaiTable.Items[youkaiIDx].ExpNumerator;
                item.Before.ExpBar.Percentage = userYoukaiTable.Items[youkaiIDx].Percentage;
                item.After.Level = item.Before.Level;
                item.After.Exp = item.Before.Exp;
                item.After.ExpBar.Denominator = item.Before.ExpBar.Denominator;
                item.After.ExpBar.Numerator = item.Before.ExpBar.Numerator;
                item.After.ExpBar.Percentage = item.Before.ExpBar.Percentage;

                
                if (userYoukaiTable.Items[youkaiIDx].Level < YoukaiMstTable.Items[YoukaiMstIndex].MaxLevel && userYoukaiTable.Items[youkaiIDx].IsLockedLevel == 0)
                {

                    var LevelIndex = 0;
                    var Level = item.Before.Level;
                    item.After.Exp = item.Before.Exp + res.UserGameResultData.Exp;

                    // get new level for youkai
                    while (LevelIndex != -1)
                    {
                        LevelIndex = MstYoukaiManager.GetYoukaiLevelIndex(YoukaiLevelMstTable, YoukaiMstTable.Items[YoukaiMstIndex].LevelType, Level);
                        Level++;
                        if (LevelIndex == -1)
                            break;
                        if ((item.After.Exp < YoukaiLevelMstTable.Items[LevelIndex].BaseExp) || (item.After.Exp > YoukaiLevelMstTable.Items[LevelIndex].MaxExp))
                        {
                            continue;
                        }
                        item.After.Level = YoukaiLevelMstTable.Items[LevelIndex].Level;
                        item.After.ExpBar.Denominator = (YoukaiLevelMstTable.Items[LevelIndex].MaxExp + 1) - YoukaiLevelMstTable.Items[LevelIndex].BaseExp;
                        item.After.ExpBar.Numerator = item.After.Exp - YoukaiLevelMstTable.Items[LevelIndex].BaseExp;
                        item.After.ExpBar.Percentage = (int)(((double)item.After.ExpBar.Numerator / (double)item.After.ExpBar.Denominator) * 100.0f);
                        break;
                    }

                    int YoukaiLevelOpenIndex = MstYoukaiManager.GetYoukaiLevelOpenIndex(YoukaiLevelOpenMstTable, item.Before.Level, item.After.Level, YoukaiMstTable.Items[YoukaiMstIndex].YoukaiRarity);
                    if (YoukaiLevelOpenIndex != -1)
                    {
                        item.After.Level = YoukaiLevelOpenMstTable.Items[YoukaiLevelOpenIndex].Level;
                        userYoukaiTable.Items[youkaiIDx].IsLockedLevel = 1;
                        int tmpindex = MstYoukaiManager.GetYoukaiLevelIndex(YoukaiLevelMstTable, YoukaiMstTable.Items[YoukaiMstIndex].LevelType, item.After.Level);
                        if (tmpindex != -1)
                        {
                            item.After.ExpBar.Denominator = (YoukaiLevelMstTable.Items[tmpindex].MaxExp + 1) - YoukaiLevelMstTable.Items[tmpindex].BaseExp;
                            item.After.Exp = YoukaiLevelMstTable.Items[tmpindex].BaseExp;
                            item.After.ExpBar.Numerator = 0;
                            item.After.ExpBar.Percentage = 0;
                        }
                    }
                }
                if (item.Before.Level != item.After.Level)
                    item.HaveFlg = false;
                userYoukaiTable.Items[youkaiIDx].Level = item.After.Level;
                userYoukaiTable.Items[youkaiIDx].Exp = item.After.Exp;
                userYoukaiTable.Items[youkaiIDx].ExpDenominator = item.After.ExpBar.Denominator;
                userYoukaiTable.Items[youkaiIDx].ExpNumerator = item.After.ExpBar.Numerator;
                userYoukaiTable.Items[youkaiIDx].Percentage = item.After.ExpBar.Percentage;

                int hpOffset = (YoukaiMstTable.Items[YoukaiMstIndex].MaxHp - YoukaiMstTable.Items[YoukaiMstIndex].BaseHp) / YoukaiMstTable.Items[YoukaiMstIndex].MaxLevel;
                int atkOffset = (YoukaiMstTable.Items[YoukaiMstIndex].MaxAtk - YoukaiMstTable.Items[YoukaiMstIndex].BaseAtk) / YoukaiMstTable.Items[YoukaiMstIndex].MaxLevel;
                userYoukaiTable.Items[youkaiIDx].Hp += (hpOffset * (item.After.Level - item.Before.Level));
                userYoukaiTable.Items[youkaiIDx].Atk += (atkOffset * (item.After.Level - item.Before.Level));
                if (userYoukaiTable.Items[youkaiIDx].IsLockedLevel == 1)
                {
                    item.IsLockLevel = true;
                }
                if (userYoukaiTable.Items[youkaiIDx].Level >= YoukaiMstTable.Items[YoukaiMstIndex].MaxLevel)
                {
                    item.isMaxLevel = true;
                    item.IsLockLevel = false;
                    userYoukaiTable.Items[youkaiIDx].IsLockedLevel = 0;
                    userYoukaiTable.Items[youkaiIDx].Level = YoukaiMstTable.Items[YoukaiMstIndex].MaxLevel;
                }
                YwpUserYoukai yokai = userYoukaiTable.Items[youkaiIDx];
                userYoukaiTableDiff.AddItem(yokai);
                res.UserYoukaiResultList.Add(item);
            }
        }
        public static void HandleStage(GameEndRequest deserialized, GameEndResponse res, int FirstClear, TableParser<YwpUserStage> ywpUserStage, TableParser<YwpUserMap> ywpUserMap)
        {
            List<YwpMstMap> ywpMstMap = JsonConvert.DeserializeObject<List<YwpMstMap>>(
                JsonConvert.DeserializeObject<Dictionary<string, object>>(
                    DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_map"]
                )!["data"].ToString()!
            )!;
           
            var userStageIdx = StageManager.GetStageIndex(ywpUserStage, deserialized.StageId);
            // create stage entry if it diden't exist yet
            if (userStageIdx == -1)
            {
                FirstClear = 1;
                StageManager.AddStage(ywpUserStage, deserialized.StageId);
            }
            // check if it's first clear
            if (ywpUserStage.Items[userStageIdx].IsClear == 0)
            {
                FirstClear = 1;
            }
            // check if it's a new record
            res.UserGameResultData.PrevScore = (int)ywpUserStage.Items[userStageIdx].Score;
            if (deserialized.Score > res.UserGameResultData.PrevScore)
            {
                res.UserGameResultData.ScoreUpdateFlg = 1;
            }

            for (int i = 0; i < 3 ; i++)
            {
                // used to differentiate locked levels from one same map
                int secretStageSkipp = 0;

                // compute the conditionId from stageId and conditionCount
                long currentCondId = MasterStageData.StageItems.Where(x => x.StageId == deserialized.StageId).First().StarCondIDs[i];

                // get the array index of the computed conditionId in the table
                int tempIndex = MasterStageData.GetStageConditionIndex(currentCondId);

                // if didn't exist we break cause this means we finished all conditions
                if (tempIndex == -1)
                    break;

                // we get the condition parameter
                long param1 = MasterStageData.ConditionItems[tempIndex].ConditionVal1;
                object sourceParam1 = 0;

                if (MasterStageData.ConditionItems[tempIndex].ConditionType == ConditionType.MinScore)
                {
                    sourceParam1 = deserialized.Score;
                }
                else if (MasterStageData.ConditionItems[tempIndex].ConditionType == ConditionType.MaxClearTime)
                {
                    sourceParam1 = deserialized.ClearTimeSec;
                }
                else if (MasterStageData.ConditionItems[tempIndex].ConditionType == ConditionType.MinLinkSize)
                {
                    sourceParam1 = deserialized.LinkSizeMax;
                }
                else if (MasterStageData.ConditionItems[tempIndex].ConditionType == ConditionType.UsedYoukai)
                {
                    sourceParam1 = deserialized.UserYoukaiResultList!;
                }

                bool good = ConditionManager.ComputeCondition(
                    MasterStageData.ConditionItems[tempIndex].ConditionType,
                    sourceParam1,
                    param1);

                Console.WriteLine($"{currentCondId} | {good} | {MasterStageData.ConditionItems[tempIndex].ConditionType}");

                // 1-3 are generally the condition for stars, 4+ are generally locked levels condition
                if (i == 1)
                {
                    res.UserGameResultData.StarGetFlg1 = good ? 1 : 0;
                }
                else if (i == 2)
                {
                    res.UserGameResultData.StarGetFlg2 = good ? 1 : 0;
                }
                else if (i == 3)
                {
                    res.UserGameResultData.StarGetFlg3 = good ? 1 : 0;
                }
                else if (i >= 4 && good)
                {
                    long newAddedStage = -1;

                    bool isFinalStageMap =
                        MasterStageData.GetNextStage(deserialized.StageId) == -1;

                    Console.WriteLine(MasterStageData.GetNextStage(deserialized.StageId));

                    var mapIndex = MstMapManager.GetMapIndex(
                        ywpMstMap,
                        (int)Math.Floor(deserialized.StageId / 1000.0));

                    if (isFinalStageMap && ywpMstMap[mapIndex].ReverseMapId != 0)
                    {
                        var mapIndex2 = MstMapManager.GetMapIndex(
                            ywpMstMap,
                            ywpMstMap[mapIndex].ReverseMapId);

                        if (mapIndex2 != -1)
                        {
                            int newMapIndex = MapManager.GetMapIndex(
                                ywpUserMap,
                                ywpMstMap[mapIndex2].MapId);

                            if (newMapIndex == -1)
                            {
                                MapManager.AddMap(ywpUserMap, ywpMstMap[mapIndex2].MapId);
                            }

                            MapManager.UpdateMap(ywpUserMap, ywpMstMap[mapIndex2].MapId, 1);

                            int newStageIdIndex = StageManager.GetStageIndex(
                                ywpUserStage,
                                (ywpMstMap[mapIndex2].MapId * 1000) + 1);

                            if (newStageIdIndex == -1)
                            {
                                StageManager.AddStage(
                                    ywpUserStage,
                                    (ywpMstMap[mapIndex2].MapId * 1000) + 1);

                                newAddedStage = (ywpMstMap[mapIndex2].MapId * 1000) + 1;
                            }
                        }
                    }
                    else
                    {
                        long newStageId =
                            MasterStageData.GetUnlockedSecretStage(deserialized.StageId, secretStageSkipp);

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
                        res.LockedStageResultList.Add(new LockedStageResultList
                        {
                            StageId = newAddedStage,
                            Title = MasterStageData.ConditionItems[tempIndex].OpenStageIdList,
                            ConditionType = (int)MasterStageData.ConditionItems[tempIndex].ConditionType,
                            Description = MasterStageData.ConditionItems[tempIndex].Description,
                            OriginStageId = 0,
                        });
                    }

                    secretStageSkipp++;
                }
            }
            StageManager.EditStage(ywpUserStage, deserialized.StageId, 1, deserialized.Score, res.UserGameResultData.StarGetFlg1, res.UserGameResultData.StarGetFlg2, res.UserGameResultData.StarGetFlg3, ywpUserStage.Items[userStageIdx].NumClear + 1);


            // beta might only work for few maps
            bool mapLocked = false;
            long nextStage = MasterStageData.GetNextStage(deserialized.StageId);
            if (nextStage == -1) // new map
            {
                var OgMapIndex = MstMapManager.GetMapIndex(ywpMstMap, (int)Math.Floor(deserialized.StageId / 1000.0));
                if (OgMapIndex != -1 && ywpMstMap[OgMapIndex].NextMapId != 0)
                {
                    int NewMstMapIndex = MstMapManager.GetMapIndex(ywpMstMap, ywpMstMap[OgMapIndex].NextMapId);
                    if (NewMstMapIndex != -1)
                    {
                        int newMapIndex = MapManager.GetMapIndex(ywpUserMap, ywpMstMap[NewMstMapIndex].MapId);
                        if (newMapIndex == -1)
                        {
                            mapLocked = !ywpMstMap[NewMstMapIndex].TextUnlock.IsNullOrEmpty();
                            MapManager.AddMap(ywpUserMap, ywpMstMap[NewMstMapIndex].MapId);
                        }
                        MapManager.UpdateMap(ywpUserMap, ywpMstMap[NewMstMapIndex].MapId, 1);
                        nextStage = (ywpMstMap[NewMstMapIndex].MapId * 1000) + 1;
                    }
                }
            }
            if (nextStage != -1 && StageManager.GetStageIndex(ywpUserStage, nextStage) == -1)
            {
                if (mapLocked == false) // we don't create stage placeholder if the new map is locked
                {
                    StageManager.AddStage(ywpUserStage, nextStage);
                }
                var nextStageItem = new LockedStageResultList()
                {
                    StageId = nextStage,
                    Title = "",
                    ConditionType = 0,
                    Description = "",
                    OriginStageId = 0,
                };
                res.LockedStageResultList.Add(nextStageItem);
            }
        }
        public static async Task HandleAsync(HttpContext ctx)
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
            var ReqId = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_requestid");
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
            var userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized!.Gdkey!, "ywp_user_data");
            if (userData == null)
            {
                await GeneralUtils.SendBadRequest(ctx);
                return;
            }
            int newStarsCnt = 0;
            var youkaiDiff = new TableParser<YwpUserYoukai>("");
            var youkaiSkillDiff = new TableParser<YwpUserYoukaiSkill>("");
            var dictionaryDiff = new TableParser.Logic.TableParser("");
            var res = new GameEndResponse();

            // PLACEHOLDER
            res.UserGameResultData.Score = deserialized.Score;
            res.UserGameResultData.Exp = Server.GameServer.Logic.MoneyExpManager.ScoreToExp(deserialized.Score); // credit : Flamer12344
            res.UserGameResultData.Money = Server.GameServer.Logic.MoneyExpManager.ScoreToMoney(deserialized.Score); // credit : Flamer12344


            res.UserGameResultData.StageId = deserialized.StageId;

            var MstEnemyParam = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai_enemy_param"]!)!["tableData"]);
            //var stageInfoIdx = stagesInfo.FindIndex([deserialized.StageId.ToString()]);
            //List<long> starIdx = new List<long> { long.Parse(stagesInfo.Table[stageInfoIdx][8]), long.Parse(stagesInfo.Table[stageInfoIdx][9]), long.Parse(stagesInfo.Table[stageInfoIdx][10])};

            // items data
            var itemsList = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_item");
            var itmesListTable = new TableParser.Logic.TableParser(itemsList!);


            //edit stage data
            
            
            var ywpUserStage = new TableParser<YwpUserStage>(
                await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_stage")
            );
            var ywpUserMap = new TableParser<YwpUserMap>(
                await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_map")
            );
            int FirstClear = 0;
            HandleStage(deserialized, res, FirstClear, ywpUserStage, ywpUserMap);



                // enemy list
                // gameEnd for some ywp_user data, he send only modified row (ywp_user_..._diff)
            var userYoukai = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_youkai");
            var userYoukaiSkill = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_youkai_skill");
            var dictionaryYoukai = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_dictionary");
            var dictionaryYoukaiTable = new TableParser.Logic.TableParser(dictionaryYoukai!);
            var userYoukaiTable = new TableParser<YwpUserYoukai>(userYoukai!);
            TableParser<YwpUserYoukaiSkill> userYoukaiSkillTable = new(userYoukaiSkill!);

            foreach (EnemyYoukaiResultList i in deserialized!.EnemyYoukaiResultList!)
            {
                var MstEnemyParamIndex = MstEnemyParam.FindIndex([i.EnemyId.ToString()]);
                long YoukaiId = 0L;
                if (MstEnemyParamIndex != -1)
                    YoukaiId = long.Parse(MstEnemyParam.Table[MstEnemyParamIndex][1]);
                // add youkai to dictionary
                dictionaryYoukaiTable = DictionaryManager.EditDictionary(dictionaryYoukaiTable, YoukaiId, true, false);
                dictionaryDiff = DictionaryManager.EditDictionary(dictionaryDiff, YoukaiId, true, false);
                // add yokai if befriend
                if (i.DropYoukaiFlg == 1)
                {
                    if (YoukaiId != 0) {
                        if (res.UserGameResultData.RewardYoukaiId == 0L) {
                            dictionaryYoukaiTable = DictionaryManager.EditDictionary(dictionaryYoukaiTable, YoukaiId, false, true);
                            dictionaryDiff = DictionaryManager.EditDictionary(dictionaryDiff, YoukaiId, false, true);
                            res.UserGameResultData.RewardYoukaiId = YoukaiId;
                            YoukaiManager.AddYoukai(userYoukaiTable, YoukaiId, userYoukaiSkillTable);
                            YoukaiManager.AddYoukai(youkaiDiff, YoukaiId, youkaiSkillDiff);

                            res.YoukaiPopupResult = new();
                            res.YoukaiPopupResult.IsWBonusEffectOpen = false; //IDK : TODO
                            res.YoukaiPopupResult.BonusEffectLevelBefore = 0; //IDK Todo
                            res.YoukaiPopupResult.StrongSkillLevelAfter = 0; //IDK Todo
                            res.YoukaiPopupResult.BonusEffectLevelAfter = 0; //IDK Todo
                            res.YoukaiPopupResult.StrongSkillLevelBefore = 0; //IDK Todo
                            res.YoukaiPopupResult.LegendYoukaiId = 0; // Legendary youkai flg Todo
                            res.YoukaiPopupResult.LevelBefore = 1; //level Todo
                            res.YoukaiPopupResult.LevelAfter = 1; //level todo
                            res.YoukaiPopupResult.GetTypes = 10; //IDK Todo
                            res.YoukaiPopupResult.YoukaiId = YoukaiId;
                            res.YoukaiPopupResult.ReleaseType = 0; //IDK todo
                            // skill data is null in the response for now so : IDK | TODO
                            res.YoukaiPopupResult.ExchgYmoney = 0; //IDK TODO
                            res.YoukaiPopupResult.LimitLevelAfter = 0; //IDK todo
                            res.YoukaiPopupResult.LimitLevelBefore = 0; //IDK todo
                            res.YoukaiPopupResult.ReleaseLevelType = 0; //IDK TODO

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
                    itmesListTable = ItemManager.AddItem(itmesListTable , i.DropItemId, 1);
                    res.UserItemResultList.Add(val);
                }
            }
            //add first reward item
            var playerIcon = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_player_icon");
            var playerIconTable = new TableParser.Logic.TableParser(playerIcon!);
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
                        itmesListTable = ItemManager.AddItem(itmesListTable, entry.ItemId, entry.ItemCount);
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

            // yokai user list
            HandleUserYoukai(deserialized, res, userYoukaiTable, youkaiDiff);

            // edit tutorial
            var tutorialList = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<TutorialList>(deserialized!.Gdkey!, "ywp_user_tutorial_list");
            
            if (LevelData.TutorialEdit != null && LevelData.TutorialEdit.TutorialResp != null)
            {
                foreach (TutorialEntry item in LevelData.TutorialEdit.TutorialResp)
                {
                    if (item.FirstClear == 0 || (item.FirstClear == 1 && FirstClear == 1))
                    {
                        tutorialList.EditTutorialFlg(item.TutorialType,(int) item.TutorialId, item.TutorialStatus);
                    }
                }
            }
            var menufuncList = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_menufunc");
            var menufuncListTable = new TableParser.Logic.TableParser(menufuncList!);
            if (LevelData.Menufunc != null)
            {
                foreach (MenufuncEntry item in LevelData.Menufunc)
                {
                    if (FirstClear == 1)
                    {
                        var MenuIndex = -1;
                        var tmpIdx = 0;
                        foreach (string[] str in menufuncListTable.Table)
                        {
                            if (str[0] == item.Id.ToString())
                                MenuIndex = tmpIdx;
                            tmpIdx++;
                        }
                        if (MenuIndex != -1)
                        {
                            menufuncListTable.Table[MenuIndex][1] = item.Value.ToString();
                        }
                    }
                }
            }
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_requestid", "");
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_stage", ywpUserStage.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_youkai", userYoukaiTable.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_item", itmesListTable.ToString());
            userData.YMoney += res.UserGameResultData.Money; // add money to user
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_data", userData);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_menufunc", menufuncListTable.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_tutorial_list", tutorialList);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_dictionary", dictionaryYoukaiTable.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_player_icon", playerIconTable.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_map", ywpUserMap.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_youkai_skill", userYoukaiSkillTable.ToString());

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
