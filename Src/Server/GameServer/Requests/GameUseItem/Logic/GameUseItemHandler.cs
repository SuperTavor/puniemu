using Newtonsoft.Json;
using Puniemu.Src.UserDataManager;
using Puniemu.Src.ConfigManager;
using Puniemu.Src.Server.GameServer.Requests.GameUseItem.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;
using System.Buffers;

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
            var playerItemTable = new TableParser.Logic.TableParser(playerItem!);

            var idx = playerItemTable.FindIndex([deserialized.ItemId.ToString()!]);
            if (idx != -1)
            {
                if (int.Parse(playerItemTable.Table[idx][1]) > 0)
                {
                    playerItemTable.Table[idx][1] = (int.Parse(playerItemTable.Table[idx][1]) - 1).ToString();
                }
            }
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_item", playerItemTable.ToString());
            var renameResponse = new GameUseItemResponse(userData!, playerItemTable.ToString(), deserialized.ItemId);
            var marshalledResponse = JsonConvert.SerializeObject(renameResponse);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
        }
    }
}
