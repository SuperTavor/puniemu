using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.CreateUser.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;
using System.Buffers;
using Puniemu.Src.ConfigManager.Logic;
using Puniemu.Src.UserDataManager.Logic;

namespace Puniemu.Src.Server.GameServer.Requests.CreateUser.Logic
{
    public class CreateUserHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<CreateUserRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";
            var generatedUserData = new YwpUserData((PlayerIcon)deserialized.IconID, (PlayerTitle)deserialized.IconID, deserialized.Level5UserID, deserialized.PlayerName);
            var createUserResponse = new CreateUserResponse(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_user_tutorial_list_def"], generatedUserData);
            var marshalledResponse = JsonConvert.SerializeObject(createUserResponse);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
            //Register the default tables on another thread and return from the handler, so createUser doesn't take too much time
            _ = Task.Run(() => RegisterDefaultTables(deserialized, generatedUserData));
            
        }

        private static async Task RegisterDefaultTables(CreateUserRequest deserialized,YwpUserData generatedUserData)
        {
           foreach(var userTable in Consts.LOGIN_TABLES.Where(x => x.StartsWith("ywp_user") && x != "ywp_user_data"))
           {
                //initialize with default if exists, else 
                if(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache.TryGetValue(userTable+"_def", out var data))
                {
                    object? deserializedDefaultUserTable = null!;
                    try
                    {
                        deserializedDefaultUserTable = JsonConvert.DeserializeObject<object>(data);
                    }
                    catch
                    {
                        deserializedDefaultUserTable = data;
                    }

                    await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, userTable, deserializedDefaultUserTable); 
                }
           }
            //Set ywpuser data
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_data", generatedUserData);
           await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID,"start_date",DateTimeOffset.Now.ToUnixTimeMilliseconds());
        }
    }
}
