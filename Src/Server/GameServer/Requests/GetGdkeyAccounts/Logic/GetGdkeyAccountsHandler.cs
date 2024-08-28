using Newtonsoft.Json;
using Puniemu.Src.NHNCrypt.Logic;
using Puniemu.Src.Server.GameServer.DataClasses;
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
            ctx.Response.ContentType = "application/json";
            GetGdkeyAccountsResponse? res = await GetGdkeyAccountsResponse.ConstructAsync(deserialized.DeviceID, gdkeys);
            if(res == null)
            {
                var response = new MsgAndGoBackToTitle("this error should almost NEVER happen\ncontact zura and darkcraft", "what");
                var encrypted = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(response));
                await ctx.Response.WriteAsync(encrypted);
            }
            await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));
        }
    }
}
