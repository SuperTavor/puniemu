using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses.Watch;
using Puniemu.Src.Server.GameServer.Requests.Watch.DataClasses;
using System.Text;
using System.Buffers;
using Puniemu.Src.Server.GameServer.Requests.Watch.UpdateWatchReadFlg.DataClasses;
namespace Puniemu.Src.Server.GameServer.Requests.Watch.UpdateWatchReadFlg.Logic
{
    public class UpdateWatchReadFlgHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);

            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<UpdateWatchReadFlgRequest>(requestJsonString!);


            var userWatch = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<YwpUserWatch>>(deserialized.Level5UserID, "ywp_user_watch");
            var watchItem = userWatch.FirstOrDefault(x => x.WatchID == deserialized.WatchID);
            if(watchItem == null)
            {
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(new MsgBoxResponse("Watch not found", "Error"))));
                return;
            }
            watchItem.ReadFlag = 1;
            var res = new CommonWatchResponse()
            {
                YwpUserData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID, "ywp_user_data"),
                YwpUserWatch = userWatch
            };
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_watch", userWatch);
            ctx.Response.StatusCode = 200;
            await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));
        }
    }
}
