using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.CreateUser.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;
using Puniemu.Src.ConfigManager.Logic;

namespace Puniemu.Src.Server.GameServer.Requests.CreateUser.Logic
{
    public class CreateUserHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            var encRequest = Encoding.UTF8.GetString(ctx.Request.BodyReader.ReadAsync().Result.Buffer);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<CreateUserRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";
            await UserDataManager.Logic.UserDataManager.RegisterGdKeyInUdKeyAsync(deserialized.DeviceID, deserialized.Level5UserID);
            //The icon ID is provided as also the title ID, as that's the only way to generate the first title ID, as the ids match
            var generatedUserData = new YwpUserData((PlayerIcon)deserialized.IconID, (PlayerTitle)deserialized.IconID, deserialized.Level5UserID, deserialized.PlayerName);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_data", generatedUserData);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_tutorial_list", ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["DefaultTutorialList"]);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_player_icon", Consts.DEFAULT_OBTAINED_ICONS_AND_TITLES);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_player_title", Consts.DEFAULT_OBTAINED_ICONS_AND_TITLES);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "start_date", DateTimeOffset.Now.ToUnixTimeSeconds());
            var createUserResponse = new CreateUserResponse(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["DefaultTutorialList"], generatedUserData);
            var marshalledResponse = JsonConvert.SerializeObject(createUserResponse);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
        }
    }
}
