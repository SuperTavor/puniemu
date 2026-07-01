using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.LevelLockOff.DataClasses;
using System.Text;
using System.Buffers;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.GetMission.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses.Mission;
using Puniemu.Src.TableParser.Logic;
using Puniemu.Src.UserDataManager.Logic;
namespace Puniemu.Src.Server.GameServer.Requests.GetMission.Logic
{
    public class GetMissionHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<CommonRequest>(requestJsonString!)!;
            var mstMission = (string)
                JsonConvert.DeserializeObject<Dictionary<string, object>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_mission"])["tableData"];
            var mstDaily = (string)
                JsonConvert.DeserializeObject<Dictionary<string, object>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_daily_event_mission"])["tableData"];
            var userMission = new TableParser<YwpUserMission>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_mission"));
            var userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID, "ywp_user_data");
            //Get all the already rewarded missions and append them as the last ones with isAppear 0
            //when GetMissionAlreadyReward is called it will be isAppear 1
            List<YwpUserMission> alreadyRewardMissions = new();
            foreach(var mission in userMission.Items.Where(x => x.MissionCompleteStatus == MissionCompleteStatus.CompleteRewardAcquired))
            {
                mission.IsAppear = 0;
                alreadyRewardMissions.Add(mission);
                userMission.Items.Remove(mission);
            }

            foreach(var mission in alreadyRewardMissions)
            {
                userMission.Items.Add(mission);
            }
            var res = new GetMissionResponse();
            res.UserData = userData;
            res.UserMission = userMission.ToString();
            res.MstMission = mstMission;
            res.MstDailyMission = mstDaily;
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_mission", res.UserMission);
            var marshalledResponse = JsonConvert.SerializeObject(res);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
        }
    }
}
