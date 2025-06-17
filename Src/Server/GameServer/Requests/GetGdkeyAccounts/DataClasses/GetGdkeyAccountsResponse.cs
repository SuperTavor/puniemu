using Newtonsoft.Json;
using Puniemu.Src.ConfigManager.Logic;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.UserDataManager.Logic;

namespace Puniemu.Src.Server.GameServer.Requests.GetGdkeyAccounts.DataClasses
{
    public class GetGdkeyAccountsResponse: PuniemuResponseBase
    {
        [JsonProperty("udkeyPlayerList")]
        public List<UdkeyPlayerItem>? UDKeyPlayerList { get; set; } // List of UDKeyPlayerItem


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
            res.UDKeyPlayerList = playerItems;
            res.ResultCode = 0;
            res.ResultType = 0;

            return res;

        }

    }
}
