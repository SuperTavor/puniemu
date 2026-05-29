namespace Puniemu.Src.Server.GameServer.DataClasses.Mission
{
    public class YwpUserMission
    {
        public int MissionIDWithSeries;
        public int MissionID;
        public int IsAppear;
        //for example if the mission is crank 3 times, this will be 3 
        public int MissionParamTarget; //however mission param only has 1 param, for example for the tribe bosses its the tribe, not the boss stage
        public int MissionParamProgress;
        public MissionCompleteStatus MissionCompleteStatus;
        public MissionNewStatus NewStatus;
    }
}
