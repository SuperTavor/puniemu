using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Requests.GameEnd.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Requests.GameEnd.Logic
{
    public static class GameEndHandler
    {

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

            var youkaiDiff = new TableParser.Logic.TableParser("");
            var dictionaryDiff = new TableParser.Logic.TableParser("");
            var res = new GameEndResponse();

            // PLACEHOLDER
            res.UserGameResultData.Score = deserialized.Score;
            res.UserGameResultData.Exp = deserialized.Score;
            res.UserGameResultData.Money = deserialized.Score;
            res.UserGameResultData.StageId = deserialized.StageId;

            var MstEnemyParam = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai_enemy_param"]!)!["tableData"]);
            var stageConditionInfo = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_stage_condition"]!)!["tableData"], "", "^");
            var stagesInfo = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_stage"]!)!["tableData"]);
            var stageInfoIdx = stagesInfo.FindIndex([deserialized.StageId.ToString()]);
            var YoukaiLevelMstTable = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai_level"]!)!["tableData"]);
            List<long> starIdx = new List<long> { long.Parse(stagesInfo.Table[stageInfoIdx][8]), long.Parse(stagesInfo.Table[stageInfoIdx][9]), long.Parse(stagesInfo.Table[stageInfoIdx][10])};

            // items data
            var itemsList = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_item");
            var itmesListTable = new TableParser.Logic.TableParser(itemsList!);
            // stage
            var stageList = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_stage");
            var stageListTable = new TableParser.Logic.TableParser(stageList!);

            var stageIndex = stageListTable.FindIndex([deserialized.StageId.ToString()]);

            // stage modifier
            var FirstClear = 0;
            if (stageIndex == -1)
            {
                FirstClear = 1;
                stageListTable.AddRow([deserialized.StageId.ToString(), "1", "0", "0", "0", deserialized.Score.ToString(), "1", "0"]);
                res.UserGameResultData.PrevScore = 0;
            }
            else 
            {
                if (stageListTable.Table[stageIndex][1] == "0")
                {
                    FirstClear = 1;
                }
                res.UserGameResultData.PrevScore = int.Parse(stageListTable.Table[stageIndex][5]);
                stageListTable.Table[stageIndex][1] = "1"; // cleared flg
                stageListTable.Table[stageIndex][6] = "1"; // idk
                if (deserialized.Score > int.Parse(stageListTable.Table[stageIndex][5]))
                {
                    res.UserGameResultData.ScoreUpdateFlg = 1;
                    stageListTable.Table[stageIndex][5] = deserialized.Score.ToString();
                }
            }

            // Add star if win
            stageIndex = stageListTable.FindIndex([deserialized.StageId.ToString()]);
            int index = 0;
            List<int> starGetFlg = new List<int> { 0, 0, 0 };
            foreach (long i in starIdx)
            {
                var stageConditionIdx = stageConditionInfo.FindIndex([i.ToString()]);
                var starType = int.Parse(stageConditionInfo.Table[stageConditionIdx][1]);
                var goodFlg = 0;
                var value = stageConditionInfo.Table[stageConditionIdx][3];
                if (starType == 1 && deserialized.Score >= int.Parse(value))
                    goodFlg = 1;
                else if (starType == 3 && (deserialized.ClearTimeSec <= int.Parse(value) || (deserialized.ClearTimeLongSec <= long.Parse(value) && deserialized.ClearTimeLongSec > 0)))
                    goodFlg = 1;
                else if (starType == 10 && deserialized.LinkSizeMax >= int.Parse(value))
                    goodFlg = 1;
                if (goodFlg == 1)
                {
                    if (int.Parse(stageListTable.Table[stageIndex][index + 2]) == 1)
                    {
                        starGetFlg[index] = 2;
                    }
                    else
                    {
                        starGetFlg[index] = 1;
                    }
                    stageListTable.Table[stageIndex][index + 2] = "1";
                }
                index++;
            }
            index = 0;
            // set getstar flg (im not sure for flg value)
            foreach (int i in starGetFlg)
            {
                if (starGetFlg[index] == 1 || starGetFlg[index] == 2)
                {
                    if (index == 0)
                    {
                        res.UserGameResultData.StarGetFlg1 = starGetFlg[index];
                    }
                    else if (index == 1)
                    {
                        res.UserGameResultData.StarGetFlg2 = starGetFlg[index];
                    }
                    else if (index == 2)
                    {
                        res.UserGameResultData.StarGetFlg3 = starGetFlg[index];
                    }
                }
                index++;
            }


            // pre add nex stage user data
            var NxtStageIndex = stagesInfo.FindIndex([(deserialized.StageId + 1).ToString()]);

            if (NxtStageIndex != -1)
            {
                var NxtStageUsrIndex = stageListTable.FindIndex([(deserialized.StageId + 1).ToString()]); // pre add new stage info in user data
                if (NxtStageUsrIndex == -1)
                {
                    stageListTable.AddRow([(deserialized.StageId + 1).ToString(), "0", "0", "0", "0", "0", "0", "0"]);
                }
                // probably unlock level animation TODO
                if (FirstClear == 1)
                {
                    var item = new LockedStageResultList()
                    {
                        StageId = (deserialized.StageId + 1),
                        Title = string.Empty, //todo
                        ConditionType = 0,
                        Description = string.Empty,
                        OriginStageId = 0,
                    };
                    res.LockedStageResultList.Add(item);
                }
            }



            // enemy list
            // gameEnd for some ywp_user data, he send only modified row (ywp_user_..._diff)
            var YoukaiMstTable = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai"]!)!["tableData"]);
            var userYoukai = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_youkai");
            var dictionaryYoukai = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_dictionary");
            var dictionaryYoukaiTable = new TableParser.Logic.TableParser(dictionaryYoukai!);
            var UserDictionaryOld = new TableParser.Logic.TableParser(dictionaryYoukai!); ;
            var userYoukaiTable = new TableParser.Logic.TableParser(userYoukai!);
            var UserYoukaiOld = new TableParser.Logic.TableParser(userYoukai!); ;

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
                            userYoukaiTable = YoukaiManager.AddYoukai(userYoukaiTable, YoukaiId);
                            youkaiDiff = YoukaiManager.AddYoukai(youkaiDiff, YoukaiId);

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
            foreach (UserYoukaiResultListReq i in deserialized!.UserYoukaiResultList!)
            {
                var youkaiIDx = userYoukaiTable.FindIndex([i.YoukaiId.ToString()]);
                if (youkaiIDx != -1) {
                    var YoukaiMstIndex = YoukaiMstTable.FindIndex([i.YoukaiId.ToString()]);
                    var levelType = int.Parse(YoukaiMstTable.Table[YoukaiMstIndex][5]); // get level exp const value
                    UserYoukaiResultListRes item = new();
                    item.HaveFlg = false; // TODO
                    item.CanEvolve = false; //todo
                    item.IsLockLevel = false; //todo (need to paid to be able to level up)
                    item.isMaxLevel = false; //todo
                    item.youkaiId = i.YoukaiId;
                    item.Before.Level = int.Parse(userYoukaiTable.Table[youkaiIDx][1]);
                    item.Before.Exp = int.Parse(userYoukaiTable.Table[youkaiIDx][2]);
                    item.Before.ExpBar.Denominator = int.Parse(userYoukaiTable.Table[youkaiIDx][5]);
                    item.Before.ExpBar.Numerator = int.Parse(userYoukaiTable.Table[youkaiIDx][6]);
                    item.Before.ExpBar.Percentage = int.Parse(userYoukaiTable.Table[youkaiIDx][7]);


                    var LevelIndex = 0;
                    var const_exp = 200; // test
                    item.After.Exp = item.Before.Exp + const_exp;
                    index = 1;

                    while (LevelIndex != -1)
                    {
                        LevelIndex = -1;
                        var tmpIdx = 0;
                        foreach (string[] str in YoukaiLevelMstTable.Table)
                        {
                            if (str[0] == levelType.ToString() && str[1] == index.ToString())
                                LevelIndex = tmpIdx;
                            tmpIdx++;
                        }
                        if (LevelIndex != -1)
                        {
                            if ((item.After.Exp >= int.Parse(YoukaiLevelMstTable.Table[LevelIndex][2])) && (item.After.Exp <= int.Parse(YoukaiLevelMstTable.Table[LevelIndex][3])))
                            {
                                item.After.Level = int.Parse(YoukaiLevelMstTable.Table[LevelIndex][1]);
                                item.After.ExpBar.Denominator = (int.Parse(YoukaiLevelMstTable.Table[LevelIndex][3]) + 1) - int.Parse(YoukaiLevelMstTable.Table[LevelIndex][2]);
                                item.After.ExpBar.Numerator = item.After.Exp - int.Parse(YoukaiLevelMstTable.Table[LevelIndex][2]);
                                item.After.ExpBar.Percentage = (int)(((double)((double)item.After.ExpBar.Numerator / (double)item.After.ExpBar.Denominator)) * 100);
                                break;
                            }
                            index++;
                        }
                    }
                    if (index == -1)
                        item.isMaxLevel = true;
                    if (item.Before.Level != item.After.Level)
                        item.HaveFlg = false;
                    userYoukaiTable.Table[youkaiIDx][1] = item.After.Level.ToString();
                    userYoukaiTable.Table[youkaiIDx][2] = item.After.Exp.ToString();
                    userYoukaiTable.Table[youkaiIDx][5] = item.After.ExpBar.Denominator.ToString();
                    userYoukaiTable.Table[youkaiIDx][6] = item.After.ExpBar.Numerator.ToString();
                    userYoukaiTable.Table[youkaiIDx][7] = item.After.ExpBar.Percentage.ToString();
                    string[] yokai = userYoukaiTable.Table[youkaiIDx];
                    youkaiDiff.AddRow(yokai);
                    youkaiDiff = new TableParser.Logic.TableParser(youkaiDiff.ToString());
                    res.UserYoukaiResultList.Add(item);
                }
            }

            // edit tutorial
            var tutorialList = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_tutorial_list");
            var tutorialListTable = new TableParser.Logic.TableParser(tutorialList!);
            
            if (LevelData.TutorialEdit != null && LevelData.TutorialEdit.TutorialResp != null)
            {
                foreach (TutorialEntry item in LevelData.TutorialEdit.TutorialResp)
                {
                    if (item.FirstClear == 0 || (item.FirstClear == 1 && FirstClear == 1))
                    {
                        tutorialListTable = TutorialFlagManager.EditTutorialFlg(tutorialListTable, item.TutorialType,(int) item.TutorialId, item.TutorialStatus);
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
                            if (str[0] == item.id.ToString())
                                MenuIndex = tmpIdx;
                            tmpIdx++;
                        }
                        if (MenuIndex != -1)
                        {
                            menufuncListTable.Table[MenuIndex][1] = item.value.ToString();
                        }
                    }
                }
            }
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_requestid", "");
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_stage", stageListTable.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_youkai", userYoukaiTable.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_item", itmesListTable.ToString());
            userData.YMoney += res.UserGameResultData.Money; // add money to user
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_data", userData);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_menufunc", menufuncListTable.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_tutorial_list", tutorialListTable.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_dictionary", dictionaryYoukaiTable.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_player_icon", playerIconTable.ToString());


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
            resdict["ywp_user_youkai_diff"] = youkaiDiffStr;
            resdict["ywp_user_dictionary_diff"] = dictionaryDiffStr;

            await GeneralUtils.AddTablesToResponse(Consts.GAME_END_TABLES, resdict!, true, deserialized!.Gdkey!);
            var encryptedRes = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(resdict));
            ctx.Response.Headers.ContentType = "application/json";
            await ctx.Response.WriteAsync(encryptedRes);
        }
    }
}
