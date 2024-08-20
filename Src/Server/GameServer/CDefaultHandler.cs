using Newtonsoft.Json;
using Puniemu.Src.ConfigManager;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.NHNCrypt;

namespace Puniemu.Src.Server.GameServer
{
    public class CDefaultHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            if (ctx.Request.Method != HttpMethods.Post)
            {
                ctx.Response.StatusCode = StatusCodes.Status405MethodNotAllowed; 
                await ctx.Response.WriteAsync("Method Not Allowed");
                return;
            }

            try
            {
                var path = ctx.Request.Path;
                var formattedMsg = $"Unimplemented request:\n{path}";
                var msgStruct = new SMsgAndGoBackToTitle(formattedMsg, CConfigManager.Cfg!.Value.ServerName);
                var msgJson = JsonConvert.SerializeObject(msgStruct);
                var encrypted = CNHNCrypt.EncryptResponse(msgJson);
                ctx.Response.Headers.ContentType = "application/json";
                await ctx.Response.WriteAsync(encrypted);
            }
            catch
            {
                ctx.Response.StatusCode = StatusCodes.Status500InternalServerError; 
                await ctx.Response.WriteAsync("Internal server error");
            }
        }
    }
}
