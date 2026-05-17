using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.InitGacha.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses.Gacha.GachaStamp;
using System.Buffers;
using System.Text;
using Puniemu.Src.Server.GameServer.Requests.Init.InitGacha.DataClasses.WibWob;

namespace Puniemu.Src.Server.GameServer.Requests.Init.InitGacha.Logic.WibWob
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
            var res = await InitGachaResponse.ConstructAsync(deserialized.GdKey);

            var outResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res));
            await ctx.Response.WriteAsync(outResponse);

        }
    }
}
