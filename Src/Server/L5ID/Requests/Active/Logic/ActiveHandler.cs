using Puniemu.Src.Server.L5ID.DataClasses;
using Newtonsoft.Json;
using Puniemu.Src.Server.L5ID.Requests.Active.DataClasses;
namespace Puniemu.Src.Server.L5ID.Requests.Active.Logic
{
    //This call is seeimgly used to check the validity of udkeys and gdkeys on the L5id server.
    public class ActiveHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            var queryParams = ctx.Request.Query;
            ctx.Response.ContentType = "application/json";
            string udkey;
            if (!queryParams.ContainsKey("udkey"))
            {
                udkey = Key.GenerateKey("d-");
            }
            else
            {
                udkey = queryParams["udkey"]!;
            }
            var res = await ActiveResponse.CreateAsync(udkey);
            var json = JsonConvert.SerializeObject(res);
            await ctx.Response.WriteAsync(json);
        }
    }
}
