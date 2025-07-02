using Puniemu.Src.Server.L5ID.Requests.CreateGDKey.DataClasses;
using Puniemu.Src.Server.L5ID.DataClasses;
using Newtonsoft.Json;

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
                var json = JsonConvert.SerializeObject(badResponse);
                await ctx.Response.WriteAsync(json);
            }
            else
            {
                var gdkey = await UserDataManager.Logic.UserDataManager.NewAccountAsync();
                await UserDataManager.Logic.UserDataManager.AddAccountToDevice(query["udkey"]!, gdkey);
                var res = new CreateGDKeyGoodResponse(gdkey);
                var json = JsonConvert.SerializeObject(res);
                await ctx.Response.WriteAsync(json);
            }
        }
    }
}
