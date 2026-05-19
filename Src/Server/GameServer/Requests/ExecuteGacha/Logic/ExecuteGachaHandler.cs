using System.Buffers;
using Newtonsoft.Json;
using System.Text;
using Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;

namespace Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.Logic
{
    public class ExecuteGachaHandler
    {
        static (string, int) DropYokai(Dictionary<string, List<(float, int)>> bk_set)
        {
            double x = Random.Shared.NextDouble();
            float sum = 0;

            foreach (var kvp in bk_set)
            {
                string rank = kvp.Key;
                List<(float, int)> liste = kvp.Value;

                foreach (var tuple in liste)
                {
                    sum += tuple.Item1;
                    if (sum > x)
                    {
                        return (rank, tuple.Item2);
                    }
                }
            }

            return ("Uz", 9002294);
        }

        static int SoulLevelFormula(int n)
        {
            if (n == 0)
                return 0;
            return Math.Max(1, (n * (n + 1)) / 3) * 1000;
        }
        static int SoulLevel(int n)
        {
            int points = 0;
            for (int i = 1; i < 7; i++)
            {
                points += SoulLevelFormula(i);
                if (n < points)
                    return i;
            }
            return 7;
        }
        static int SoulPoints(int n)
        {
            int points = 0;
            for (int i = 1; i <= n; i++)
            {
                points += SoulLevelFormula(i);
            }
            return points;
        }

        static SkillResult? compute_skill_pctg(TableParser<YwpUserYoukaiSkill> youkaiList, long YoukaiId)
        {
            var UserYoukaiIndex = YoukaiManager.GetYoukaiSkillIndex(youkaiList, YoukaiId);
            if (UserYoukaiIndex == -1)
            {
                return null;
            }
            else
            {
                int amuLevel = youkaiList.Items[UserYoukaiIndex].Level;
                SkillResult? res = new();
                res.isMaxLevel = false;
                res.Before.Level = amuLevel;
                res.Before.Exp = youkaiList.Items[UserYoukaiIndex].Points;
                res.Before.ExpBar.Denominator = youkaiList.Items[UserYoukaiIndex].PercentageDenominator;
                res.Before.ExpBar.Numerator = youkaiList.Items[UserYoukaiIndex].PercentageNumerator;
                res.Before.ExpBar.Percentage = youkaiList.Items[UserYoukaiIndex].Percentage;

                if (amuLevel >= 7)
                {
                    res.After.Level = res.Before.Level;
                    res.After.Exp = res.Before.Exp;
                    res.After.ExpBar.Denominator = res.Before.ExpBar.Denominator;
                    res.After.ExpBar.Numerator = res.Before.ExpBar.Numerator;
                    res.After.ExpBar.Percentage = res.Before.ExpBar.Percentage;
                    res.isMaxLevel = true;
                    return res;
                }

                int current_points = res.Before.Exp;
                int new_points = 1000 + current_points;
                res.After.Exp = new_points;

                int new_level = SoulLevel(new_points);

                int denominator = SoulLevelFormula(new_level);
                int numerator = new_points - SoulPoints(new_level - 1);
                int percentage = (int)(((double)numerator / (double)denominator) * 100);

                res.After.Level = new_level;
                res.After.ExpBar.Denominator = denominator;
                res.After.ExpBar.Numerator = numerator;
                res.After.ExpBar.Percentage = percentage;

                return res;
            }
        }
        static List<(int, int)> Crank(Dictionary<string, List<(float, int)>> bk_set, List<(float, int)> ball_set, int n)
        {
            List<(string, int)> droppedYokai = new List<(string, int)>();

            for (int i = 0; i < n; i++)
            {
                droppedYokai.Add(DropYokai(bk_set));
            }

            List<(int, int)> List = new List<(int, int)>();



            foreach (var tuple in droppedYokai)
            {
                double x = Random.Shared.NextDouble();
                float sum = 0f;

                foreach (var ball_tuple in ball_set)
                {
                    sum += ball_tuple.Item1;
                    if (sum >= x) // >= au lieu de >
                    {
                        List.Add((ball_tuple.Item2, tuple.Item2));
                        break;
                    }
                }
            }


            return List;
        }

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
            pullCount = int.Parse(GachaMstTable.Table[gachaIndex][11]); ;
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

            // Configuration du système de gacha
            var bk_set1 = new Dictionary<string, List<(float, int)>>()
            {
                ["Uz+"] = new List<(float, int)>
                {
                    (50.0f, 9002630)
                },
                /*["Uz"] = new List<(float, int)>
                {
                    (0.005f, 9002321),
                    (0.005f, 9002320)
                },
                ["ZZZ"] = new List<(float, int)>
                {
                    (0.01f, 9002324),
                    (0.01f, 9002323)
                },
                ["ZZ"] = new List<(float, int)>
                {
                    (0.025f, 9001603),
                    (0.025f, 9001490),
                    (0.025f, 9001729),
                    (0.025f, 9001771),
                    (0.025f, 9001619),
                    (0.025f, 9001854),
                    (0.025f, 9001778),
                    (0.025f, 9001687)
                },
                ["Z"] = new List<(float, int)>
                {
                    (0.0167f, 9001511),
                    (0.0167f, 9001915),
                    (0.0167f, 9000866),
                    (0.0167f, 9000598),
                    (0.0167f, 9000727),
                    (0.0167f, 9001242),
                    (0.0167f, 9001691),
                    (0.0167f, 9001697),
                    (0.0167f, 9001878),
                    (0.0167f, 9000853),
                    (0.0167f, 9002004),
                    (0.0167f, 9001466),
                    (0.0167f, 9001894),
                    (0.0167f, 9000760),
                    (0.0167f, 9001772)
                },
                ["SSS"] = new List<(float, int)>
                {
                    (0.0194f, 9001053),
                    (0.0194f, 9001151),
                    (0.0194f, 9001468),
                    (0.0194f, 9000581),
                    (0.0194f, 9000763),
                    (0.0194f, 9000814),
                    (0.0194f, 9000943),
                    (0.0194f, 9001433),
                    (0.0194f, 9000668),
                    (0.0194f, 9000726),
                    (0.0194f, 9000944),
                    (0.0194f, 9001179),
                    (0.0194f, 9000706),
                    (0.0194f, 9001469),
                    (0.0194f, 9001497),
                    (0.0194f, 9000645),
                    (0.0194f, 9000870),
                    (0.0194f, 9001677)
                },
                ["SS"] = new List<(float, int)>
                {
                    (0.0267f, 9000421),
                }*/
            };
            // bk_set1["UZ+"] = new List<(float, int)>();
            // bk_set1["UZ"] = new List<(float, int)>();
            // bk_set1["ZZZ"] = new List<(float, int)>();

            // // Ajout des yokais avec leurs probabilités

            // bk_set1["ZZZ"].Add((0.3f, 9002354));
            // bk_set1["UZ"].Add((0.2f, 9002294));
            // bk_set1["UZ"].Add((0.2f, 9002354));
            // bk_set1["UZ"].Add((0.2f, 9002294));
            // bk_set1["UZ+"].Add((0.1f, 9002294));

            // Points requis pour chaque niveau d’amultime (exemple : ajustable si tu trouves les vraies valeurs)
            // int[] AMU_REQUIREMENTS = { 
            //     2000, // Amu1 -> 2
            //     4000, // Amu2 -> 3
            //     6000, // Amu3 -> 4
            //     8000, // Amu4 -> 5
            //     10000,// Amu5 -> 6
            //     12000 // Amu6 -> 7
            // };


            // Configuration des couleurs de capsules
            List<(float, int)> ball_set1 = new List<(float, int)>
            {
                /*(0.6f, 1),
                (0.5f, 2),
                (0.4f, 3),
                (0.3f, 4),
                (0.2f, 5),*/
                (1.0f, 6),
            };


            var prizes = new List<GachaPrize>();
            var youkaiIds = new List<int>();
            var capsuleIds = new List<int>();
            // Tous les yo-kai déjà possédés


            const int NEW_GETTYPE = 10;           // valeur observée dans ta sauvegarde pour "nouveau"
            const int DUPLICATE_GETTYPE = 2;      // valeur choisie pour signaler doublon (modifie si besoin)
            //const int DUPLICATE_REWARD_ITEM_ID = 20506; // exemple d'item donné en cas de doublon



            if (deserialized.RequestYoukaiId == 0)
            {
                List<(int, int)> bk_result = Crank(bk_set1, ball_set1, pullCount);
                foreach (var t in bk_result)
                {
                    capsuleIds.Add(t.Item1);
                    youkaiIds.Add(t.Item2);
                }
            }
            else
            {
                youkaiIds.Add(deserialized.RequestYoukaiId);
                capsuleIds.Add(5);
                pullCount = 1;
            }

            for (int i = 0; i < pullCount; i++)
            {
                int youkaiId = youkaiIds[i];

                int getType = DUPLICATE_GETTYPE;

                if (YoukaiManager.GetYoukaiIndex(UserYoukaiTable, youkaiId) < 0)
                {
                    getType = NEW_GETTYPE;
                }


                SkillResult? skill = compute_skill_pctg(UserYoukaiSkillTable, youkaiId);

                YoukaiManager.AddYoukai(UserYoukaiTable, youkaiId, UserYoukaiSkillTable);
                
                DictionaryListTable = DictionaryManager.EditDictionary(DictionaryListTable, youkaiId, false, true);

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
                        YoukaiId = youkaiIds[i],
                        ReleaseType = 0,
                        Skill = skill,
                        ExchgYmoney = 0,
                        Exp = 0,
                        LimitLevelBefore = 0,
                        LimitLevelAfter = 0,
                        ReleaseLevelType = 0

                    },
                    RarityType = 7,
                    ConvertItemInfo = null

                });

            }

            resp.GachaPrizeList = prizes.ToArray();


            /*
            var updates = new Dictionary<string, string>
            {
                ["ywp_user_dictionary"] = SupabaseManager.BuildRawFromMap(dictMap),
                ["ywp_user_youkai_skill"] = SupabaseManager.BuildRawFromMap(skillMap),
                ["ywp_user_youkai_bonus_effect"] = SupabaseManager.BuildRawFromMap(bonusMap),
                ["ywp_user_youkai_strong_skill"] = SupabaseManager.BuildRawFromMap(strongMap)
            };

            await SupabaseManager.UpdateYwpFields(userId, updates);*/

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

            var resdict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(resp))!;
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
