using Newtonsoft.Json;
using Puniemu.Src.Utils.GeneralUtils;
using System;
using System.Collections.Generic;

namespace Puniemu.Src.Server.GameServer.Requests.GetMaster.DataClasses
{
    public static class BaseMasterDataBuilder
    {
        public static Dictionary<string, object?> Build()
        {
            var result = new Dictionary<string, object?>
            {
                ["serverDt"] = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                ["resultType"] = 0,
                ["shopSaleList"] = GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("shopSaleList"),
                ["dialogTitle"] = "",
                ["resultCode"] = 0,
                ["hitodamaShopSaleList"] = GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("hitodamaShopSaleList"),
                ["ywpToken"] = "",
                ["token"] = null,
                ["storeUrl"] = "",
                ["webServerIp"] = "",
                ["mstVersionMaster"] = int.Parse(DataManager.Logic.DataManager.GameDataManager!.GamedataCache["mstVersionMaster"]),
                ["nextScreenType"] = 0,
                ["ymoneyShopSaleList"] = GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("ymoneyShopSaleList"),
                ["dialogMsg"] = ""
            };

            return result;
        }
    }
}
