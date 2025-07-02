using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
using System.Xml.Linq;
namespace Puniemu.Src.Server.GameServer.Requests.UserInfoRefresh.DataClasses
{
    public class UserInfoRefreshResponse : PuniResponse
    {

        // Basic user data.
        [JsonProperty("ywp_user_data")]
        public YwpUserData UserData { get; set; }

        public UserInfoRefreshResponse(YwpUserData userData)
        {
            ResultCode = 0;
            NextScreenType = 0;
            UserData = userData;
            ResultType = 0;
        }
        public Dictionary<string, object> ToDictionary()
        {
            var dict = new Dictionary<string, object>
            {
                    { nameof(ShopSaleList), ShopSaleList! },
                    { nameof(ServerDate), ServerDate },
                    { nameof(YwpToken), YwpToken },
                    { nameof(YMoneyShopSaleList), YMoneyShopSaleList },
                    { nameof(MstVersionMaster), MstVersionMaster },
                    { nameof(ResultCode), ResultCode },
                    { nameof(NextScreenType), NextScreenType },
                    { nameof(HitodamaShopSaleList), HitodamaShopSaleList! },
                    { nameof(UserData), UserData },
                    { nameof(ResultType), ResultType }
                
            };

            return dict;
        }
    }
}
