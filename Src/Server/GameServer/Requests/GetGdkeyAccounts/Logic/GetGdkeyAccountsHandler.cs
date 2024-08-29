using Newtonsoft.Json;
using System.Buffers;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.GetGdkeyAccounts.DataClasses;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Requests.GetGdkeyAccounts.Logic
{
    public class GetGdkeyAccountsHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
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
