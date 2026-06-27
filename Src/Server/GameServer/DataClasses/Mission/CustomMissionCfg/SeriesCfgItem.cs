using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses.Mission.CustomMissionCfg
{
    
    //This is not for game data from the game, its a custom format for puniemu (This mission info is available on the OG server but is not exposed to the client so we recreate it)
    public class SeriesCfgItem
    {
        public int SeriesID;
        public string SeriesName;
        public List<MissionCfgItem> Missions;
    }
}
