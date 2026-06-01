namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class ShopLimitManager
    {
        public static async Task CheckShopLimitReset(string gdkey)
        {
            string todayStr = DateTime.UtcNow.ToString("yyyyMMdd");
            var lastResetDate = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(gdkey, "lastShopResetDate");

            if (lastResetDate != todayStr)
            {
                await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(gdkey, "ywp_user_shop_item_remain_cnt", "");

                await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(gdkey, "lastShopResetDate", todayStr);

            }
        }
    }
}
