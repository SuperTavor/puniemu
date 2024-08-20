using Puniemu.Src.Server.L5ID.API.V1.Active.DataClasses;
using Puniemu.Src.Server.L5ID.DataClasses;
using Newtonsoft.Json;
namespace Puniemu.Src.Server.L5ID.API.V1.Active
{
    //This call is seeimgly used to check the validity of udkeys and gdkeys on the L5id server.
    public class CActiveHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            var queryParams = ctx.Request.Query;
            ctx.Response.ContentType = "application/json";
            string udkey;
            if(!queryParams.ContainsKey("udkey"))
            {
                 udkey = CKey.GenerateKey("d-");
            }
            else
            {
                 udkey = queryParams["udkey"]!;
            }
            var res = await CGoodActiveResponse.CreateAsync(udkey);
            var json = JsonConvert.SerializeObject(res);
            await ctx.Response.WriteAsync(json);
        }
    }
}
