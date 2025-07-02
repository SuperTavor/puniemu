using Newtonsoft.Json;
using System.Buffers;
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
                await GeneralUtils.SendBadRequest(ctx);
                return;
            }
            string? decrypted;
            try
            {
                decrypted = NHNCrypt.Logic.NHNCrypt.DecryptRequest(requestStr);
            }
            catch
            {
                await GeneralUtils.SendBadRequest(ctx);
                return;
            }

            //Check game version
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(decrypted!);
            if (dict == null)
            {
                await GeneralUtils.SendBadRequest(ctx);
                return;
            }
            if (dict.ContainsKey("appVer"))
            {
                var appVer = dict["appVer"];
                if ((string)appVer != DataManager.Logic.DataManager.GameVersion)
                {
                    //Display "Wrong version" message box in the client
                    var msg = new MsgBoxResponse("Game version is not\ncompatible with the server.", DataManager.Logic.DataManager.ServerName!);
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
                await GeneralUtils.SendBadRequest(ctx);
                return;
            }
        }
    }
}
