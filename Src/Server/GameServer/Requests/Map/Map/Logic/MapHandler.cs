using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.Map.Map.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.MapUnLock.DataClasses;
using Puniemu.Src.UserDataManager.Logic;
using System.Buffers;
using System.Text;
namespace Puniemu.Src.Server.GameServer.Requests.Map.Map.Logic
{
    public class MapHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<CommonRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";

            var res = new MapResponse();
            res.UserData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID, "ywp_user_data");
            res.YwpUserMap = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_map");
            var mstMapDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(
                DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_map"]
            );
            res.YwpMstMap = ((JArray)mstMapDict["data"]).ToObject<List<object>>(); res.YwpMstEvent = JsonConvert.DeserializeObject<List<object>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_event"]);

            var marshalledResponse = JsonConvert.SerializeObject(res);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
        }
    }
}
