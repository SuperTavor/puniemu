using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.NHNCrypt.Logic;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Requests.BuyItem.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using Puniemu.Src.UserDataManager.Logic;
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
            var userShop = new TableParser<YwpUserShopItemUnlock>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_shop_item_unlock"));
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

            if (deserialized.GoodsCount <= 0 || deserialized.GoodsCount > 99)
            {
                 await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(
                     JsonConvert.SerializeObject(new MsgBoxResponse($"Invalid quantity.", "Error"))));
                 return;
            }

            if(item.LockConditionFlg == 1)
            {
                var userShopitem = userShop.Items.Where(x => x.ItemID == item.ItemId);
                if(userShopitem == null)
                {
                    await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(
                        JsonConvert.SerializeObject(new MsgBoxResponse($"Item not unlocked", "Item not found"))));
                    return;
                }
            }
            //Check if user has enough money
            if (deserialized.GoodsCount * item.Price > res.UserData.YMoney)
            {
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(
                       JsonConvert.SerializeObject(new MsgBoxResponse($"You don't have enough Y-Money.", "Not enough Y-Money"))));
                return;
            }

            //Check if daily limit not reached
            if (item.LimitCnt > 0)
            {
                await ShopLimitManager.CheckShopLimitReset(deserialized.Gdkey);
                var userRemainCntStr = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_shop_item_remain_cnt");
                TableParser<YwpUserShopItemRemainCnt> userRemainCnt = new(userRemainCntStr);
                var remainCountItem = userRemainCnt.Items.FirstOrDefault(x => x.ItemID == item.GoodsId);
                if (remainCountItem == null)
                { 
                    remainCountItem = new YwpUserShopItemRemainCnt() { ItemID = deserialized.GoodsId, AlreadyBought = 0};
                    userRemainCnt.Items.Add(remainCountItem);
                }
                //Check if can still buy
                if(remainCountItem.AlreadyBought + deserialized.GoodsCount <= item.LimitCnt)
                {
                    remainCountItem.AlreadyBought += deserialized.GoodsCount;
                    res.YwpUserShopItemRemainCount = userRemainCnt.ToString();
                    await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Gdkey,"ywp_user_shop_item_remain_cnt", res.YwpUserShopItemRemainCount);
                }
                else
                {
                    await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(
                       JsonConvert.SerializeObject(new MsgBoxResponse($"Item is out of stock for today.", "Out of stock"))));
                    return;
                }
            }
           

            //Subtract price
            var ymoneyToSubtract = item.Price * deserialized.GoodsCount;

            res.UserData.YMoney -= ymoneyToSubtract;

            //Give item
            var userItems = new TableParser.Logic.TableParser<YwpUserItem>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_item"));

            var itemIdx = userItems.FindIndex([deserialized.GoodsId.ToString()]);

            if(itemIdx == -1)
            {
                //if doesnt have item add to inventory
                userItems.Items.Add(new YwpUserItem { ItemId = deserialized.GoodsId, Count = deserialized.GoodsCount });
            }
            else
            {
                userItems.Items[itemIdx].Count += deserialized.GoodsCount;
            }

            var userMission = await MissionManager.UpdateProgress(deserialized.Gdkey, GameServer.DataClasses.Mission.MissionType.TotalPurchaseShop, 1, null, true);
            await MissionManager.UpdateProgress(deserialized.Gdkey, GameServer.DataClasses.Mission.MissionType.BuySpecificItemAtShop, deserialized.GoodsId, userMission, true);
            await MissionManager.SaveUserMission(deserialized.Gdkey, userMission);
            res.YwpUserItem = userItems.ToString();
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Gdkey, "ywp_user_item", res.YwpUserItem);
            var encRes = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res));
            await ctx.Response.WriteAsync(encRes);
        }
    }
}
