using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.UpdateTutorialFlag.DataClasses.WibWob;
using Puniemu.Src.Server.GameServer.Requests.UpdateTutorialFlag.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using System.Buffers;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Requests.UpdateTutorialFlag.Logic.WibWob
{
    public class UpdateTutorialFlagHandler
    {

        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Response.ContentType = "application/json";
            ctx.Request.EnableBuffering();
            var buffer = new MemoryStream();
            await ctx.Request.Body.CopyToAsync(buffer);
            buffer.Seek(0, SeekOrigin.Begin);
            string? requestJsonString;
            using (var reader = new StreamReader(buffer, Encoding.UTF8))
            {
                var readResult = await reader.ReadToEndAsync();
                requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(readResult);
            }
            var deserialized = JsonConvert.DeserializeObject<UpdateTutorialFlagRequest>(requestJsonString!);
            var tutorialList = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<Tutorial>>(deserialized!.Level5UserId!, "ywp_user_tutorial_list");
            var tuto = tutorialList.Where(x => x.TutorialId == deserialized.TutorialId).FirstOrDefault();
            tuto.TutorialStatus = deserialized.TutorialStatus;
            tuto.TutorialType = deserialized.TutorialType;

            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserId!,"ywp_user_tutorial_list",tutorialList);
            var userdata = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized!.Level5UserId!, "ywp_user_data");
            var res = new UpdateTutorialFlagResponse(tutorialList, userdata!);
            await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));
        }
    }
}
