using Newtonsoft.Json;
using Puniemu.Src.ConfigManager.Logic;
using Puniemu.Src.UserDataManager.Logic;

namespace Puniemu.Src.Server.GameServer.Requests.GetGdkeyAccounts.DataClasses
{
    public struct GetGdkeyAccountsResponse
    {
        [JsonProperty("serverDt")]
        public long ServerDt { get; set; } // Timestamp when the response was sent.

        [JsonProperty("ywpToken")]
        public string YWPToken { get; set; } // Empty here.

        [JsonProperty("mstVersionVer")]
        public int MstVersionVer { get; set; } // Version of assets on the server

        [JsonProperty("resultCode")]
        public int ResultCode { get; set; } // 0 here

        [JsonProperty("nextScreenType")]
        public int NextScreenType { get; set; } // 0 here

        [JsonProperty("udkeyPlayerList")]
        public List<UdkeyPlayerItem> UDKeyPlayerList { get; set; } // List of UDKeyPlayerItem

        [JsonProperty("resultType")]
        public int ResultType { get; set; } // 0 here

        //Cause async constructors are not allowed
        public static async Task<GetGdkeyAccountsResponse?> ConstructAsync(string udkey,List<string> gdkeys)
        {
            List<UdkeyPlayerItem> playerItems = new();
            if(!gdkeys.SequenceEqual(new List<string>()))
            {
                foreach (var gdkey in gdkeys)
                {
                    var item = await UdkeyPlayerItem.ConstructAsync(gdkey);
                    if (item == null)
                    {
                        await UserDataManager.Logic.UserDataManager.DeleteUser(udkey, gdkey);
                        return null;
                    }
                    playerItems.Add(item.Value);
                }
            }
            GetGdkeyAccountsResponse res = new();
            res.ServerDt = DateTimeOffset.Now.ToUnixTimeSeconds();
            res.YWPToken = string.Empty;
            res.UDKeyPlayerList = playerItems;
            res.MstVersionVer = int.Parse(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["mstVersionMaster"]);
            res.ResultCode = 0;
            res.ResultType = 0;

            return res;

        }

    }
}
