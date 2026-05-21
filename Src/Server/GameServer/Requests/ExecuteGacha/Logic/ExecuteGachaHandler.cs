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

            var userData = (await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<GameServer.DataClasses.YwpUserData>(deserialized!.Level5UserId!, "ywp_user_data"))!;
            //gacha info
            var GachaMstTable = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_gacha"]!)!["tableData"]);
            var itmesListTable = new TableParser.Logic.TableParser(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserId!, "ywp_user_item")!);

            var DictionaryListTable = new TableParser.Logic.TableParser(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserId!, "ywp_user_dictionary")!);
            var UserYoukaiTable = new TableParser<YwpUserYoukai>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserId!, "ywp_user_youkai")!);

            var UserYoukaiSkillTable = new TableParser<YwpUserYoukaiSkill>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserId!, "ywp_user_youkai_skill")!);
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
                    int tmp_price = priceNum;
                    foreach(string[] item in itemsListMstTable.Table)
                    {
                        if (int.Parse(item[1]) == itemIdAction)
                        {
                            int tmp = ItemManager.GetItemCount(itmesListTable, int.Parse(item[0]));
                            number += tmp;
                            if (tmp_price - tmp <= 0)
                            {
                                itmesListTable = ItemManager.RemoveItem(itmesListTable, int.Parse(item[0]), tmp_price);
                                break;
                            } else
                            {
                                tmp_price -= tmp;
                                itmesListTable = ItemManager.RemoveItem(itmesListTable, int.Parse(item[0]), tmp);
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
                    number = ItemManager.GetItemCount(itmesListTable, priceId);
                    if ((number - priceNum) < 0)
                    {
                        var errSession = new MsgBoxResponse("Error occured", "Error");
                        await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                        return;
                    }
                    itmesListTable = ItemManager.RemoveItem(itmesListTable, priceId, priceNum);
                }

                //res = await ExecuteGachaResponse.BuildAsync(priceNum, pullCount, userId, userData.YMoney - priceNum, true);
            }
            var resp = new ExecuteGachaResponse();


            // MAIN HANDLER

            // Capsules should be:
            // blue - b
            // red - a
            // gold - S/SS/Pass
            var prizes = new List<GachaPrize>();
            var yokais = new List<YokaiGachaResult>();
            var capsuleIds = new List<int>();


            const int NEW_GETTYPE = 10;           // valeur observée dans ta sauvegarde pour "nouveau"
            const int DUPLICATE_GETTYPE = 2;      // valeur choisie pour signaler doublon (modifie si besoin)
            //const int DUPLICATE_REWARD_ITEM_ID = 20506; // exemple d'item donné en cas de doublon



            if (deserialized.RequestYoukaiId == 0)
            {
                List<(int CapsuleID, YokaiGachaResult Result)> crankResults = GachaService.Crank(deserialized.GachaId, pullCount);
                foreach (var t in crankResults)
                {
                    capsuleIds.Add(t.CapsuleID);
                    yokais.Add(t.Result);
                }
            }
            else
            {
                yokais.Add(new() { YokaiID = deserialized.RequestYoukaiId, YokaiRank = 0 });
                capsuleIds.Add(5);
                pullCount = 1;
            }

            for (int i = 0; i < pullCount; i++)
            {
                var yokai = yokais[i];

                int getType = DUPLICATE_GETTYPE;

                if (YoukaiManager.GetYoukaiIndex(UserYoukaiTable, yokai.YokaiID) < 0)
                {
                    getType = NEW_GETTYPE;
                }


                SkillResult? skill = GachaService.CompureSkillPctg(UserYoukaiSkillTable, yokai.YokaiID);

                YoukaiManager.AddYoukai(UserYoukaiTable, yokai.YokaiID, UserYoukaiSkillTable);
                
                DictionaryListTable = DictionaryManager.EditDictionary(DictionaryListTable, yokai.YokaiID, false, true);

                //bonusMap[youkaiId] = new[] { youkaiId.ToString(), "1", "0", "0" };
                //strongMap[youkaiId] = new[] { youkaiId.ToString(), "1", "0", "1000", "0", "0" 

                //

                prizes.Add(new GachaPrize
                {
                    Item = null,
                    CapsuleColor = capsuleIds[i],
                    PrizeType = 2,
                    Icon = null,
                    YMoney = null,
                    Youkai = new YokaiWonPopup
                    {
                        BonusEffectLevelAfter = 1,
                        StrongSkillLevelBefore = 0,
                        BonusEffectLevelBefore = 1,
                        LegendYoukaiId = 0,
                        StrongSkillLevelAfter = 0,
                        IsWBonusEffectOpen = false,
                        LevelAfter = 1,
                        LevelBefore = 1,
                        GetTypes = getType,
                        YoukaiId = yokai.YokaiID,
                        ReleaseType = 0,
                        Skill = skill,
                        ExchgYmoney = 0,
                        Exp = 0,
                        LimitLevelBefore = 0,
                        LimitLevelAfter = 0,
                        ReleaseLevelType = 0

                    },
                    RarityType = yokai.YokaiRank,
                    ConvertItemInfo = null

                });

            }

            resp.GachaPrizeList = prizes.ToArray();

            resp.EffectType = 1;
            resp.ResultCode = 0;
            resp.ResultType = 0;
            resp.NextScreenType = 0;

            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserId!, "ywp_user_dictionary", DictionaryListTable.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserId!, "ywp_user_youkai_skill", UserYoukaiSkillTable.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserId!, "ywp_user_youkai", UserYoukaiTable.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserId!, "ywp_user_data", userData);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserId!, "ywp_user_item", itmesListTable.ToString());
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserId!, "ywp_user_tutorial_list", tutorialList);
            var converter = new ExecuteGachaResponseConverter();
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(converter);
            var serialized = JsonConvert.SerializeObject(resp,settings);
            var resdict = JsonConvert.DeserializeObject<Dictionary<string, object>>(serialized)!;
            resdict["ywp_user_youkai"] = UserYoukaiTable.ToString();
            resdict["ywp_user_data"] = userData;
            resdict["ywp_user_dictionary"] = DictionaryListTable.ToString();
            resdict["ywp_user_youkai_skill"] = UserYoukaiSkillTable.ToString();
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
