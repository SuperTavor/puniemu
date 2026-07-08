using Newtonsoft.Json;
using Puniemu.Src.Server.CustomAuth;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.Rename.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.SerialConfirm.DataClasses;
using System.Buffers;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Requests.SerialConfirm.Logic
{
    public class SerialConfirmHandler
    {
        //NOTE: this is not an implementation of the actual serialConfirm.
        //in WWR, we are using serialConfirm as the auth gateway
        //this the auth logic, the true serialConfirm will be implemented later and you will be able to toggle between the 2
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<SerialConfirmRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";

            int code = 0;
            if (int.TryParse(deserialized.SerialCode.Trim(), out int number))
            {
                code = number;
            }
            else
            {
                var errRes = new MsgBoxResponse("Invalid code. Needs to be all numbers", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errRes)));
                return;
            }
            if (AuthManager.CodeCache.TryGetValue(code, out var val))
            {
                if(val.isLink)
                {
                    AuthManager.CodeCache.Remove(code, out _);
                    await UserDataManager.Logic.UserDataManager.AddOrEditEmail(val.email, val.udkey);
                    var res = new MsgBoxResponse("Saves successfully linked to email.", "Success");
                    await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));
                    return;
                }
                else
                {
                    AuthManager.CodeCache.Remove(code, out _);
                    var linkedData = await UserDataManager.Logic.UserDataManager.GetDataByMail(val.email);
                    if (linkedData == null)
                    {
                        var errRes = new MsgBoxResponse("No data is linked for this email", "Error");
                        await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errRes)));
                        return;
                    }
                    //Transfer gdkeys from old udkey to new udkey
                    var oldUdkey = linkedData.CurrentUdkey;
                    var newUdkey = val.udkey;
                    if(oldUdkey == newUdkey)
                    {
                        var errRes = new MsgBoxResponse("You cannot transfer data to the same device.", "Error");
                        await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errRes)));
                        return;
                    }
                    await UserDataManager.Logic.UserDataManager.TransferGdkeys(oldUdkey, newUdkey);
                    await UserDataManager.Logic.UserDataManager.AddOrEditEmail(val.email, val.udkey);
                    var res = new MsgBoxResponse("Connected to cloud save on this\ndevice.Please restart your game.", "Error");
                    await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));
                    return;
                }
            }
        }
    }
}
