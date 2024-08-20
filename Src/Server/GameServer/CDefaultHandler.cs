using Newtonsoft.Json;
using Puniemu.Src.ConfigManager;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.NHNCrypt;

namespace Puniemu.Src.Server.GameServer
{
    public class CDefaultHandler
    {
        public static async void HandleAsync(HttpContext ctx)
        {
            var path = ctx.Request.Path;
            var formattedMsg = $"Unimplemented request:\n{path}";
            var msgStruct = new SMsgAndGoBackToTitle(formattedMsg,CConfigManager.Cfg!.Value.ServerName);
            var msgJson = JsonConvert.SerializeObject(msgStruct);
            var encrypted = CNHNCrypt.EncryptResponse(msgJson);
            ctx.Response.Headers.ContentType = "application/json";
            await ctx.Response.WriteAsync(encrypted);
        }
    }
}
