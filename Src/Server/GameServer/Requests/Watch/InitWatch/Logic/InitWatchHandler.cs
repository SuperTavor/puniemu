using Newtonsoft.Json;
using System.Text;
using System.Buffers;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.Watch.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses.Watch;
namespace Puniemu.Src.Server.GameServer.Requests.Watch.InitWatch.Logic
{
    public class InitWatchHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);

            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<CommonRequest>(requestJsonString!);
            var res = new CommonWatchResponse()
            {
                YwpUserData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID, "ywp_user_data"),
                YwpUserWatch = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<YwpUserWatch>>(deserialized.Level5UserID, "ywp_user_watch")
            };

            ctx.Response.StatusCode = 200;
            await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));
        }
    }
}
