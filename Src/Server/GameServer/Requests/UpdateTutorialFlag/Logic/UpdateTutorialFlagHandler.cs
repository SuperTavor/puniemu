using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.UpdateTutorialFlag.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using System.Buffers;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Requests.UpdateTutorialFlag.Logic
{
    public class UpdateTutorialFlagHandler
    {

        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<UpdateTutorialFlagRequest>(requestJsonString!);
            var tutorialList = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserId!, "ywp_user_tutorial_list");
            var tutorialListTable = new TableParser.Logic.TableParser(tutorialList!);
            tutorialListTable = TutorialFlagManager.EditTutorialFlg(tutorialListTable, deserialized.TutorialType, deserialized.TutorialId, deserialized.TutorialStatus);
            var modifiedTutoList = tutorialListTable.ToString();
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserId!,"ywp_user_tutorial_list",modifiedTutoList);
            var userdata = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized!.Level5UserId!, "ywp_user_data");
            var res = new UpdateTutorialFlagResponse(modifiedTutoList, userdata!);
            await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));
        }
    }
}
