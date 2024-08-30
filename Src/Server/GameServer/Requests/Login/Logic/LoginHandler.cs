using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.Login.DataClasses;
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
            var deserialized = JsonConvert.DeserializeObject<LoginRequest>(requestJsonString!);
            //Construct response
            var res = new LoginResponse();
            var resdict = await res.ToDictionary();

            //Test
            resdict["openingTutorialFlg"] = 1;
            resdict["storeUrl"] = "";
            resdict["teamEventButtonHiddenFlg"] = 1;
            resdict["shopSaleList"] = new List<object>();
            resdict["noticePageList"] = new List<object> { new Dictionary<string, object> { { "pageNo", 2 } }, new Dictionary<string, object> { { "pageNo", 6 } } };
            resdict["mstMapMobPeriodNoList"] = new List<object> { 0, 1 };
            resdict["hitodamaShopSaleList"] = new List<object>();
            resdict["webServerIp"] = "";
            resdict["dialogTitle"] = "";
            resdict["responseCodeTeamEvent"] = 0;
            resdict["requireAgeConfirm"] = true;
            
            foreach(var table in Consts.LOGIN_TABLES)
            {
                string? tableText = null!;
                if(table.StartsWith("ywp_user"))
                {
                    var gotten = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<object>(deserialized.Gdkey, table);
                    tableText = JsonConvert.SerializeObject(gotten);
                }
                else tableText = ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache[table];
                //if we can't deserialize json it means it's not a json and we store it as is
                object tableObj = new();
                try
                {
                    tableObj = JsonConvert.DeserializeObject<object>(tableText);
                }
                catch
                {
                    tableObj = tableText;
                }
                if(tableObj == null)
                {
                    tableObj = new List<object>();
                }
                resdict[table] = tableObj;
            }

            var encryptedRes = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(resdict));
            ctx.Response.Headers.ContentType = "application/json";
            await ctx.Response.WriteAsync(encryptedRes);

        }
    }
}
