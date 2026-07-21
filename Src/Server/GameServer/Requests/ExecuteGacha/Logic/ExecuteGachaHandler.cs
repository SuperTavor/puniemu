using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.DataManager.Logic;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using Puniemu.Src.UserDataManager.Logic;
using Puniemu.Src.Utils.GeneralUtils;
using System.Buffers;
using System.Text;

// ONI FEVER : DEVELOPED BY WIBWOB_YT

namespace Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.Logic
{
    public class ExecuteGachaHandler
    {
        private const int FEVER_PULLS_REQUIRED = 5;

        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);

            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<ExecuteGachaRequest>(requestJsonString!);

            GachaPoolManager.EnsureLoaded();
            if (!GachaPoolManager.Gachas.ContainsKey(deserialized.GachaId))
            {
                var errSession = new MsgBoxResponse($"No pool data exists for:\ngachaId:{deserialized.GachaId}", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                return;
            }
            var userData = (await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<GameServer.DataClasses.YwpUserData>(deserialized!.Level5UserId!, "ywp_user_data"))!;
            //gacha info
            var GachaMstTable = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_gacha"]!)!["tableData"]);
            var userItemtable = new TableParser.Logic.TableParser<YwpUserItem>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserId!, "ywp_user_item")!);
            var userBonus = new TableParser<YwpUserYoukaiBonusEffect>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserId, "ywp_user_youkai_bonus_effect"));
            var dictionaryListTable = new TableParser<YwpUserDictionary>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserId!, "ywp_user_dictionary")!);
            var userYokaiTable = new TableParser<YwpUserYoukai>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserId!, "ywp_user_youkai")!);
            var userSkillTable = new TableParser<YwpUserYoukaiSkill>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserId!, "ywp_user_youkai_skill")!);
            //var bonusMap = new TableParser.Logic.TableParser(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserId!, "ywp_user_youkai_bonus_effect")!);
            //var strongMap = new TableParser.Logic.TableParser(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserId!, "ywp_user_youkai_strong_skill")!);

            var mstYokai = new TableParser<YwpMstYoukai>(
                JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai"]!
                    )!["tableData"]
            );
            var tutorialList = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<TutorialList>(deserialized.Level5UserId, "ywp_user_tutorial_list");
            bool tutorial_changed = false;
            if (tutorialList.GetStatus(2, 2) == 0)
            {
                tutorialList.EditTutorialFlg(2, 2, 1);
                tutorial_changed = true;
            }
            var itemsListMstTable = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_item"]!)!["tableData"]);

            //101668

            int pullCount = deserialized.GachaCnt;
            int gachaId = deserialized.GachaId;
            var userId = deserialized.UserId;

            int gachaIndex = GetTableIndex.GetIndex(GachaMstTable, new List<Tuple<int, string>> { Tuple.Create(0, deserialized.GachaId.ToString()) });

            if (DataManager.Logic.DataManager.IsWibWob) pullCount = 1;
            else pullCount = int.Parse(GachaMstTable.Table[gachaIndex][11]);

            int priceType = int.Parse(GachaMstTable.Table[gachaIndex][3]);
            long priceId = long.Parse(GachaMstTable.Table[gachaIndex][4]);
            int priceNum = int.Parse(GachaMstTable.Table[gachaIndex][5]);

            
            int gachaType = int.Parse(GachaMstTable.Table[gachaIndex][6]);

            if (priceType == 1) // YMONEY
            {
                if ((userData.YMoney - priceNum) < 0)
                {
                    var errSession = new MsgBoxResponse("Error occured", "Error");
                    await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                    return;
                }
                userData.YMoney -= priceNum;
            }
            else if (priceType == 2) // ITEMS
            {
                int itemIdx = GetTableIndex.GetIndex(itemsListMstTable, new List<Tuple<int, string>> { Tuple.Create(0, priceId.ToString()) });
                if (itemIdx < 0)
                {
                    var errSession = new MsgBoxResponse("Error occured", "Error");
                    await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                    return;
                }
                int itemIdAction = int.Parse(itemsListMstTable.Table[itemIdx][1]);
                int number = 0;
                if (itemIdAction == 81) // Ypoint
                {
                    int price = priceNum;
                    foreach (string[] item in itemsListMstTable.Table)
                    {
                        if (int.Parse(item[1]) == itemIdAction)
                        {
                            int userItemCount = userItemtable.Items.FirstOrDefault(x => x.ItemId == int.Parse(item[0]))?.Count ?? 0;
                            number += userItemCount;
                            if (price - userItemCount <= 0)
                            {
                                userItemtable = ItemManager.RemoveItem(userItemtable, int.Parse(item[0]), price);
                                break;
                            }
                            else if (userItemCount > 0)
                            {
                                price -= userItemCount;
                                userItemtable = ItemManager.RemoveItem(userItemtable, int.Parse(item[0]), userItemCount);
                            }
                        }
                    }
                    if ((number - priceNum) < 0)
                    {
                        var errSession = new MsgBoxResponse("Error occured", "Error");
                        await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                        return;
                    }
                }
                else
                {
                    number = userItemtable.Items.Where(x => x.ItemId == priceId).First().Count;
                    if ((number - priceNum) < 0)
                    {
                        var errSession = new MsgBoxResponse("Error occured", "Error");
                        await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                        return;
                    }
                    userItemtable = ItemManager.RemoveItem(userItemtable, priceId, priceNum);
                }

                //res = await ExecuteGachaResponse.BuildAsync(priceNum, pullCount, userId, userData.YMoney - priceNum, true);
            }
            var resp = new ExecuteGachaResponse();


            // MAIN HANDLER

            var prizes = new List<GachaPrize>();
            //const int DUPLICATE_REWARD_ITEM_ID = 20506; 

            if (deserialized.RequestYoukaiId == 0)
            {
                var mode = GachaMaxedYokaiMode.ConvertToItem;
                if (DataManager.Logic.DataManager.IsWibWob) mode = GachaMaxedYokaiMode.RerollUntilValid;
                for (int i = 0; i < pullCount; i++)
                {
                    prizes.Add(await GachaPoolManager.CrankReward(gachaId, userYokaiTable, userSkillTable, dictionaryListTable, userItemtable, userBonus, deserialized.Level5UserId, mode));
                }
            }

            else if (deserialized.RequestYoukaiId != 0 && GachaYoukaiChoiceManager.IsChoiceOk(gachaId, deserialized.RequestYoukaiId))
            {
                //Check yokai rarity type
                var ykIdx = mstYokai.Items.FindIndex(x => x.YoukaiId == deserialized.RequestYoukaiId);
                if (ykIdx == -1)
                {
                    var errSession = new MsgBoxResponse("Error occured", "Error");
                    await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                    return;
                }
                var rarity = mstYokai.Items[ykIdx].YoukaiRarity;
                prizes.Add(await GachaPoolManager.RegisterYokaiAndGetPrize(deserialized.RequestYoukaiId, CapsuleColor.Red, rarity,
                    userYokaiTable, userSkillTable, dictionaryListTable, userItemtable, gachaId, userBonus, deserialized.Level5UserId));
                pullCount = 1;
            }
            else
            {
                var errSession = new MsgBoxResponse("Error occured", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                return;
            }

            
            int feverChargeType = 0;
            object? gachaListForResponse = null;
            var userGachaList = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<YwpUserGachaEntry>>(deserialized!.Level5UserId!, "ywp_user_gacha");
            if (userGachaList != null)
            {
                var feverEntry = userGachaList.FirstOrDefault(g => g.GachaType == gachaType);
                if (feverEntry != null)
                {
                    int feverIncrement = 100 / FEVER_PULLS_REQUIRED;
                    int newPctg = feverEntry.FeverPctg + feverIncrement * pullCount;
                    if (newPctg >= 100)
                    {
                        feverChargeType = 2;
                        int remainder = newPctg % 100;
                        feverEntry.FeverPctg = remainder;
                        await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserId!, "ywp_user_gacha", userGachaList);
                        gachaListForResponse = new List<object>
                        {
                            new { feverPctg = 100, gachaType = feverEntry.GachaType }
                        };
                    }
                    else
                    {
                        feverChargeType = 1;
                        feverEntry.FeverPctg = newPctg;
                        await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserId!, "ywp_user_gacha", userGachaList);
                        gachaListForResponse = userGachaList;
                    }
                }
            }

            int color1Cnt = prizes.Count(p => p.CapsuleColor == CapsuleColor.Gray);
            int color2Cnt = prizes.Count(p => p.CapsuleColor == CapsuleColor.Blue);
            int color3Cnt = prizes.Count(p => p.CapsuleColor == CapsuleColor.Red);
            int color4Cnt = prizes.Count(p => p.CapsuleColor == CapsuleColor.Gold || p.CapsuleColor == CapsuleColor.Rainbow);

            resp.EffectType = 1;
            resp.ResultCode = 0;
            resp.ResultType = 0;
            resp.NextScreenType = 0;
            resp.GachaPrizeList = prizes;
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserId!, "ywp_user_dictionary", dictionaryListTable.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserId!, "ywp_user_youkai_skill", userSkillTable.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserId!, "ywp_user_youkai", userYokaiTable.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserId!, "ywp_user_data", userData);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserId!, "ywp_user_item", userItemtable.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserId!, "ywp_user_tutorial_list", tutorialList);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserId!, "ywp_user_youkai_bonus_effect", userBonus.ToString());
            var settings = new JsonSerializerSettings();
            var converter = new ExecuteGachaResponseConverter();
            settings.Converters.Add(converter);
            var serialized = JsonConvert.SerializeObject(resp, settings);
            var resdict = JsonConvert.DeserializeObject<Dictionary<string, object>>(serialized)!;
            resdict["ywp_user_youkai"] = userYokaiTable.ToString();
            resdict["ywp_user_data"] = userData;
            resdict["ywp_user_youkai_bonus_effect"] = userBonus.ToString();
            resdict["ywp_user_dictionary"] = dictionaryListTable.ToString();
            resdict["ywp_user_youkai_skill"] = userSkillTable.ToString();
            List<string> list = new List<string>();
            list.Add("gachaLotRule");
            //add to response
            if (tutorial_changed)
            {
                resdict["ywp_user_tutorial_list"] = tutorialList;
            }
            if (feverChargeType > 0)
            {
                resdict["rouletteEffect"] = new
                {
                    rouletteEffectType = 0,
                    feverChargeType = feverChargeType,
                    color1Cnt = color1Cnt,
                    color2Cnt = color2Cnt,
                    color3Cnt = color3Cnt,
                    color4Cnt = color4Cnt,
                    color4NormalCnt = 0
                };
            }
            await MissionManager.UpdateProgress(deserialized.Level5UserId, GameServer.DataClasses.Mission.MissionType.TotalCrank, 1);
            await GeneralUtils.AddTablesToResponse(Consts.EXECUTE_GACHA_TABLES, resdict!, true, deserialized!.Level5UserId!);
            if (gachaListForResponse != null)
                resdict["ywp_user_gacha"] = gachaListForResponse;
            var outResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(resdict));
            await ctx.Response.WriteAsync(outResponse);
            return;
        }
    }
}