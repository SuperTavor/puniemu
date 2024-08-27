using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.GetGdkeyAccounts.DataClasses;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Requests.GetGdkeyAccounts.Logic
{
    public class GetGdkeyAccountsHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            var encRequest = Encoding.UTF8.GetString(ctx.Request.BodyReader.ReadAsync().Result.Buffer);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<GetGdkeyAccountsRequest>(requestJsonString!);
            //Convert gdkey map to gdkey list
            List<string> gdkeys = new();
            foreach (var gdkeyDict in deserialized.GDKeys)
            {
                foreach (var kvp in gdkeyDict)
                {
                    gdkeys.Add(kvp.Value);
                }
            }
            GetGdkeyAccountsResponse res = new();
            try
            {
                res = await GetGdkeyAccountsResponse.ConstructAsync(gdkeys);
            }
            catch
            {
                ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return;
            }
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));
        }
    }
}
