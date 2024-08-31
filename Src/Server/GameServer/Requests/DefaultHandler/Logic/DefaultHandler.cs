using Newtonsoft.Json;
using Puniemu.Src.ConfigManager.Logic;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.DefaultHandler.Logic
{
    public class DefaultHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            try
            {
                var path = ctx.Request.Path;
                var formattedMsg = $"Unimplemented request:\n{path}";
                var msgStruct = new MsgBoxResponse(formattedMsg, ConfigManager.Logic.ConfigManager.Cfg!.Value.ServerName);
                var msgJson = JsonConvert.SerializeObject(msgStruct);
                var encrypted = NHNCrypt.Logic.NHNCrypt.EncryptResponse(msgJson);
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
