using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.InitScoreAttack.DataClasses
{
public class InitScoreAttackResponse : CommonResponse
    {
        [JsonProperty("serverDt")]
        public long ServerDt { get; set; }

        [JsonProperty("responseCode")]
        public int ResponseCode { get; set; }

        [JsonProperty("weekSeq")]
        public int WeekSeq { get; set; }

        [JsonProperty("leagueId")]
        public int LeagueId { get; set; }

        // Toutes les autres tables seront ajoutées dynamiquement via GeneralUtils
    }
}