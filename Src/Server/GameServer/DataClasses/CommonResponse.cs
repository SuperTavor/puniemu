using Newtonsoft.Json;
using Puniemu.Src.Utils.GeneralUtils;
using System.Reflection;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class CommonResponse
    {
        // The unix time when the response was sent.
        [JsonProperty("serverDt")]
        public long ServerDate = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        [JsonProperty("resultCode")]
        public int ResultCode { get; set; }

        [JsonProperty("nextScreenType")]
        public int NextScreenType { get; set; }

        [JsonProperty("resultType")]
        public int ResultType { get; set; }

        //Version of assets on the server.
        [JsonProperty("mstVersionMaster")]
        public int MstVersionMaster = int.Parse(DataManager.Logic.DataManager.GameDataManager!.GamedataCache["mstVersionMaster"]);
       
        // Constant.
        [JsonProperty("shopSaleList")]
        public List<int>? ShopSaleList { get; set; }

        // Constant.
        [JsonProperty("ymoneyShopSaleList")]
        public List<int> YMoneyShopSaleList = GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("ymoneyShopSaleList");

        // Constant.
        [JsonProperty("hitodamaShopSaleList")]
        public List<int>? HitodamaShopSaleList { get; set; }

        // Always empty.
        [JsonProperty("ywpToken")]
        public string YwpToken = "";

        [JsonProperty("token")]
        public string Token = "";

        [JsonProperty("dialogMsg")]
        public string DialogMsg = "";

        [JsonProperty("webServerIp")]
        public string WebServerIp = Consts.OG_GAMESERVER_URL;

        [JsonProperty("dialogTitle")]
        public string DialogTitle = "";

        [JsonProperty("storeUrl")]
        public string StoreUrl = "";

        [JsonProperty("mstVersionVer")]
        public int MstVersionVer { get; set; } // Version of assets on the server

        public async Task<Dictionary<string, object>> ToDictionary()
        {
            var dict = new Dictionary<string, object>();

            var type = GetType();

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (var field in fields)
            {
                var jsonProp = field.GetCustomAttribute<JsonPropertyAttribute>();
                if (jsonProp == null || string.IsNullOrWhiteSpace(jsonProp.PropertyName))
                    continue;

                var key = jsonProp.PropertyName;
                var value = field.GetValue(this);

                dict[key] = value!;
            }

            return dict;
        }

    }

    public class PuniRequest
    {
        // Client version
        [JsonProperty("appVer")]
        public string? AppVer { get; set; }

        // Equivalent to UDKey
        [JsonProperty("deviceID")]
        public string? DeviceID { get; set; }

        // Interchangeable with gdkey
        [JsonProperty("level5UserID")]
        public string? Level5UserID { get; set; }

        // Version of server data
        [JsonProperty("mstVersionVer")]
        public int MstVersionVer { get; set; }

        // OS type. 2 is Android.
        [JsonProperty("osType")]
        public int OsType { get; set; }

        // Always 0 here
        [JsonProperty("userID")]
        public string? UserID { get; set; }

        // Always 0
        [JsonProperty("ywpToken")]
        public string? YwpToken { get; set; }
    }
}
