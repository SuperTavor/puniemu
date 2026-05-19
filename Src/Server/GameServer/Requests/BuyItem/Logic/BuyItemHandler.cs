using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.NHNCrypt.Logic;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.BuyItem.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using System.Buffers;
using System.Text;
namespace Puniemu.Src.Server.GameServer.Requests.BuyItem.Logic
{
    public class BuyItemHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<BuyItemRequest>(requestJsonString!)!;

            //init res
            var res = await BuyItemResponse.BuildAsync(deserialized.Gdkey);

            //Get item entry
            var json = DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_shop_item_list"];

            var itemList = JsonConvert
                .DeserializeObject<JObject>(json)!["data"]!
                .ToObject<List<ShopItem>>();

            var item = itemList.Where(x => x.GoodsId == deserialized.GoodsId).FirstOrDefault();
            if(item == null)
            {
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(
                        JsonConvert.SerializeObject(new MsgBoxResponse($"Item with ID {deserialized.GoodsId}\nnot found.","Item not found"))));
                return;
            }

            //Check if user has enough money
            if (deserialized.GoodsCount * item.Price > res.UserData.YMoney)
            {
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(
                       JsonConvert.SerializeObject(new MsgBoxResponse($"You don't have enough Y-Money.", "Not enough Y-Money"))));
                return;
            }

            //Subtract price
            var ymoneyToSubtract = item.Price * deserialized.GoodsCount;

            res.UserData.YMoney -= ymoneyToSubtract;

            //Give item
            var userItems = new TableParser.Logic.TableParser<YwpUserItemEntry>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_item"));

            var itemIdx = userItems.FindIndex([deserialized.GoodsId.ToString()]);

            if(itemIdx == -1)
            {
                //If user doesnt already have item, add it to inventory
                userItems.Items.Add(new YwpUserItemEntry { ItemId = deserialized.GoodsId, Count = deserialized.GoodsCount });
            }
            else
            {
                userItems.Items[itemIdx].Count += deserialized.GoodsCount;
            }

            res.YwpUserItem = userItems.ToString();

            var encRes = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res));
            await ctx.Response.WriteAsync(encRes);
        }
    }
}
