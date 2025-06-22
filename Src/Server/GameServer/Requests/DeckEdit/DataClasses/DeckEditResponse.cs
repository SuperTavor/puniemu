using Newtonsoft.Json;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.Server.GameServer.DataClasses;
namespace Puniemu.Src.Server.GameServer.Requests.DeckEdit.DataClasses
{
    public class DeckEditResponse : PuniResponse
    {

        public DeckEditResponse()
        {
            ResultCode = 0;
            ResultType = 0;
            NextScreenType = 0;
        }
        public Task<Dictionary<string, object>> ToDictionary()
        {
            var dict = new Dictionary<string, object>
            {
                { "serverDt", ServerDate },
                { "mstVersionMaster", MstVersionMaster },
                { "resultCode", ResultCode },
                { "resultType", ResultType },
                { "nextScreenType", NextScreenType },
                { "ymoneyShopSaleList", YMoneyShopSaleList },
                { "ywpToken", YwpToken },
                { "token", Token },
                { "dialogMsg", DialogMsg }
            };
            return Task.FromResult(dict);
        }

    }
}

