using Puniemu.src.Utils.UserDataManager;
using Puniemu.Src.Server.L5ID.API.V1.CreateGDKey.DataClasses;
using Puniemu.Src.Server.L5ID.DataClasses;

namespace Puniemu.Src.Server.L5ID.API.V1.CreateGDKey
{
    public class CCreateGDKeyHandler
    {
        public async static Task HandleAsync(HttpContext ctx)
        {
            var query = ctx.Request.Query.ToDictionary();
            ctx.Response.Headers.ContentType = "application/json";
            if(!query.ContainsKey("udkey"))
            {
                var badResponse = new SBadResponse(L5IDErr.UNKNOWN_UDKEY,"Unknown UDKey");
                await ctx.Response.WriteAsJsonAsync(badResponse);
            }
            else
            {
                var gdkey = CKey.GenerateKey("g-");
                await CUserDataManager.RegisterGdKeyInUdKeyAsync(query["udkey"]!, gdkey);
                var res = new SCreateGDKeyGoodResponse(gdkey);
                await ctx.Response.WriteAsJsonAsync(res);
            }
        }
    }
}
