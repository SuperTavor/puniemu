using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.BuyHitodama.DataClasses;
using System.Buffers;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Requests.BuyHitodama.Logic
{
    public static class BuyHitodamaHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<BuyHitodamaRequest>(requestJsonString!)!;
            //Find ID
            ctx.Response.Headers.ContentType = "application/json";
            if (HitodamaGoods.Goods.TryGetValue(deserialized.GoodsId,out var good))
            {
                var userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized!.Gdkey!, "ywp_user_data");
                var before = new HitodamaInformation(userData!.Hitodama, userData.FreeHitodama);
                if(userData.YMoney < good.Cost)
                {
                    var response = JsonConvert.SerializeObject(new MsgBoxResponse("You don't have enough Y Money", "Too expensive"));
                    await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(response));
                    return;
                }
                userData.BuyHitodamaGood(good);
                //Update the userdata on the server
                await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_data", userData);
                var after = new HitodamaInformation(userData.Hitodama,userData.FreeHitodama);
                var res = new BuyHitodamaResponse(before,after,userData);
                var encryptedAndSerialized = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res));
                await ctx.Response.WriteAsync(encryptedAndSerialized);
            }
            else
            {
                ctx.Response.StatusCode = 400;
                await ctx.Response.WriteAsync("Invalid GoodID");
            }
        }
    }
}
