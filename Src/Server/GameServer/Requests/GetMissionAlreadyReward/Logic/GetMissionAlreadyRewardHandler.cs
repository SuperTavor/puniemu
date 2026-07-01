using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses.Mission;
using Puniemu.Src.Server.GameServer.Requests.GetMissionAlreadyReward.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.GetRanking.DataClasses;
using Puniemu.Src.TableParser.Logic;
using System.Buffers;
namespace Puniemu.Src.Server.GameServer.Requests.GetMissionAlreadyReward.Logic
{
    public class GetMissionAlreadyRewardHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = System.Text.Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var req = JsonConvert.DeserializeObject<CommonRequest>(requestJsonString!);

            var userMission = new TableParser<YwpUserMission>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(req.Level5UserID, "ywp_user_mission"));
            foreach(var m in userMission.Items)
            {
                if(m.MissionCompleteStatus == MissionCompleteStatus.CompleteRewardAcquired)
                {
                    m.IsAppear = 1;
                }
            }

            var res = new GetMissionAlreadyRewardResponse();
            res.UserMission = userMission.ToString();

            await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));
        }
    }
}
