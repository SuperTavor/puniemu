using Puniemu.Src.Server.L5ID.API.V1.CreateGDKey.DataClasses;
using Puniemu.Src.Server.L5ID.DataClasses;

namespace Puniemu.Src.Server.L5ID.Requests.CreateGDKey.Logic
{
    public class CreateGDKeyHandler
    {
        public async static Task HandleAsync(HttpContext ctx)
        {
            var query = ctx.Request.Query.ToDictionary();
            ctx.Response.Headers.ContentType = "application/json";
            if (!query.ContainsKey("udkey"))
            {
                var badResponse = new BadL5IDResponse(L5IDErr.UNKNOWN_UDKEY, "Unknown UDKey");
                await ctx.Response.WriteAsJsonAsync(badResponse);
            }
            else
            {
                var gdkey = Key.GenerateKey("g-");
                await UserDataManager.Logic.UserDataManager.RegisterGdKeyInUdKeyAsync(query["udkey"]!, gdkey);
                var res = new CreateGDKeyGoodResponse(gdkey);
                await ctx.Response.WriteAsJsonAsync(res);
            }
        }
    }
}
