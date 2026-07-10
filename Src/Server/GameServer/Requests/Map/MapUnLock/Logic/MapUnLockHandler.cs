using Newtonsoft.Json;
using Puniemu.Src.UserDataManager;

using Puniemu.Src.Server.GameServer.Requests.MapUnLock.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;
using System.Buffers;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.UserDataManager.Logic;
using Puniemu.Src.Server.GameServer.Requests.Map.MapUnLock.Logic;

namespace Puniemu.Src.Server.GameServer.Requests.MapUnLock.Logic
{
    public class MapUnLockHandler
    {
        //Puni puni now have a new profile system (with plate, effect and codename) but for some reason in the private server it's always the old app
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<MapUnLockRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";
            var service = await MapUnLockService.BuildAsync(deserialized);
            try
            {
                await service.Unlock();
            }
            catch(MapUnlockException ex)
            {
                var errSession = new MsgBoxResponse(ex.Message, "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                return;
            }
            var res = await service.SaveDataAndGetResponse();
            var marshalledResponse = JsonConvert.SerializeObject(res);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
        }
    }
}
