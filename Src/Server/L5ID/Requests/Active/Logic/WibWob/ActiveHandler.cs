using Puniemu.Src.Server.L5ID.DataClasses;
using Newtonsoft.Json;
using Puniemu.Src.Server.L5ID.Requests.Active.DataClasses;
using Puniemu.Src.UserDataManager.Logic;
namespace Puniemu.Src.Server.L5ID.Requests.Active.Logic.WibWob
{
    //This call is seeimgly used to check the validity of udkeys and gdkeys on the L5id server.
    public class ActiveHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            /*
            WibWob seemingly does not send UDKey in the active call for some odd reason, with the HSP patch we applied at least.
            To fix this, we spoof the udkey in the TICKET value using a Frida script inside the game.
            This ONLY works on WibWob Reloaded APKs with the Frida script, not normal WibWob.
            */
            var queryParams = ctx.Request.Query;
            ctx.Response.ContentType = "application/json";
            string udkey = queryParams["TICKET"]!;
            if(!(await UserDataManager.Logic.UserDataManager.IsDeviceExists(udkey)))
            {
                await UserDataManager.Logic.UserDataManager.NewDeviceAsync(udkey);
            }
            var res = await ActiveResponse.CreateAsync(udkey);
            var json = JsonConvert.SerializeObject(res);
            await ctx.Response.WriteAsync(json);
        }
    }
}
