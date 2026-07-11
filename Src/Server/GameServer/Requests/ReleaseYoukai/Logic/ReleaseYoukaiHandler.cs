using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Logic.Mst;
using Puniemu.Src.Server.GameServer.Requests.ReleaseYoukai.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using System.Buffers;
using System.Text;
using udm = Puniemu.Src.UserDataManager.Logic.UserDataManager;

namespace Puniemu.Src.Server.GameServer.Requests.ReleaseYoukai.Logic
{
    public class ReleaseYoukaiHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<ReleaseYoukaiRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";
            var userData = await udm.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID, "ywp_user_data");
            var userYokai = new TableParser<YwpUserYoukai>(await udm.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_youkai"));
            var userSkill = new TableParser<YwpUserYoukaiSkill>(await udm.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_youkai_skill"));
            var userBonus = new TableParser<YwpUserYoukaiBonusEffect>(await udm.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_youkai_bonus_effect"));
            var userDict = new TableParser<YwpUserDictionary>(await udm.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_dictionary"));
            var legendReleaseHistory = new TableParser<YwpUserYoukaiLegendReleaseHistory>(await udm.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_youkai_legend_release_history"));
            MstLegendReleaseManager.EnsureLoaded();
            var mstLegendItem = MstLegendReleaseManager.Table.Items.FirstOrDefault(x => x.LegendYokaiID == deserialized.YoukaiID);
            if (mstLegendItem == null)
            {
                var errRes = new MsgBoxResponse("Can't find this legend", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errRes)));
                return;
            }

            //check if the user already did the legend
            if (legendReleaseHistory.Items.FirstOrDefault(x => x.LegendYokaiID == deserialized.YoukaiID) != null)
            {
                var errRes = new MsgBoxResponse("You already did this legend", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errRes)));
                return;
            }

            //Check if the user has all the yokai required
            var userYokaiSeal1 = userYokai.Items.FirstOrDefault(x => x.YoukaiId == mstLegendItem.Yokai1ID);
            var userYokaiSeal2 = userYokai.Items.FirstOrDefault(x => x.YoukaiId == mstLegendItem.Yokai2ID);
            var userYokaiSeal3 = userYokai.Items.FirstOrDefault(x => x.YoukaiId == mstLegendItem.Yokai3ID);
            var userYokaiSeal4 = userYokai.Items.FirstOrDefault(x => x.YoukaiId == mstLegendItem.Yokai4ID);
            var userYokaiSeal5 = userYokai.Items.FirstOrDefault(x => x.YoukaiId == mstLegendItem.Yokai5ID);
            var userYokaiSeal6 = userYokai.Items.FirstOrDefault(x => x.YoukaiId == mstLegendItem.Yokai6ID);

            if (userYokaiSeal1 == null || userYokaiSeal2 == null || userYokaiSeal3 == null || userYokaiSeal4 == null || userYokaiSeal5 == null || userYokaiSeal6 == null)
            {
                var errRes = new MsgBoxResponse("You don't have the required yokai.", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errRes)));
                return;
            }


            //Give the yokai after all the checks
            await YoukaiManager.AddYoukai(userYokai, deserialized.YoukaiID, userSkill, userBonus, deserialized.Level5UserID);
            DictionaryManager.EditDictionary(ref userDict, deserialized.YoukaiID, true, true);
            //Edit the release history
            legendReleaseHistory.Items.Add(new YwpUserYoukaiLegendReleaseHistory { LegendYokaiID = deserialized.YoukaiID });
            var popup = new YokaiWonPopup(deserialized.YoukaiID, userYokai, userSkill);
            var res = new ReleaseYoukaiResponse()
            {
                UserData = userData,
                UserYokai = userYokai.ToString(),
                UserDict = userDict.ToString(),
                UserSkill = userSkill.ToString(),
                UserBonus = userBonus.ToString(),
                Youkai = popup,
                LegendReleaseHistory = legendReleaseHistory.ToString(),
            };

            await udm.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_youkai", res.UserYokai);
            await udm.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_youkai_skill", res.UserSkill);
            await udm.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_youkai_bonus_effect", res.UserBonus);
            await udm.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_dictionary", res.UserDict);
            await udm.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_youkai_legend_release_history", res.LegendReleaseHistory);


            await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));

        }
    }
}
