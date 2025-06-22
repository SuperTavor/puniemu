using Newtonsoft.Json;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.Server.GameServer.DataClasses;
namespace Puniemu.Src.Server.GameServer.Requests.GameRetire.DataClasses
{
    public class GameRetireResponse : PuniResponse
    {
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
        [JsonProperty("eventPoint")]
        public int EventPoint = 0;
        //IDK
        [JsonProperty("eventStatus")]
        public int EventStatus = 0;
        //IDK
        [JsonProperty("responseCodeTeamEvent")]
        public int ResponseCodeTeamEvent = 0;
        //Information about the user's youkai.
        [JsonProperty("userGameResultData")]
        public UserGameResultData UserGameResultData = new();


        public GameRetireResponse()
        {
            ResultCode = 0;
            ResultType = 0;
            NextScreenType = 0;
        }
        
    }
}

