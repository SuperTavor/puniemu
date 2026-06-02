using Newtonsoft.Json;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.Server.GameServer.DataClasses;
namespace Puniemu.Src.Server.GameServer.Requests.GameEnd.DataClasses
{
    public class GameEndResponse : CommonResponse
    {
        // GameEnd, GameRetire
        [JsonProperty("teamEventButtonHiddenFlg")]
        public int TeamEventButtonHiddenFlg = 0; // idk
        [JsonProperty("natureEventPoint")]
        public double NatureEventPoint = 0.0; // idk
        [JsonProperty("userYoukaiResultList")]
        public List<UserYoukaiResultListRes> UserYoukaiResultList = new(); //Information about result/user yokai
        [JsonProperty("eventPoint")]
        public int EventPoint = 0; // idk
        [JsonProperty("eventStatus")]
        public int EventStatus = 0; // idk
        [JsonProperty("responseCodeTeamEvent")]
        public int ResponseCodeTeamEvent = 0; // idk
        [JsonProperty("userGameResultData")]
        public UserGameResultData UserGameResultData = new();

        // GameEnd
        [JsonProperty("eventTeamPoint")]
        public int EventTeamPoint = 0; // idk
        [JsonProperty("eventPointUpItemId")]
        public int EventPointUpItemId = 0; // idk
        [JsonProperty("eventSubPoint")]
        public int EventSubPoint = 0; // idk
        [JsonProperty("eventStatusCode")]
        public int EventStatusCode = 0; // idk
        [JsonProperty("hpRecoverFlg")]
        public int HpRecoverFlg = 0; // idk
        [JsonProperty("truncateItemList")]
        public List<object> TruncateItemList = new(); //idk list
        [JsonProperty("userItemResultList")]
        public List<UserItemResultList> UserItemResultList = new(); //items result list
        [JsonProperty("lockedStageResultList")]
        public List<LockedStageResultList> LockedStageResultList = new(); // idk
        [JsonProperty("youkai")]
        public YokaiWonPopup? YoukaiPopupResult { get; set; } // gived youkai


        public GameEndResponse()
        {
            ResultCode = 0;
            ResultType = 0;
            NextScreenType = 0;
        }
    }
}

