using Newtonsoft.Json;
using Puniemu.Src.UserDataManager;
using Puniemu.Src.ConfigManager;
using Puniemu.Src.Server.GameServer.Requests.UserStageRanking.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;
using System.Buffers;

namespace Puniemu.Src.Server.GameServer.Requests.UserStageRanking.Logic
{
    public class UserStageRankingHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            Console.WriteLine("hello");
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<UserStageRankingRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";
            var userStageRankingData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<dynamic>>(deserialized.Level5UserID, "ywp_user_stage_rank");
            List<object> newList = new List<object>();
            if (userStageRankingData != null) {
                foreach (var element in userStageRankingData)
                {
                    if (element["stageId"] == deserialized.StageID)
                    {
                        newList.Add(element);
                    }
                }
            } else {
                userStageRankingData = new List<object>();
            }
            if (newList.Count == 0)
            {
                Dictionary<string,object> newDict = new Dictionary<string,object>();
                newDict["list"] = new List<object>();
                newDict["stageId"] = deserialized.StageID;
                newList.Add(newDict);
                if (userStageRankingData != null)
                {
                    userStageRankingData.Add(newDict);
                    await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_stage_rank", userStageRankingData);
                }
            }
            var updateProfileResponse = new UserStageRankingResponse(newList);
            var marshalledResponse = JsonConvert.SerializeObject(updateProfileResponse);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
        }
    }
}