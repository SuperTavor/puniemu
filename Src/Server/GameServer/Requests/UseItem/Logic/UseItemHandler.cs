using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.UseItem.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.UserInfoRefresh.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using Puniemu.Src.UserDataManager.Logic;
using System.Buffers;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Requests.UseItem.Logic
{
    public class UseItemHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<UseItemRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";

            //Get item info
            var mstItemJson = DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_item"];
            var mstItemParse = JsonConvert.DeserializeObject<Dictionary<string, object>>(mstItemJson);
            var mstItem = new TableParser<YwpMstItem>((string)mstItemParse["tableData"]);
            YwpMstItem itemInfo;
            try
            {
                itemInfo = mstItem.Items.Where(x => x.ItemID == deserialized.ItemID).First();
            }
            catch
            {
                var errMsg = new MsgBoxResponse("Item not found on server", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errMsg)));
                return;
            }

            var response = new UseItemResponse();
            response.ItemType = itemInfo.ItemType;
            response.YwpUserIconBudge = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_icon_budge");
            response.YwpUserDictionary = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_dictionary");

            var userYoukaiSkill = new TableParser<YwpUserYoukaiSkill>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_youkai_skill"));
            var userYoukai = new TableParser<YwpUserYoukai>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_youkai"));
            var userItem = new TableParser<YwpUserItem>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_item"));

            var itemService = new UseItemService(deserialized.ItemID, deserialized.YoukaiID, userItem, userYoukai, userYoukaiSkill, mstItem);
            try
            {
                if (response.ItemType == ItemType.Exporb)
                {
                    itemService.UseExporb();
                }
                else if (response.ItemType == ItemType.SoultBooster)
                {
                    itemService.UseSoultBooster();
                }
                else if (response.ItemType == ItemType.SkillBooster)
                {
                    itemService.UseSkillBooster();
                }
            }
            catch (KeyNotFoundException)
            {
                var errMsg = new MsgBoxResponse("You don't have the item.", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errMsg)));
                return;
            }
            catch (Exception)
            {
                var errMsg = new MsgBoxResponse("An error has occured.", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errMsg)));
                return;
            }
            response.YwpUserYoukai = userYoukai.ToString();
            response.YwpUserYoukaiSkill = userYoukaiSkill.ToString();
            response.YwpUserItem = userItem.ToString();

            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_item", response.YwpUserItem);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_youkai", response.YwpUserYoukai);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_youkai_skill", response.YwpUserYoukaiSkill);

            await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(response)));
        }
    }
}