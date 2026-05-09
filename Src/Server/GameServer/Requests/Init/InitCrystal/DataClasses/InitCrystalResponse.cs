using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.InitCrystal.DataClasses
{
    public class InitCrystalResponse : CommonResponse
    {
        private JObject? _jo;

        public static async Task<InitCrystalResponse> BuildAsync(InitCrystalRequest request)
        {
            var inst = new InitCrystalResponse();
            
            // Créer la structure de base
            var jo = new JObject();
            
            // Remplir les données master (statiques)
            await PopulateMasterData(jo);
            
            // Mise à jour du timestamp
            jo["serverDt"] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            
            // Remplir ywp_user_data depuis Supabase
            await PopulateUserData(jo, request);
            
            // Initialiser les tables utilisateur d'event à vides
            InitializeUserEventTables(jo);
            
            inst._jo = jo;
            return inst;
        }

        private static async Task PopulateMasterData(JObject jo)
        {
            // Table master - données des menus Crystal
            jo["ywp_mst_crystal_menu"] = CreateCrystalMenuMaster();
            
            // Autres champs statiques
            jo["shopSaleList"] = new JArray();
            jo["ywpToken"] = "";
            jo["ymoneyShopSaleList"] = new JArray();
            jo["mstVersionMaster"] = 16774;
            jo["resultCode"] = 0;
            jo["ywp_user_icon_budge"] = "9000|5*5|1*2000|0*10|1*2007|1*2008|1*2009|1*2010|0*9004|21*9003|0*9007|0*9008|0*9009|0*10959|0*20959|0";
            jo["nextScreenType"] = 0;
            jo["dialogMsg"] = "";
            jo["hitodamaShopSaleList"] = new JArray();
            jo["webServerIp"] = "";
            jo["storeUrl"] = "";
            jo["dialogTitle"] = "";
            jo["resultType"] = 0;
        }

        private static async Task PopulateUserData(JObject jo, InitCrystalRequest request)
        {
            // Initialiser avec des données par défaut
            jo["ywp_user_data"] = new JObject
            {
                ["birthday"] = "",
                ["freeHitodama"] = 3,
                ["friendMaxCnt"] = 10,
                ["plateId"] = 1,
                ["medalPoint"] = 0,
                ["webCode"] = "y1zy12nfflmpxw6q",
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
                ["limitTimeSaleRemainSec"] = 63780,
                ["totMedalPoint"] = 0,
                ["eventPointUpItemRemainSec"] = 0,
                ["playerName"] = "superog",
                ["todaysRemainSec"] = 63781,
                ["weeklyFreeFlg"] = 1,
                ["hitodama"] = 0,
                ["lastRenameDt"] = 0,
                ["codenameId"] = 1,
                ["usingItemList"] = new JArray(),
                ["hitodamaRecoverSec"] = 240,
                ["limitTimeSaleEndDt"] = "2025-09-15 23:59:59",
                ["equipWatchId"] = 10101
            };

            // Tenter de récupérer les vraies données utilisateur depuis Supabase
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
                            } else
                            {
                                jo["ywp_user_data"] = null;
                            }
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log l'erreur si nécessaire mais continuer avec les données par défaut
                Console.WriteLine($"Erreur lors de la récupération des données utilisateur: {ex.Message}");
            }
        }

        private static void InitializeUserEventTables(JObject jo)
        {
            // Tables utilisateur d'événement - initialisées vides pour un nouvel utilisateur
            jo["ywp_user_crystal_menu"] = new JArray();
        }

        private static JArray CreateCrystalMenuMaster()
        {
            var menus = new JArray();
            
            var menuData = new[]
            {
                new { youkaiId = 2927000, crystalMenuId = 1, webViewName = "CGet_bushinyan", openDt = CreateDateTime(2018, 3, 1) },
                new { youkaiId = 9000294, crystalMenuId = 2, webViewName = "CGet_yamabukioni", openDt = CreateDateTime(2018, 7, 17) },
                new { youkaiId = 9000263, crystalMenuId = 3, webViewName = "CGet_netabarerina", openDt = CreateDateTime(2018, 6, 16) },
                new { youkaiId = 9000313, crystalMenuId = 4, webViewName = "CGet_hanasakajii", openDt = CreateDateTime(2018, 9, 17) },
                new { youkaiId = 9000379, crystalMenuId = 5, webViewName = "CGet_ikemenken", openDt = CreateDateTime(2019, 1, 17) },
                new { youkaiId = 9000380, crystalMenuId = 6, webViewName = "CGet_yamatan", openDt = CreateDateTime(2019, 1, 17) },
                new { youkaiId = 9000312, crystalMenuId = 7, webViewName = "CGet_shurakoma", openDt = CreateDateTime(2018, 9, 17) },
                new { youkaiId = 9000394, crystalMenuId = 8, webViewName = "CGet_unchikuma", openDt = CreateDateTime(2019, 2, 15) }
            };

            var closeDt = CreateDateTime(8099, 12, 31, 23, 59, 59);

            foreach (var menu in menuData)
            {
                menus.Add(new JObject
                {
                    ["youkaiId"] = menu.youkaiId,
                    ["openDt"] = menu.openDt,
                    ["crystalMenuId"] = menu.crystalMenuId,
                    ["webViewName"] = menu.webViewName,
                    ["closeDt"] = closeDt
                });
            }

            return menus;
        }

        private static JObject CreateDateTime(int year, int month, int day, int hour = 0, int minute = 0, int second = 0)
        {
            long unixTime;
            int dayOfWeek;
            
            // Pour les dates futures impossibles (comme 8099), on utilise une valeur hardcodée
            if (year >= 3000)
            {
                unixTime = 253402268399000; // Valeur du JSON original pour 8099
                dayOfWeek = 5; // Vendredi (valeur du JSON original)
            }
            else
            {
                var dt = new DateTime(year, month, day, hour, minute, second);
                unixTime = ((DateTimeOffset)dt).ToUnixTimeMilliseconds();
                dayOfWeek = (int)dt.DayOfWeek;
            }
            
            return new JObject
            {
                ["date"] = day,
                ["day"] = dayOfWeek,
                ["hours"] = hour,
                ["minutes"] = minute,
                ["month"] = month - 1, // JavaScript months are 0-indexed
                ["seconds"] = second,
                ["time"] = unixTime,
                ["timezoneOffset"] = -540, // JST timezone offset
                ["year"] = year - 1900 // JavaScript years are offset by 1900
            };
        }

        public async Task<Dictionary<string, object>> ToDictionary(string gdkey)
        {
            // Convert the JObject to a Dictionary suitable for serialisation/chiffrement
            return _jo!.ToObject<Dictionary<string, object>>()!;
        }
    }
}