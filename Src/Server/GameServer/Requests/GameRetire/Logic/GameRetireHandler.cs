using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Requests.GameRetire.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
namespace Puniemu.Src.Server.GameServer.Requests.GameRetire.Logic
{
    public static class GameRetireHandler
    {

        public static async Task HandleAsync(HttpContext ctx)
        {
            // WIP : the code is rather messy because this .nhn is incomplete and still under development

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
            var deserialized = JsonConvert.DeserializeObject<GameRetireRequest>(requestJsonString!);
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
            var youkaiDiff = new TableParser.Logic.TableParser("");
            var dictionaryDiff = new TableParser.Logic.TableParser("");
            var index = 0;
            var res = new GameRetireResponse();

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

            // stage
            var stageList = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_stage");
            var stageListTable = new TableParser.Logic.TableParser(stageList!);

            var stageIndex = stageListTable.FindIndex([deserialized.StageId.ToString()]);

            // stage modifier
            if (stageIndex == -1)
            {
                stageListTable.AddRow([deserialized.StageId.ToString(), "0", "0", "0", "0", deserialized.Score.ToString(), "1", "0"]);
                res.UserGameResultData.PrevScore = 0;
            }
            else 
            {
                res.UserGameResultData.PrevScore = int.Parse(stageListTable.Table[stageIndex][5]);
                stageListTable.Table[stageIndex][6] = "1"; // idk
                if (deserialized.Score > int.Parse(stageListTable.Table[stageIndex][5]))
                {
                    res.UserGameResultData.ScoreUpdateFlg = 1;
                    stageListTable.Table[stageIndex][5] = deserialized.Score.ToString();
                }
            }

            // set getstar flg
            res.UserGameResultData.StarGetFlg1 = 0;
            res.UserGameResultData.StarGetFlg2 = 0;
            res.UserGameResultData.StarGetFlg3 = 0;




            // enemy list
            // gameRetire for some ywp_user data, he send only modified row (ywp_user_..._diff)
            var YoukaiMstTable = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_mst_youkai"]!)!["tableData"]);
            var userYoukai = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_youkai");
            var dictionaryYoukai = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_dictionary");
            var dictionaryYoukaiTable = new TableParser.Logic.TableParser(dictionaryYoukai!); 
            var userYoukaiTable = new TableParser.Logic.TableParser(userYoukai!);

            foreach (EnemyYoukaiResultList i in deserialized!.EnemyYoukaiResultList!)
            {
                var MstEnemyParamIndex = MstEnemyParam.FindIndex([i.EnemyId.ToString()]);
                long YoukaiId = 0L;
                if (MstEnemyParamIndex != -1)
                    YoukaiId = long.Parse(MstEnemyParam.Table[MstEnemyParamIndex][1]);
                // add youkai to dictionary

                dictionaryYoukaiTable = DictionaryManager.EditDictionary(dictionaryYoukaiTable, YoukaiId, true, false);
                dictionaryDiff = DictionaryManager.EditDictionary(dictionaryDiff, YoukaiId, true, false);
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
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_requestid", "");
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_stage", stageListTable.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_youkai", userYoukaiTable.ToString());
            userData.YMoney += res.UserGameResultData.Money; // add money to user
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_data", userData);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_dictionary", dictionaryYoukaiTable.ToString());


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
