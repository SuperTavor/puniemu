using Newtonsoft.Json;
using Puniemu.Src.UserDataManager;

using Puniemu.Src.Server.GameServer.Requests.UpdateProfile.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;
using System.Buffers;

namespace Puniemu.Src.Server.GameServer.Requests.UpdateProfile.Logic
{
    public class UpdateProfileHandler
    {
        //Puni puni now have a new profile system (with plate, effect and codename) but for some reason in the private server it's always the old app
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<UpdateProfileRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";
            var userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID, "ywp_user_data");

            // Unlocked Icons and Ttiles
            var userPlayerIcon = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_player_icon");
            var userPlayerTitle = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_player_title");
            var userPlayerPlate = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_player_plate");
            var userPlayerEffect = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_player_effect");
            var userPlayerCodename = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_player_codename");

            // Change current Icons/Titles ID by the Icons/Titles ID inside the requests data
            if (deserialized.IconID > 0)
            {
                userData!.IconID=deserialized.IconID;
            }
            if (deserialized.TitleID > 0)
            {
                userData!.CharacterTitleID = deserialized.TitleID;
            }
            if (deserialized.CodenameID > 0)
            {
                userData!.CodeNameID = deserialized.CodenameID;
            }
            if (deserialized.EffectID > 0)
            {
                userData!.EffectID = deserialized.EffectID;
            }
            if (deserialized.PlateID > 0)
            {
                userData!.PlateID = deserialized.PlateID;
            }
            
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_data", userData);
            var updateProfileResponse = new UpdateProfileResponse(userPlayerIcon!, userPlayerTitle!, userPlayerPlate!, userPlayerEffect!, userPlayerCodename!,  userData!);
            var marshalledResponse = JsonConvert.SerializeObject(updateProfileResponse);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
        }
    }
}
