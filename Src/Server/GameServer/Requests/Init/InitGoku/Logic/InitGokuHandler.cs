using System.Text;
using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.InitGoku.DataClasses;
using System.Buffers;
using Puniemu.Src.Server.GameServer.DataClasses;
using Newtonsoft.Json.Linq;

namespace Puniemu.Src.Server.GameServer.Requests.InitGoku.Logic
{
    public class InitGokuHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);

            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<InitGokuRequest>(requestJsonString!);

            var res = new InitGokuResponse();
            res.YwpMstGokuStory = JObject.Parse(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_goku_story"])["data"]!.ToObject<List<Dictionary<string, object>>>()!;
            res.YwpMstGokuMenu =JObject.Parse(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_goku_menu"])["data"]!.ToObject<List<Dictionary<string, object>>>()!;
            res.YwpMstGokuYoukaiIntro = JObject.Parse(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_goku_youkai_intro"])["data"]!.ToObject<List<Dictionary<string, object>>>()!;
            res.YwpMstGokuYoukaiIntroRelease = JObject.Parse(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_goku_youkai_intro_release"])["data"]!.ToObject<List<Dictionary<string, object>>>()!;
            
            res.YwpUserIconBudge = await UserDataManager.Logic.DBService.GetYwpUserAsync<string>(deserialized!.Level5UserId!, "ywp_user_icon_budge");
            res.YwpUserGokuStory = await UserDataManager.Logic.DBService.GetYwpUserAsync<List<Dictionary<string, object>>>(deserialized!.Level5UserId!, "ywp_user_goku_story");
            res.YwpUserData = await UserDataManager.Logic.DBService.GetYwpUserAsync<YwpUserData>(deserialized!.Level5UserId!, "ywp_user_data");
            res.YwpUserGokuYoukaiIntroRelease = await UserDataManager.Logic.DBService.GetYwpUserAsync<List<YwpUserGokuYoukaiIntroReleaseEntry>>(deserialized!.Level5UserId!, "ywp_user_goku_youkai_intro_release")!;

            bool found;
            foreach (Dictionary<string, object> item in res.YwpMstGokuYoukaiIntroRelease)
            {
                found = false;
                foreach (YwpUserGokuYoukaiIntroReleaseEntry item2 in res.YwpUserGokuYoukaiIntroRelease!)
                {
                    if (item2.IntroReleasedId == (long)item["introReleaseId"])
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    YwpUserGokuYoukaiIntroReleaseEntry entry = new();
                    entry.UpdateClearFlg = 0; //idk
                    entry.Update = false; //idk
                    entry.UpdateNowValue = 0; //idk
                    entry.IntroReleasedId = (long)item["introReleaseId"];
                    entry.UserId = res.YwpUserData!.UserID;
                    entry.ClearFlg = 1;
                    if ((long)item["clearConditionVal2"] != 0)
                    {
                        entry.ClearFlg = 0;
                    }
                    entry.ReadFlg = 0; //idk
                    entry.MissionType = 1; //idk
                    entry.NowValue = 0; // idk
                    entry.CreateRecord = false; //idk
                    entry.TargetValue = (long)item["clearConditionVal2"];
                    entry.UpdateReadFlg = 0; // idk
                    res.YwpUserGokuYoukaiIntroRelease.Add(entry);
                }
            }
            await UserDataManager.Logic.DBService.SetYwpUserAsync(deserialized!.Level5UserId!, "ywp_user_goku_youkai_intro_release", res.YwpUserGokuYoukaiIntroRelease!);
            var outResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res));
            await ctx.Response.WriteAsync(outResponse);
        }
    }
}
