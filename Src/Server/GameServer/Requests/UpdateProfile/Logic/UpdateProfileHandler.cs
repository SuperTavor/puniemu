using Newtonsoft.Json;
using Puniemu.Src.UserDataManager;
using Puniemu.Src.ConfigManager;
using Puniemu.Src.Server.GameServer.UpdateProfile.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;

namespace Puniemu.Src.Server.GameServer.UpdateProfile
{
    public class UpdateProfileHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            var encRequest = Encoding.UTF8.GetString(ctx.Request.BodyReader.ReadAsync().Result.Buffer);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<UpdateProfileRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";
            var userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID, "ywp_user_data");

            // Unlocked Icons and Ttiles
            var userPlayerIcon = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_player_icon");
            var userPlayerTitle = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_player_title");

            // Change current Icons/Titles ID by the Icons/Titles ID inside the requests data
            if (deserialized.IconID > 0)
            {
                userData.IconID=deserialized.IconID;
            }
            if (deserialized.TitleID > 0)
            {
                userData.CharacterTitleID = deserialized.TitleID;
            }
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_data", userData);
            var createUserResponse = new UpdateProfileResponse(userPlayerIcon, userPlayerTitle, userData);
            var marshalledResponse = JsonConvert.SerializeObject(createUserResponse);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
        }
    }
}
