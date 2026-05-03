using Newtonsoft.Json;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text.Json.Serialization;
namespace Puniemu.Src.Server.GameServer.Requests.Login.DataClasses.WibWob
{
    public class LoginResponse : CommonLoginResponse
    {

        [JsonProperty("robTreasureExecFlg")]
        public bool RobTreasureExecFlag { get; set; }

    }
}

