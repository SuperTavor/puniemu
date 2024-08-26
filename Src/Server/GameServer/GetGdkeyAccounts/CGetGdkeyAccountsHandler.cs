using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.CreateUser.DataClasses;
using Puniemu.Src.Server.GameServer.GetGdkeyAccounts.DataClasses;
using Puniemu.Src.Utils.NHNCrypt;
using System.Text;

namespace Puniemu.Src.Server.GameServer.GetGdkeyAccounts
{
    public class CGetGdkeyAccountsHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            var encRequest = Encoding.UTF8.GetString(ctx.Request.BodyReader.ReadAsync().Result.Buffer);
            var requestJsonString = CNHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<SGetGdkeyAccountsRequest>(requestJsonString!);
            //Convert gdkey map to gdkey list
            List<string> gdkeys = new();
            foreach(var gdkeyDict in deserialized.GDKeys)
            {
                foreach(var kvp in gdkeyDict)
                {
                    gdkeys.Add(kvp.Value);
                }
            }
            SGetGdkeyAccountsResponse res = new();
            try
            {
                 res = await SGetGdkeyAccountsResponse.ConstructAsync(gdkeys);
            }
            catch
            {
                ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return;
            }
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsync(CNHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));
        }
    }
}
