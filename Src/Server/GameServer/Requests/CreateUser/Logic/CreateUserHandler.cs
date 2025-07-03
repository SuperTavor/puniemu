using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.CreateUser.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;
using System.Buffers;
using Puniemu.Src.UserDataManager.Logic;
using Puniemu.Src.DataManager.Logic;
using Puniemu.Src.UserDataManager.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.CreateUser.Logic
{
    public class CreateUserHandler
    {
        private static Account Acc { get; set; }
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<CreateUserRequest>(requestJsonString!);
            var dbres = await UserDataManager.Logic.UserDataManager.SupabaseClient.From<Account>().Where(x => x.Gdkey == deserialized.Level5UserID).Get();
            Acc = dbres.Model;
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
            var createUserResponse = new CreateUserResponse(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_user_tutorial_list_def"], generatedUserData);
            Acc.Update<Account>();
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
                if(DataManager.Logic.DataManager.GameDataManager!.GamedataCache.TryGetValue(userTable+"_def", out var data))
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

                    tables.Add(userTable, deserializedDefaultUserTable!);
                }
                else
                {
                    throw new Exception();
                }
           }

           //Set ywpuser data
           tables.Add("ywp_user_data", generatedUserData);
           //Set start date
           Acc.StartDate = DateTimeOffset.Now.ToUnixTimeMilliseconds();
           tables.Add("login_stamp", "0|0|0");
           await UserDataManager.Logic.UserDataManager.SetEntireUserData(deserialized.Level5UserID,tables);
        }
    }
}
