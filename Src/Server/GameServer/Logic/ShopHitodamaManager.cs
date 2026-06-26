using Newtonsoft.Json.Linq;
using Puniemu.Src.DataManager.Logic;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.TableParser.Logic;

namespace Puniemu.Src.Server.GameServer.Logic
{
    public static class ShopHitodamaManager
    {
        private static GameDataManager gm = DataManager.Logic.DataManager.GameDataManager;
        public static List<YwpMstShopHitodama> Data = JObject.Parse(gm.GamedataCache["ywp_mst_shop_hitodama_list"])["data"].ToObject<List<YwpMstShopHitodama>>();
    }
}
