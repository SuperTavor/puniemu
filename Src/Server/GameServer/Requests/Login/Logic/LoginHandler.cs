using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.DataManager.Logic;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Requests.Login.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.UseAddition.Logic;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using Puniemu.Src.UserDataManager.DataClasses;
using Puniemu.Src.UserDataManager.Logic;
using Puniemu.Src.Utils.GeneralUtils;
using System.Buffers;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Requests.Login.Logic
{
    public static class LoginHandler
    {

        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<LoginRequest>(requestJsonString!)!;
            var acc = await UserDataManager.Logic.UserDataManager.GetAccountFromGdkeyAsync(deserialized.Gdkey!);

            var additionService = new UseAdditionService(deserialized.Gdkey);
            if(await additionService.CanDoShrineToday())
            {
                await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Gdkey, "ywp_user_addition", false);
            }
            await ShopLimitManager.CheckShopLimitReset(deserialized.Gdkey);
            //Construct response
            CommonLoginResponse res = new CommonLoginResponse();
            if (DataManager.Logic.DataManager.IsWibWob)
            {
                res = new DataClasses.WibWob.LoginResponse();
            }
            else res = new DataClasses.Puni.LoginResponse();

            var userYokai = new TableParser<YwpUserYoukai>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_youkai"));
            await res.ConstructAsync(deserialized!.Gdkey!);
            //Get the user tables
            var resdict = (await res.ToDictionary())!;            
            //for now for testing just add puni login tables, similar anyway
            await GeneralUtils.AddTablesToResponse(Consts.LOGIN_TABLES_PUNI,resdict!,true,deserialized!.Gdkey!);
            //Set last login time to now
            acc.LastLoginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var encryptedRes = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(resdict));
            ctx.Response.Headers.ContentType = "application/json";
            await ctx.Response.WriteAsync(encryptedRes);

        }
    }
}
