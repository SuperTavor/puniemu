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
            try
            {
                await RegisterDefaultTables(deserialized, generatedUserData);
            }
            catch
            {
                ctx.Response.StatusCode = 500;
                await ctx.Response.WriteAsync("Internal server error");
                return;
            }
            var createUserResponse = new CreateUserResponse(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_user_tutorial_list_def"], generatedUserData);
            var marshalledResponse = JsonConvert.SerializeObject(createUserResponse);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);            
            
        }

        private static async Task RegisterDefaultTables(CreateUserRequest deserialized,YwpUserData generatedUserData)
        {
           Dictionary<string,object> tables = new Dictionary<string,object>();
           foreach(var userTable in Consts.LOGIN_TABLES.Where(x => x.Contains("ywp_user") && x != "ywp_user_data"))
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

                    tables.Add(userTable, deserializedDefaultUserTable);
                }
                else
                {
                    throw new Exception();
                }
           }
            //Set openingTutorialFlg
           tables.Add("opening_tutorial_flg", 1);
           //Set ywpuser data
           tables.Add("ywp_user_data", generatedUserData);
           //Set start date
           tables.Add("start_date",DateTimeOffset.Now.ToUnixTimeMilliseconds());
           
           await UserDataManager.Logic.UserDataManager.SetEntireUserData(deserialized.Level5UserID,tables);
        }
    }
}
