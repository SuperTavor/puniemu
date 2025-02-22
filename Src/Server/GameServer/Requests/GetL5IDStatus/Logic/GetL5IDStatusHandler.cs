using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.GetL5IDStatus.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.UpdateTutorialFlag.DataClasses;
using System.Buffers;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Requests.GetL5IDStatus.Logic
{
    public class GetL5IDStatusHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            //we can literally ignore the request as we are sending static data excluding the normal dynamic values such as shoplists.
            //we do this bc we dont support l5id so we give like 99999 l5 points so ppl can get pandanoko and shi
            var res = new GetL5IDStatusResponse();
            await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));

        }
    }
}
