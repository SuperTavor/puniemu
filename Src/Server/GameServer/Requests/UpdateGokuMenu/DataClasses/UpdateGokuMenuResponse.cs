using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.UpdateGokuMenu.DataClasses
{
    public class UpdateGokuMenuResponse : CommonResponse
    {
        private JObject? _jo;

        public static async Task<UpdateGokuMenuResponse> BuildAsync(UpdateGokuMenuRequest request)
        {
            var inst = new UpdateGokuMenuResponse();
            var jo = new JObject();

            // Champs statiques
            PopulateStaticData(jo);

            // Timestamp
            jo["serverDt"] = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            // Charger les données utilisateur depuis Supabase
            await PopulateUserData(jo, request);

            // Mettre à jour le menu Goku utilisateur
            await UpdateUserGokuMenu(jo, request);

            // Mettre à jour aussi le youkai_intro_release
            await UpdateUserGokuYoukaiIntroRelease(jo, request);

            inst._jo = jo;
            return inst;
        }

        private static void PopulateStaticData(JObject jo)
        {
            jo["shopSaleList"] = new JArray();
            jo["ywpToken"] = "";
            jo["ymoneyShopSaleList"] = new JArray();
            jo["mstVersionMaster"] = 16774;
            jo["resultCode"] = 0;
            jo["ywp_user_icon_budge"] =
                "9000|5*5|1*2000|0*10|1*2007|1*2008|1*2009|1*2010|0*9004|21*9003|0*9007|0*9008|0*9009|0*10959|0*20959|0";
            jo["nextScreenType"] = 0;
            jo["dialogMsg"] = "";
            jo["hitodamaShopSaleList"] = new JArray();
            jo["webServerIp"] = "";
            jo["storeUrl"] = "";
            jo["dialogTitle"] = "";
            jo["resultType"] = 0;
        }

        private static async Task PopulateUserData(JObject jo, UpdateGokuMenuRequest request)
        {
            // Valeurs par défaut
            jo["ywp_user_data"] = new JObject
            {
                ["birthday"] = "",
                ["freeHitodama"] = 3,
                ["friendMaxCnt"] = 10,
                ["plateId"] = 1,
                ["medalPoint"] = 0,
                ["nowStageId"] = 1001006,
                ["titleId"] = 1,
                ["gokuCollectCnt"] = 0,
                ["ymoney"] = 3285,
                ["reviewFlg"] = 0,
                ["moveReason"] = 0,
                ["chargeYmoney"] = 0,
                ["effectId"] = 1,
                ["crystalCollectCnt"] = 0,
                ["eventPointUpItemId"] = 0,
                ["characterId"] = "3qjiw6vi",
                ["iconId"] = 1,
                ["limitTimeSaleRemainSec"] = 63646,
                ["totMedalPoint"] = 0,
                ["eventPointUpItemRemainSec"] = 0,
                ["playerName"] = "superog",
                ["todaysRemainSec"] = 63647,
                ["weeklyFreeFlg"] = 1,
                ["hitodama"] = 0,
                ["lastRenameDt"] = 0,
                ["codenameId"] = 1,
                ["usingItemList"] = new JArray(),
                ["hitodamaRecoverSec"] = 106,
                ["limitTimeSaleEndDt"] = "2025-09-15 23:59:59",
                ["equipWatchId"] = 10101
            };

            try
            {
                if (!string.IsNullOrEmpty(request?.DeviceId))
                {
                    var gdkeys = await Puniemu.Src.UserDataManager.Logic.DBService.GetGdkeysFromUdkeyAsync(request.DeviceId);
                    if (gdkeys != null && gdkeys.Count > 0)
                    {
                        var tables = (await Puniemu.Src.UserDataManager.Logic.DBService.GetEntireUserData(gdkeys[0]))!;
                        if (tables != null && tables.ContainsKey("ywp_user_data"))
                        {
                            if (tables["ywp_user_data"] != null)
                            {
                                jo["ywp_user_data"] = JObject.FromObject(tables["ywp_user_data"]!);
                            } else {
                                jo["ywp_user_data"] = null;
                            }
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] PopulateUserData error: {ex.Message}");
            }
        }

        private static async Task UpdateUserGokuMenu(JObject jo, UpdateGokuMenuRequest request)
        {
            var userMenu = new JArray();

            if (request?.GokuMenuId > 0)
            {
                userMenu.Add(new JObject
                {
                    ["gokuMenuId"] = request.GokuMenuId
                });
            }

            try
            {
                if (!string.IsNullOrEmpty(request?.DeviceId))
                {
                    var gdkeys = await Puniemu.Src.UserDataManager.Logic.DBService.GetGdkeysFromUdkeyAsync(request.DeviceId);
                    if (gdkeys != null && gdkeys.Count > 0)
                    {
                        var tables = await Puniemu.Src.UserDataManager.Logic.DBService.GetEntireUserData(gdkeys[0]);
                        if (tables != null && tables.ContainsKey("ywp_user_goku_menu"))
                        {
                            var existing = tables["ywp_user_goku_menu"] as JArray ?? new JArray();
                            userMenu = existing;

                            if (request?.GokuMenuId > 0)
                            {
                                bool exists = userMenu.Any(x =>
                                    x["gokuMenuId"]?.Value<int>() == request.GokuMenuId);
                                if (!exists)
                                {
                                    userMenu.Add(new JObject
                                    {
                                        ["gokuMenuId"] = request.GokuMenuId
                                    });
                                }
                            }
                        }

                        await Puniemu.Src.UserDataManager.Logic.DBService.SetYwpUserAsync(gdkeys[0], "ywp_user_goku_menu",
                            userMenu.ToObject<List<object>>()!);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] UpdateUserGokuMenu error: {ex.Message}");
            }

            jo["ywp_user_goku_menu"] = userMenu;
        }

        private static async Task UpdateUserGokuYoukaiIntroRelease(JObject jo, UpdateGokuMenuRequest request)
        {
            var introReleaseArray = new JArray();

            try
            {
                if (!string.IsNullOrEmpty(request?.DeviceId))
                {
                    var gdkeys = await Puniemu.Src.UserDataManager.Logic.DBService.GetGdkeysFromUdkeyAsync(request.DeviceId);
                    if (gdkeys != null && gdkeys.Count > 0)
                    {
                        var tables = await Puniemu.Src.UserDataManager.Logic.DBService.GetEntireUserData(gdkeys[0]);
                        if (tables != null && tables.ContainsKey("ywp_user_goku_youkai_intro_release"))
                        {
                            introReleaseArray =
                                tables["ywp_user_goku_youkai_intro_release"] as JArray ?? new JArray();
                        }

                        if (request?.GokuMenuId > 0)
                        {
                            int introId = request.GokuMenuId * 1000 + 1;

                            bool exists = introReleaseArray.Any(x =>
                                x["introReleaseId"]?.Value<int>() == introId);

                            if (!exists)
                            {
                                introReleaseArray.Add(new JObject
                                {
                                    ["introReleaseId"] = introId,
                                    ["userId"] = request.UserId ?? "",
                                    ["clearFlg"] = 1,
                                    ["readFlg"] = 0,
                                    ["missionType"] = 1,
                                    ["nowValue"] = 0,
                                    ["targetValue"] = 0,
                                    ["update"] = false,
                                    ["updateClearFlg"] = 0,
                                    ["updateNowValue"] = 0,
                                    ["updateReadFlg"] = 0,
                                    ["createRecord"] = false
                                });
                            }

                            await Puniemu.Src.UserDataManager.Logic.DBService.SetYwpUserAsync(gdkeys[0],
                                "ywp_user_goku_youkai_intro_release",
                                introReleaseArray.ToObject<List<object>>()!);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] UpdateUserGokuYoukaiIntroRelease error: {ex.Message}");
            }

            jo["ywp_user_goku_youkai_intro_release"] = introReleaseArray;
        }

        public async Task<Dictionary<string, object>> ToDictionary(string gdkey)
        {
            return _jo!.ToObject<Dictionary<string, object>>()!;
        }
    }
}
