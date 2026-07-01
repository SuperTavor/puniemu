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
        public static async Task HandleAsync(HttpContext ctx, bool isAlreadyReward)
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
            //Get all the already rewarded missions and make them disappear
            //when GetMissionAlreadyReward is called it will be isAppear 1
            for (int i = 0; i < userMission.Items.Count; i++ )
            {
                var mission = userMission.Items[i];
                if(mission.MissionCompleteStatus == MissionCompleteStatus.CompletePendingReward)
                {
                    userMission.Items.Remove(mission);
                    userMission.Items.Insert(0, mission);
                }

            }
            foreach(var mission in userMission.Items.Where(x => x.MissionCompleteStatus == MissionCompleteStatus.CompleteRewardAcquired))
            {
                mission.IsAppear = Convert.ToInt32(isAlreadyReward);
                mission.Unk = 0;
            }
            var res = new GetMissionResponse();
            res.UserData = userData;
            res.UserMission = userMission.ToString();
            res.MstMission = mstMission;
            res.MstDailyMission = mstDaily;
            var marshalledResponse = JsonConvert.SerializeObject(res);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_mission", userMission.ToString());
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
        }
    }
}
