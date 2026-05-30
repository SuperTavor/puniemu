using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.DataManager.Logic;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using Puniemu.Src.Utils.GeneralUtils;
using System.Buffers;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.Logic
{
    public class ExecuteGachaHandler
    {

        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);

            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<ExecuteGachaRequest>(requestJsonString!);

            GachaPoolManager.EnsureLoaded();
            if(!GachaPoolManager.Gachas.ContainsKey(deserialized.GachaId))
            {
                var errSession = new MsgBoxResponse($"No pool data exists for:\ngachaId:{deserialized.GachaId}", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                return;
            }
            var userData = (await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<GameServer.DataClasses.YwpUserData>(deserialized!.Level5UserId!, "ywp_user_data"))!;
            //gacha info
            var GachaMstTable = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_gacha"]!)!["tableData"]);
            var userItemtable = new TableParser.Logic.TableParser<YwpUserItem>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserId!, "ywp_user_item")!);

            var dictionaryListTable = new TableParser.Logic.TableParser(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserId!, "ywp_user_dictionary")!);
            var userYokaiTable = new TableParser<YwpUserYoukai>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserId!, "ywp_user_youkai")!);

            var userSkillTable = new TableParser<YwpUserYoukaiSkill>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserId!, "ywp_user_youkai_skill")!);
            //var bonusMap = new TableParser.Logic.TableParser(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserId!, "ywp_user_youkai_bonus_effect")!);
            //var strongMap = new TableParser.Logic.TableParser(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserId!, "ywp_user_youkai_strong_skill")!);

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

            int gachaIndex = GetTableIndex.GetIndex(GachaMstTable, new List<Tuple<int, string>>{Tuple.Create(0, deserialized.GachaId.ToString())});

            if (DataManager.Logic.DataManager.IsWibWob) pullCount = 1;
                else pullCount = int.Parse(GachaMstTable.Table[gachaIndex][11]); 

            int priceType = int.Parse(GachaMstTable.Table[gachaIndex][3]);
            long priceId = long.Parse(GachaMstTable.Table[gachaIndex][4]);
            int priceNum = int.Parse(GachaMstTable.Table[gachaIndex][5]);

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
                if (itemIdAction == 81) // Ymoney
                {
                    int price = priceNum;
                    foreach(string[] item in itemsListMstTable.Table)
                    {
                        if (int.Parse(item[1]) == itemIdAction)
                        {
                            
                            int userItemCount = userItemtable.Items.Where(x => x.ItemId == int.Parse(item[0])).First().Count;
                            number += userItemCount;
                            if (price - userItemCount <= 0)
                            {
                                userItemtable = ItemManager.RemoveItem(userItemtable, int.Parse(item[0]), price);
                                break;
                            } else
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
            //const int DUPLICATE_REWARD_ITEM_ID = 20506; // exemple d'item donné en cas de doublon


            if (deserialized.RequestYoukaiId == 0)
            {
               for(int i = 0; i < pullCount; i++)
               {
                    prizes.Add(GachaPoolManager.CrankReward(gachaId, userYokaiTable, userSkillTable, dictionaryListTable, userItemtable));
               }
            }
            else
            {
                prizes.Add(GachaPoolManager.RegisterYokaiAndGetPrize(deserialized.RequestYoukaiId, CapsuleColor.Gray, 0, userYokaiTable, userSkillTable, dictionaryListTable));
                pullCount = 1;
            }

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
            var settings = new JsonSerializerSettings();
            var converter = new ExecuteGachaResponseConverter();
            settings.Converters.Add(converter);
            var serialized = JsonConvert.SerializeObject(resp,settings);
            var resdict = JsonConvert.DeserializeObject<Dictionary<string, object>>(serialized)!;
            resdict["ywp_user_youkai"] = userYokaiTable.ToString();
            resdict["ywp_user_data"] = userData;
            resdict["ywp_user_dictionary"] = dictionaryListTable.ToString();
            resdict["ywp_user_youkai_skill"] = userSkillTable.ToString();
            List<string> list = new List<string>();
            list.Add("gachaLotRule");
            //add to response
            if (tutorial_changed)
            {
                resdict["ywp_user_tutorial_list"] = tutorialList;
            }
           
            await GeneralUtils.AddTablesToResponse(Consts.EXECUTE_GACHA_TABLES, resdict!, true, deserialized!.Level5UserId!);
            var outResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(resdict));
            await ctx.Response.WriteAsync(outResponse);
            return;
        }
    }
}
