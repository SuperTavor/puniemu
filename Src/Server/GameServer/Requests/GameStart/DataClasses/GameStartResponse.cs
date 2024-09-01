using Newtonsoft.Json;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.Server.GameServer.DataClasses;
namespace Puniemu.Src.Server.GameServer.Requests.GameStart.DataClasses
{
    public class GameStartResponse : PuniemuResponseBase
    {

        public GameStartResponse()
        {
            ResultCode = 0;
            ResultType = 0;
            NextScreenType = 0;
        }
        public async Task<Dictionary<string, object>> ToDictionary()
        {
            return new()
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
        }

    }
}

