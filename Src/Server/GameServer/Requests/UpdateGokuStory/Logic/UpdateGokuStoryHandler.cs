using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.UpdateGokuStory.DataClasses;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Requests.UpdateGokuStory.Logic
{
    public class UpdateGokuStoryHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            using var reader = new StreamReader(ctx.Request.Body, Encoding.UTF8, leaveOpen: true);
            var encRequest = await reader.ReadToEndAsync();
            ctx.Request.Body.Position = 0;

            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<UpdateGokuStoryRequest>(requestJsonString!);

            UpdateGokuStoryResponse res = new();

            res.YwpUserData = await DBService.Logic.DBService.GetYwpUserAsync<YwpUserData>(deserialized!.Level5UserId!, "ywp_user_data");
            res.YwpUserIconBudge = await DBService.Logic.DBService.GetYwpUserAsync<string>(deserialized!.Level5UserId!, "ywp_user_icon_budge");
            res.YwpUserGokuStory = await DBService.Logic.DBService.GetYwpUserAsync<List<YwpUserGokuStoryEntry>>(deserialized!.Level5UserId!, "ywp_user_goku_story");

            bool found = false;
            foreach (YwpUserGokuStoryEntry entry in res.YwpUserGokuStory!)
            {
                if (entry.GokuStoryId == deserialized.GokuStoryId)
                {
                    found = true;
                    break;
                }
            }
            if (!found) {
                YwpUserGokuStoryEntry entry = new();
                entry.GokuStoryId = deserialized.GokuStoryId;
                res.YwpUserGokuStory.Add(entry);
            }
            await DBService.Logic.DBService.SetYwpUserAsync(deserialized!.Level5UserId!, "ywp_user_goku_story", res.YwpUserGokuStory!);
            var outResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res));
            await ctx.Response.WriteAsync(outResponse);
        }
    }
}
