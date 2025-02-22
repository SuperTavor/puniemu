using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.GetL5IDStatus.DataClasses
{
    
    public class L5IDStatus 
    {
        //Unknown value. Returned as 1 in the modified L5IDStatus call.
        [JsonProperty("code")]
        public int Code = 1;

        //Likely the player's name in the L5ID account. Since Puniemu does not support linking account, this is a value full of question marks.
        [JsonProperty("name")]
        public string Name = "???????????????";

        //Likely the player's L5 points. Set to 999999999 so the player can unlock L5ID points exclusive things.
        [JsonProperty("point")]
        public long L5Points = 999999999;
    }
}
