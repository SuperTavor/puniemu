using Newtonsoft.Json;
using Puniemu.Src.ConfigManager;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Init.DataClasses;
using Puniemu.Src.Utils.NHNCrypt;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Init
{
    public class CInitHandler
    {
        static async Task SendBadRequest(HttpContext ctx)
        {
            ctx.Response.Headers.ContentType = "text/plain";
            ctx.Response.StatusCode = 400;
            await ctx.Response.WriteAsync("Bad request");
        }
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Response.ContentType = "application/json";
            if (CConfigManager.Cfg!.Value.IsMaintenance)
            {
                var msg = new SMsgAndGoBackToTitle(CConfigManager.Cfg.Value.MaintenanceMsg, "Maintenance Notice");
                var encryptedMsgJson = CNHNCrypt.EncryptResponse(JsonConvert.SerializeObject(msg));
                await ctx.Response.WriteAsync(encryptedMsgJson);
                return;
            }
            //read and decrypt request
            var requestBuf = ctx.Request.BodyReader.ReadAsync().Result.Buffer;
            string requestStr;
            try
            {
                requestStr = Encoding.UTF8.GetString(requestBuf);
            }
            catch
            {
                await SendBadRequest(ctx);
                return;
            }
            string? decrypted;
            try
            {
                 decrypted = CNHNCrypt.DecryptRequest(requestStr);
            }
            catch
            {
                await SendBadRequest(ctx);
                return;
            }
            
            //Check game version
            var dict = JsonConvert.DeserializeObject<Dictionary<string,object>>(decrypted);
            if(dict == null)
            {
                await SendBadRequest(ctx);
                return;
            }
            if(dict.ContainsKey("appVer"))
            {
                var appVer = dict["appVer"];
                if((string)appVer != CConfigManager.Cfg!.Value.ClientVersion)
                {
                    //Display "Wrong version" message box in the client
                    var msg = new SMsgAndGoBackToTitle("Game version is not\ncompatible with the server.", CConfigManager.Cfg!.Value.ServerName);
                    var jsonMsg = JsonConvert.SerializeObject(msg);
                    var encrypted = CNHNCrypt.EncryptResponse(jsonMsg);
                    await ctx.Response.WriteAsync(encrypted);
                }
                else
                {
                    var response = new SInitResponse();
                    var jsonRes = JsonConvert.SerializeObject(response);
                    var encrypted = CNHNCrypt.EncryptResponse(jsonRes);
                    await ctx.Response.WriteAsync(encrypted);
                }
            }
            else
            {
                await SendBadRequest(ctx);
                return;
            }
        }
    }
}
