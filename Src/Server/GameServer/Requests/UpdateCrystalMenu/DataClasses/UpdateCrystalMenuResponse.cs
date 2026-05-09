using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.DBService.Logic;

namespace Puniemu.Src.Server.GameServer.Requests.UpdateCrystalMenu.DataClasses
{
    public class UpdateCrystalMenuResponse : CommonResponse
    {
        private JObject _jo = new JObject();

        public static async Task<UpdateCrystalMenuResponse> BuildAsync(UpdateCrystalMenuRequest request)
        {
            var inst = new UpdateCrystalMenuResponse();

            inst._jo["shopSaleList"] = new JArray();
            inst._jo["ywpToken"] = "";
            inst._jo["ymoneyShopSaleList"] = new JArray();
            inst._jo["mstVersionMaster"] = 16774;
            inst._jo["resultCode"] = 0;
            inst._jo["ywp_user_icon_budge"] = "";
            inst._jo["nextScreenType"] = 0;
            inst._jo["dialogMsg"] = "";
            inst._jo["hitodamaShopSaleList"] = new JArray();
            inst._jo["webServerIp"] = "";
            inst._jo["storeUrl"] = "";
            inst._jo["dialogTitle"] = "";
            inst._jo["resultType"] = 0;

            // serverDt
            inst._jo["serverDt"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // user data 
            inst._jo["ywp_user_data"] = new JObject
            {
                ["playerName"] = "random",
                ["iconId"] = 1,
                ["userId"] = request?.UserId ?? "0",
                ["characterId"] = request?.Level5UserId ?? ""
            };

            // crystal menu
            var crystalMenu = new JArray();
            if (request?.CrystalMenuId != null)
            {
                crystalMenu.Add(new JObject { ["crystalMenuId"] = request.CrystalMenuId });
            }
            inst._jo["ywp_user_crystal_menu"] = crystalMenu;

            return inst;
        }

        public async Task<Dictionary<string, object>> ToDictionary(string gdkey)
        {
            return _jo.ToObject<Dictionary<string, object>>()!;
        }
    }
}
