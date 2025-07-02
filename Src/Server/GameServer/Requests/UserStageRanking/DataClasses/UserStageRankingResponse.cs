using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;

namespace Puniemu.Src.Server.GameServer.Requests.UserStageRanking.DataClasses
{
    public class UserStageRankingResponse: PuniResponse
    {

        [JsonProperty("ywp_user_stage_rank")]
        public List<object> StageRankData { get; set; }

        public UserStageRankingResponse(List<object> newListData)
        {
            this.ResultCode = 0;
            this.NextScreenType = 0;
            this.StageRankData = newListData;
            this.ResultType = 0;
        }
    }
}
