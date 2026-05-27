namespace Puniemu.Src.Server.GameServer.DataClasses.Mission
{
    public class YwpUserMission
    {
        public int Unk1;
        public int MissionID;
        public int Unk2;
        public int MissionParam; //for example if the mission is crank 3 times, this will be 3
                                 //however mission param only has 1 param, for example for the tribe bosses its the tribe, not the boss stage
        public int Progress;
    }
}
