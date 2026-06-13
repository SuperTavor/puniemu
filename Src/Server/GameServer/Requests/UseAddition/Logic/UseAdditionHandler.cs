using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.UseAddition.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.UseItem.DataClasses;
using Puniemu.Src.UserDataManager.Logic;
using System.Buffers;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Requests.UseAddition.Logic
{
    public class UseAdditionHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<CommonRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";

            var res = new UseAdditionResponse();
            res.UserData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID, "ywp_user_data")!;

            var service = new UseAdditionService(deserialized.Level5UserID);

            if(await service.CanDoShrineToday())
            {
                //Super shrine bonus has 10% chance to happen
                var isSuper = Random.Shared.Next(10) == 0;

                if (isSuper)
                {
                    res.SetSuperShrine();
                    //only marking this because the normal shrine doesnt actually do anything
                    await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_addition", true);
                }
                else
                {
                    res.SetNormalShrine();
                }

                await service.MarkShrine();
            }
            else
            {
                res.SetUsedToday();
            }

            await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));
        }
    }
}
