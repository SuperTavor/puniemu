using Newtonsoft.Json;
using Puniemu.Src.ConfigManager.Logic;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.Init.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Requests.Init.Logic
{
    public class InitHandler
    {

        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Response.ContentType = "application/json";
            if (ConfigManager.Logic.ConfigManager.Cfg!.Value.IsMaintenance)
            {
                var msg = new MsgAndGoBackToTitle(ConfigManager.Logic.ConfigManager.Cfg.Value.MaintenanceMsg, "Maintenance Notice");
                var encryptedMsgJson = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(msg));
                await ctx.Response.WriteAsync(encryptedMsgJson);
                return;
            }
            //read and decrypt request
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var requestBuf = readResult.Buffer.ToArray();
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            string requestStr;
            try
            {
                requestStr = Encoding.UTF8.GetString(requestBuf);
            }
            catch
            {
                await CGeneralUtils.SendBadRequest(ctx);
                return;
            }
            string? decrypted;
            try
            {
                decrypted = NHNCrypt.Logic.NHNCrypt.DecryptRequest(requestStr);
            }
            catch
            {
                await CGeneralUtils.SendBadRequest(ctx);
                return;
            }

            //Check game version
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(decrypted);
            if (dict == null)
            {
                await CGeneralUtils.SendBadRequest(ctx);
                return;
            }
            if (dict.ContainsKey("appVer"))
            {
                var appVer = dict["appVer"];
                if ((string)appVer != ConfigManager.Logic.ConfigManager.Cfg!.Value.ClientVersion)
                {
                    //Display "Wrong version" message box in the client
                    var msg = new MsgAndGoBackToTitle("Game version is not\ncompatible with the server.", ConfigManager.Logic.ConfigManager.Cfg!.Value.ServerName);
                    var jsonMsg = JsonConvert.SerializeObject(msg);
                    var encrypted = NHNCrypt.Logic.NHNCrypt.EncryptResponse(jsonMsg);
                    await ctx.Response.WriteAsync(encrypted);
                }
                else
                {
                    var response = new InitResponse();
                    var jsonRes = JsonConvert.SerializeObject(response);
                    var encrypted = NHNCrypt.Logic.NHNCrypt.EncryptResponse(jsonRes);
                    await ctx.Response.WriteAsync(encrypted);
                }
            }
            else
            {
                await CGeneralUtils.SendBadRequest(ctx);
                return;
            }
        }
    }
}
