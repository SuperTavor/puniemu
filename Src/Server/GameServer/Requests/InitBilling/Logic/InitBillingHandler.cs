using Puniemu.Src.Server.GameServer.DataClasses;
using Newtonsoft.Json;
namespace Puniemu.Src.Server.GameServer.Requests.InitBilling.Logic
{
    public class InitBillingHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            var res = new MsgBoxResponse("Puniemu does not support\npaid content. It is a non-profit,\nopen source project.", "Support NHN.");
            await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));
        }
    }
}
