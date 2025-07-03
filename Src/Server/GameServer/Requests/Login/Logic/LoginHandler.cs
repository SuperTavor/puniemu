using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.Login.DataClasses;
using Puniemu.Src.UserDataManager.DataClasses;
using Puniemu.Src.UserDataManager.Logic;
using Puniemu.Src.Utils.GeneralUtils;
using System.Buffers;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Requests.Login.Logic
{
    public static class LoginHandler
    {

        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<LoginRequest>(requestJsonString!);

            var dbRes = await UserDataManager.Logic.UserDataManager.SupabaseClient.From<Account>().Where(x => x.Gdkey == deserialized.Gdkey).Get();
            var acc = dbRes.Model;
            //Construct response
            var res = new LoginResponse();
            //Get the user tables
            var userTables = await UserDataManager.Logic.UserDataManager.GetEntireUserData(deserialized!.Gdkey!);
            var resdict = await res.ToDictionary(deserialized!.Gdkey!);            
            await GeneralUtils.AddTablesToResponse(Consts.LOGIN_TABLES,resdict,true,deserialized!.Gdkey!);
            //Set last login time to now
            acc.LastLoginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            await acc.Update<Account>();
            var encryptedRes = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(resdict));
            ctx.Response.Headers.ContentType = "application/json";
            await ctx.Response.WriteAsync(encryptedRes);

        }
    }
}
