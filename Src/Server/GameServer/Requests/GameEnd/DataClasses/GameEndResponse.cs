using Newtonsoft.Json;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.Server.GameServer.DataClasses;
namespace Puniemu.Src.Server.GameServer.Requests.GameEnd.DataClasses
{
    public class GameEndResponse : PuniResponse
    {
        //IDK
        [JsonProperty("eventTeamPoint")]
        public int EventTeamPoint = 0;
        //IDK
        [JsonProperty("eventPointUpItemId")]
        public int EventPointUpItemId = 0;
        //IDK
        [JsonProperty("eventSubPoint")]
        public int EventSubPoint = 0;
        //IDK
        [JsonProperty("teamEventButtonHiddenFlg")]
        public int TeamEventButtonHiddenFlg = 0;
        //IDK
        [JsonProperty("natureEventPoint")]
        public double NatureEventPoint = 0.0;
        //Information about result yokai
        [JsonProperty("userYoukaiResultList")]
        public List<UserYoukaiResultListRes> UserYoukaiResultList = new();
        //IDK
        [JsonProperty("eventStatusCode")]
        public int EventStatusCode = 0;
        //IDK
        [JsonProperty("eventPoint")]
        public int EventPoint = 0;
        //IDK
        [JsonProperty("hpRecoverFlg")]
        public int HpRecoverFlg = 0;
        //IDK
        [JsonProperty("eventStatus")]
        public int EventStatus = 0;
        //IDK
        [JsonProperty("responseCodeTeamEvent")]
        public int ResponseCodeTeamEvent = 0;
        //Information about the user's youkai.
        [JsonProperty("truncateItemList")]
        public List<object> TruncateItemList = new();
        //items result list
        [JsonProperty("userItemResultList")]
        public List<UserItemResultList> UserItemResultList = new();
        //idk list
        [JsonProperty("lockedStageResultList")]
        public List<LockedStageResultList> LockedStageResultList = new();
        [JsonProperty("userGameResultData")]
        public UserGameResultData UserGameResultData = new();
        [JsonProperty("youkai")]
        public YokaiWonPopup? YoukaiPopupResult { get; set; }


        public GameEndResponse()
        {
            ResultCode = 0;
            ResultType = 0;
            NextScreenType = 0;
        }
        
    }
}

