using Newtonsoft.Json;
using Puniemu.Src.UserDataManager;
using Puniemu.Src.ConfigManager;
using Puniemu.Src.Server.GameServer.Requests.GameContinue.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;
using System.Buffers;

namespace Puniemu.Src.Server.GameServer.Requests.GameContinue.Logic
{
    public class GameContinueHandler
    {
        //Puni puni now have a new profile system (with plate, effect and codename) but for some reason in the private server it's always the old app
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<GameContinueRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";
            var userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID, "ywp_user_data");
            
            if (userData!.YMoney < 500)
            {
                var res = new MsgBoxResponse("You don't have enough Ymoney.", "Not Enough Ymoney");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));
            }
            else
            {
                userData.YMoney -= 500;
                await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserID!, "ywp_user_data", userData);
                var renameResponse = new GameContinueResponse(userData!);
                var marshalledResponse = JsonConvert.SerializeObject(renameResponse);
                var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
                await ctx.Response.WriteAsync(encryptedResponse);
            }
        }
    }
}
