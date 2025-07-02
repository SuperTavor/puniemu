using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.InitGacha.DataClasses;
using System.Buffers;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Requests.InitGacha.Logic
{
    public class InitGachaHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<InitGachaRequest>(requestJsonString!);

        }
    }
}
