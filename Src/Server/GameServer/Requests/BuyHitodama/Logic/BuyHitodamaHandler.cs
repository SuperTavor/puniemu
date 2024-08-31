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
            if(HitodamaGoods.Goods.TryGetValue(deserialized.GoodsId,out var good))
            {
                var userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Gdkey, "ywp_user_data");
                var before = new HitodamaInformation(userData.Hitodama, userData.FreeHitodama); 
                userData.YMoney -= good.Cost;
                userData.Hitodama += good.RewardedHitodama;
                var after = new HitodamaInformation(userData.Hitodama,userData.FreeHitodama);
                var res = new BuyHitodamaResponse(before,after,userData);
                ctx.Response.Headers.ContentType = "application/json";
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
