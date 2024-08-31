using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.Login.DataClasses;
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
            var resdict = await res.ToDictionary(deserialized.Gdkey);            

            foreach (var table in Consts.LOGIN_TABLES)
            {
                string? tableText = null!;
                if (table.StartsWith("ywp_user"))
                {
                    var gotten = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<object>(deserialized.Gdkey, table);
                    tableText = JsonConvert.SerializeObject(gotten);
                }
                else tableText = ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache[table];
                //if we can't deserialize json it means it's not a json and we store it as is
                object tableObj = new();
                try
                {
                    // If was cud structure, only send the data
                    tableObj = JsonConvert.DeserializeObject<object>(tableText);
                    if (tableObj is JObject) {
                        var jObject_temp = (JObject)tableObj;
                        if (jObject_temp.ContainsKey("data"))
                        {
                            tableObj = jObject_temp["data"];
                        } else if (jObject_temp.ContainsKey("tableData"))
                        {
                            tableObj = jObject_temp["tableData"];
                        }

                    }

                }
                catch
                {
                    tableObj = tableText;
                }
                if (tableObj == null)
                {
                    tableObj = new List<object>();
                }
                resdict[table] = tableObj;
            }
            //Set last login time to now
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Gdkey, "lgn_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            var encryptedRes = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(resdict));
            ctx.Response.Headers.ContentType = "application/json";
            await ctx.Response.WriteAsync(encryptedRes);

        }
    }
}
