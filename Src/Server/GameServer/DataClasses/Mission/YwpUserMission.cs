namespace Puniemu.Src.Server.GameServer.DataClasses.Mission
{
    public class YwpUserMission
    {
        public int SeqNo { get; set; }
        public int MissionID { get; set; }
        public int IsAppear { get; set; }
        //for example if the mission is crank 3 times, this will be 3 
        public int MissionParamTarget { get; set; } //however mission param only has 1 param, for example for the tribe bosses its the tribe, not the boss stage
        public int MissionParamProgress { get; set; }
        public MissionCompleteStatus MissionCompleteStatus { get; set; }
        public MissionNewStatus NewStatus { get; set; }

        public int Unk { get; set; } = 1;
    }
}
