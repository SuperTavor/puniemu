using Newtonsoft.Json;
using System.Text;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.GetMaster.DataClasses;
using System.Buffers;
namespace Puniemu.Src.Server.GameServer.Requests.GetMaster.Logic
{
    public class GetMasterHandler
    {
        // To not unmarshal every time and improve performance, we store the unmarshalled versions of previously unmarshalled jsons
        private static Dictionary<string, object> UnmarshalCache = new();
        //Tables sometimes requested by the server that are never delivered, even on the official servers.
        private static object UnmarshalOrGetFromCache(string jsonName, string jsonStr)
        {
            if (!UnmarshalCache.ContainsKey(jsonName))
            {
                object? unmarshalledJson = null!;
                try
                {
                    unmarshalledJson = JsonConvert.DeserializeObject<object>(jsonStr);
                }
                //if can't deserialize it means we need to serve a table so it's a string
                catch
                {
                    unmarshalledJson = jsonStr;
                }

                UnmarshalCache[jsonName] = unmarshalledJson!;
            }
            return UnmarshalCache[jsonName];
        }

        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Response.ContentType = "application/json";
            ctx.Request.EnableBuffering();
            var buffer = new MemoryStream();
            await ctx.Request.Body.CopyToAsync(buffer);
            buffer.Seek(0, SeekOrigin.Begin);
            string? requestJsonString;
            using (var reader = new StreamReader(buffer, Encoding.UTF8))
            {
                var readResult = await reader.ReadToEndAsync();
                requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(readResult);
            }
            Dictionary<string, object> requestJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(requestJsonString!)!;
            // Load base MasterData JSON. the base MasterData JSON contains data other than the requested tables that is shipped with the requested tables.
            var MasterDataJson = BaseMasterDataBuilder.Build();
            //Tables contains all the requested tables
            string[] tables;
            if (!requestJson.ContainsKey("tableNames"))
            {
                await GeneralUtils.SendBadRequest(ctx);
                return;
            }
            else
            {
                var tblNames = (string)requestJson["tableNames"];
                if (tblNames != "all")
                {
                    tables = tblNames.Split('|');
                }
                else
                {
                    tables = Consts.ALL_TABLE.Split('|');
                }
            }
            //Put all the requested tables into the base json
            foreach (var tblName in tables)
            {
                //Not sending the stuff you don't have seemingly works? I may be shooting myself in the foot with this rn
                //god knows, check back in like a month
                if (DataManager.Logic.DataManager.GameDataManager!.GamedataCache.ContainsKey(tblName))
                {
                    var selectedJsonUnmarshalled = UnmarshalOrGetFromCache(tblName, DataManager.Logic.DataManager.GameDataManager.GamedataCache[tblName]);
                    MasterDataJson[tblName] = selectedJsonUnmarshalled;
                }
            }
            var outResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(MasterDataJson));
            await ctx.Response.WriteAsync(outResponse);

        }
    }
}
