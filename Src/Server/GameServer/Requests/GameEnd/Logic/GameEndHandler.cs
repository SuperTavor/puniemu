using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.Server.GameServer.DataClasses;
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
            var jsonLevelData = JsonConvert.DeserializeObject<Dictionary<string, StageData>>(ConfigManager.Logic.ConfigManager.GameDataManager!.GamedataCache["stage_data"]);
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
            var res = new GameEndResponse();

            // PLACEHOLDER
            res.UserGameResultData.Score = deserialized.Score;
            res.UserGameResultData.Exp = deserialized.Score;
            res.UserGameResultData.Money = deserialized.Score;
            res.UserGameResultData.StageId = deserialized.StageId;

            var MstEnemyParam = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_mst_youkai_enemy_param"]!)!["tableData"]);
            var stageConditionInfo = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_mst_stage_condition"]!)!["tableData"], "", "^");
            var stagesInfo = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_mst_stage"]!)!["tableData"]);
            var stageInfoIdx = stagesInfo.FindIndex([deserialized.StageId.ToString()]);
            var YoukaiLevelMstTable = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_mst_youkai_level"]!)!["tableData"]);
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
            var YoukaiMstTable = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_mst_youkai"]!)!["tableData"]);
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
                var dictionaryYoukaiIndex = dictionaryYoukaiTable.FindIndex([YoukaiId.ToString()]);
                if (dictionaryYoukaiIndex == -1)
                {
                    dictionaryYoukaiTable.AddRow([YoukaiId.ToString(), "0", "1"]);
                    dictionaryYoukaiTable = new TableParser.Logic.TableParser(dictionaryYoukaiTable.ToString());
                }
                dictionaryYoukaiIndex = dictionaryYoukaiTable.FindIndex([YoukaiId.ToString()]);
                dictionaryYoukaiTable.Table[dictionaryYoukaiIndex][1] = "1";
                // add yokai if befriend
                if (i.DropYoukaiFlg == 1)
                {
                    var UserYoukaiIndex = 0;
                    var MstYoukaiIndex = 0;
                    if (YoukaiId != 0) {
                        if (res.UserGameResultData.RewardYoukaiId == 0L) {
                            dictionaryYoukaiTable.Table[dictionaryYoukaiIndex][0] = "1";
                            res.UserGameResultData.RewardYoukaiId = YoukaiId;
                            UserYoukaiIndex = userYoukaiTable.FindIndex([YoukaiId.ToString()]);
                            MstYoukaiIndex = YoukaiMstTable.FindIndex([YoukaiId.ToString()]);
                            res.YoukaiPopupResult = new();
                            if (UserYoukaiIndex == -1)
                            {
                                // add new youkai
                                var tmpIdx = 0;
                                var levelType = int.Parse(YoukaiMstTable.Table[MstYoukaiIndex][5]);
                                foreach (string[] str in YoukaiLevelMstTable.Table)
                                {
                                    if (str[0] == levelType.ToString() && str[1] == "1")
                                        break;
                                    tmpIdx++;
                                }
                                // set Youkai id, level, total exp, hp, attack power, exp limit (for this level), exp (for this level), percentage of exp limit and exp (for this level), actual date, unk (maybe paid level)
                                userYoukaiTable.AddRow([YoukaiId.ToString(), "1", "0", YoukaiMstTable.Table[MstYoukaiIndex][8], YoukaiMstTable.Table[MstYoukaiIndex][10], YoukaiLevelMstTable.Table[tmpIdx][3], "0", "0", "0", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(), "0"]);
                                userYoukaiTable = new TableParser.Logic.TableParser(userYoukaiTable.ToString());

                            }
                            else
                            {
                                // else TODO : edit skill and other stuff if already befriends
                                Console.WriteLine("Not implemented");
                            }
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
                    var dropItem = itmesListTable.FindIndex([i.DropItemId.ToString()]);
                    if (dropItem == -1)
                    {
                        val.NewFlg = 1;
                        itmesListTable.AddRow([i.DropItemId.ToString(), "1"]);
                        itmesListTable = new TableParser.Logic.TableParser(itmesListTable.ToString());
                    }
                    else
                    {
                        itmesListTable.Table[dropItem][1] = (int.Parse(itmesListTable.Table[dropItem][1]) + 1).ToString();
                    }
                    res.UserItemResultList.Add(val);
                }
                // add first win item

                // remove used item id
                if (i.ItemId != 0)
                {
                    var foodItemIndex = itmesListTable.FindIndex([i.ItemId.ToString()]);
                    if (foodItemIndex != -1)
                    {
                        if (int.Parse(itmesListTable.Table[foodItemIndex][1]) > 0)
                        {
                            itmesListTable.Table[foodItemIndex][1] = (int.Parse(itmesListTable.Table[foodItemIndex][1]) - 1).ToString();
                        }
                    }

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
                        var idx = itmesListTable.FindIndex([entry.ItemId.ToString()]);
                        if (idx == -1)
                        {
                            itmesListTable.AddRow([entry.ItemId.ToString(), "1"]);
                            itmesListTable = new TableParser.Logic.TableParser(itmesListTable.ToString());
                            val.NewFlg = 1;
                        }
                        else
                        {
                            itmesListTable.Table[idx][1] = (int.Parse(itmesListTable.Table[idx][1]) + 1).ToString();
                        }
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
                            playerIconTable.AddRow([entry.ItemId.ToString()]);
                            playerIconTable = new TableParser.Logic.TableParser(playerIconTable.ToString());
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
                        index = tutorialListTable.FindIndex([item.TutorialType.ToString(), item.TutorialId.ToString()]);
                        if (index == -1)
                        {
                            tutorialListTable.AddRow([item.TutorialType.ToString(), item.TutorialId.ToString(), item.TutorialStatus.ToString()]);
                        }
                        //Set the tutorial status
                        tutorialListTable.Table[index][2] = item.TutorialStatus.ToString();
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
            var youkaiDiff = new TableParser.Logic.TableParser("");
            var dictionaryDiff = new TableParser.Logic.TableParser("");
            // user diff
            foreach(string[] key in userYoukaiTable.Table)
            {
                var idx = UserYoukaiOld.FindIndex(key);
                if (idx == -1)
                {
                    youkaiDiff.AddRow(key);
                }
                else
                {
                    /*if (UserYoukaiOld.Table[idx] != userYoukaiTable.Table[idx])
                    {
                        youkaiDiff.AddRow(key);
                    }*/
                }
            }
            foreach (string[] key in dictionaryYoukaiTable.Table)
            {
                var idx = UserDictionaryOld.FindIndex(key);
                if (idx == -1)
                {
                    dictionaryDiff.AddRow(key);
                }
                else
                {
                    /*
                    if (UserDictionaryOld.Table[idx] != dictionaryYoukaiTable.Table[idx])
                    {
                        dictionaryDiff.AddRow(key);
                    }*/
                }
            }
            resdict["ywp_user_youkai_diff"] = youkaiDiff.ToString();
            resdict["ywp_user_dictionary_diff"] = dictionaryDiff.ToString();
            await GeneralUtils.AddTablesToResponse(Consts.GAME_END_TABLES, resdict!, true, deserialized!.Gdkey!);
            var encryptedRes = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(resdict));
            ctx.Response.Headers.ContentType = "application/json";
            await ctx.Response.WriteAsync(encryptedRes);
        }
    }
}
