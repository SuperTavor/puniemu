using Newtonsoft.Json;
using Puniemu.Src.ConfigManager;
using Puniemu.Src.Utils.GeneralUtils;
using System;
using System.Collections.Generic;

namespace Puniemu.Src.Server.GameServer.GetMaster.DataClasses
{
    public static class CBaseMasterDataBuilder
    {
        public static Dictionary<string, object?> Build()
        {
            var result = new Dictionary<string, object?>
            {
                ["serverDt"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                ["resultType"] = 0,
                ["shopSaleList"] = CGeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("shopSaleList"),
                ["dialogTitle"] = "",
                ["resultCode"] = 0,
                ["hitodamaShopSaleList"] = CGeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("hitodamaShopSaleList"),
                ["ywpToken"] = "",
                ["token"] = null,
                ["storeUrl"] = "",
                ["webServerIp"] = "",
                ["mstVersionMaster"] = int.Parse(CConfigManager.GameDataManager.GamedataCache["mstVersionMaster"]),
                ["nextScreenType"] = 0,
                ["ymoneyShopSaleList"] = CGeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("ymoneyShopSaleList"),
                ["dialogMsg"] = ""
            };

            return result;
        }
    }
}
