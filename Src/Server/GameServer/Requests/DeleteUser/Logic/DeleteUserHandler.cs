using Newtonsoft.Json;
using Puniemu.Src.UserDataManager;

using Puniemu.Src.Server.GameServer.Requests.DeleteUser.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;
using System.Buffers;

namespace Puniemu.Src.Server.GameServer.Requests.DeleteUser.Logic
{
    public class DeleteUserHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<DeleteUserRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";
            var userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID, "ywp_user_data");

            // Response Code (If password (character id) was correct)
            int RespCode;
            // Check if text was correct
            if (deserialized.CharacterID == userData!.CharacterID)
            {
                RespCode = 0;
                if (deserialized.FinalAnswerFlag == 1) {
                    await UserDataManager.Logic.UserDataManager.DeleteUser(deserialized.DeviceID,deserialized.Level5UserID);
                }
            } else {
                RespCode = 1;
            }
          
            var DeleteUserResponse = new DeleteUserResponse(RespCode);
            var marshalledResponse = JsonConvert.SerializeObject(DeleteUserResponse);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
        }
    }
}