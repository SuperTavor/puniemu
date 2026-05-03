using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.UpdateTutorialFlag.DataClasses.WibWob
{
    public class Tutorial
    {
        [JsonProperty("tutorialType")]
        public int TutorialType { get; set; }

        [JsonProperty("tutorialStatus")]
        public int TutorialStatus { get; set;  }

        [JsonProperty("tutorialId")] 
        public int TutorialId { get; set;  }
    }
}
