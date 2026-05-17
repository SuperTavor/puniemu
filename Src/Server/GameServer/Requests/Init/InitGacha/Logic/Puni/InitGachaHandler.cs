using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.InitGacha.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses.Gacha.GachaStamp;
using System.Buffers;
using System.Text;
using Puniemu.Src.Server.GameServer.Requests.Init.InitGacha.DataClasses.Puni;

namespace Puniemu.Src.Server.GameServer.Requests.Init.InitGacha.Logic.Puni
{
    public class InitGachaHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<InitGachaRequest>(requestJsonString!);
            var res = new InitGachaResponse();

            res.YwpMstGacha = JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager!.GamedataCache["ywp_mst_gacha"])!["tableData"];
            res.YwpMstItem = JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager!.GamedataCache["ywp_mst_item"])!["tableData"];
            if (DataManager.Logic.DataManager.GameDataManager!.GamedataCache.ContainsKey("gachaStampList"))
            {
                res.GachaStampList = JsonConvert.DeserializeObject<List<GachaStamp>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["gachaStampList"])!;
            }
            else
            {
                res.GachaStampList = new List<GachaStamp>();
            }

            var outResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res));
            await ctx.Response.WriteAsync(outResponse);

        }
    }
}
