using Newtonsoft.Json;
using Puniemu.Src.DBService;

using Puniemu.Src.Server.GameServer.Requests.Rename.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;
using System.Buffers;
using Puniemu.Src.Server.GameServer.Logic;
using Microsoft.IdentityModel.Tokens;

namespace Puniemu.Src.Server.GameServer.Requests.Rename.Logic
{
    public class RenameHandler
    {
        //Puni puni now have a new profile system (with plate, effect and codename) but for some reason in the private server it's always the old app
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<RenameRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";
            var userData = await DBService.Logic.DBService.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID, "ywp_user_data");

            if (userData != null && !deserialized.NewPlayerName.IsNullOrEmpty())
            {
                userData.PlayerName = deserialized.NewPlayerName;
            }
            GenerateFriendData.RefreshYwpUserFriend(deserialized.Level5UserID, -1,-1, userData!.PlayerName, -1, "");
            var renameResponse = new RenameResponse(userData!);
            var marshalledResponse = JsonConvert.SerializeObject(renameResponse);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
            await DBService.Logic.DBService.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_data", userData);
        }
    }
}
