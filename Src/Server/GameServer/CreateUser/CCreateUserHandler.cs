using Newtonsoft.Json;
using Puniemu.src.Utils.UserDataManager;
using Puniemu.Src.ConfigManager;
using Puniemu.Src.Server.GameServer.CreateUser.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.NHNCrypt;
using System.Text;

namespace Puniemu.Src.Server.GameServer.CreateUser
{
    public class CCreateUserHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            var encRequest = Encoding.UTF8.GetString(ctx.Request.BodyReader.ReadAsync().Result.Buffer);
            var requestJsonString = CNHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<SCreateUserRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";
            await CUserDataManager.RegisterGdKeyInUdKeyAsync(deserialized.DeviceID, deserialized.Level5UserID);
            //The icon ID is provided as also the title ID, as that's the only way to generate the first title ID, as the ids match
            var generatedUserData = new SYwpUserData((ePlayerIcon)deserialized.IconID, (ePlayerTitle)deserialized.IconID, deserialized.Level5UserID, deserialized.PlayerName);
            await CUserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_data", generatedUserData);
            await CUserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_tutorial_list", CConfigManager.GameDataManager.GamedataCache["DefaultTutorialList"]);
            await CUserDataManager.SetYwpUserAsync(deserialized.Level5UserID,"start_date",DateTimeOffset.Now.ToUnixTimeSeconds());
            var createUserResponse = new SCreateUserResponse(CConfigManager.GameDataManager.GamedataCache["DefaultTutorialList"], generatedUserData);
            var marshalledResponse = JsonConvert.SerializeObject(createUserResponse);
            var encryptedResponse = CNHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
        }
    }
}
