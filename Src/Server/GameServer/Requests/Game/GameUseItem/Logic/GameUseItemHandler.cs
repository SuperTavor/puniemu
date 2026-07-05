using Newtonsoft.Json;
using Puniemu.Src.UserDataManager;

using Puniemu.Src.Server.GameServer.Requests.GameUseItem.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;
using System.Buffers;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.TableParser.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.GameUseItem.Logic
{
    public class GameUseItemHandler
    {
        //Puni puni now have a new profile system (with plate, effect and codename) but for some reason in the private server it's always the old app
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<GameUseItemRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";
            var userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID, "ywp_user_data");
            var playerItem = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_item");
            var playerItemTable = new TableParser.Logic.TableParser<YwpUserItem>(playerItem!);
            var item = playerItemTable.Items.FirstOrDefault(x => x.ItemId == deserialized.ItemId);
            if(item == null || item.Count <= 0)
            {
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(new MsgBoxResponse("You don't have the item", "Err"))));
                return;
            }

            item.Count--;
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_item", playerItemTable.ToString());
            await MissionManager.UpdateProgress(deserialized.Level5UserID, GameServer.DataClasses.Mission.MissionType.UseSpecificItemInBattle, deserialized.ItemId);
            await MissionManager.UpdateProgress(deserialized.Level5UserID, GameServer.DataClasses.Mission.MissionType.UseTotalItems, 1);
            var renameResponse = new GameUseItemResponse(userData!, playerItemTable.ToString(), deserialized.ItemId);
            var marshalledResponse = JsonConvert.SerializeObject(renameResponse);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
        }
    }
}
