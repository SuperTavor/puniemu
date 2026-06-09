namespace Puniemu.Src.Server.GameServer.DataClasses
{

    //WIP!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public class YwpMstYoukaiSkill
    {
        public int SoultID { get; set; } //Same as yokai ID

        public string SoultName { get; set; }

        public SoultType SoultType { get; set; }

        public string SoultDescription { get; set; }

        public int SoultProperty1 { get; set; }

        public int SoultProperty2 { get; set; } //Unknown what these do probably control the power of the soult and other params

        public string Soult3DAnimName { get; set; } //filename of the EZ without extension

        public string Soult2DAnimName { get; set; } //filename of the .dat file including extension

        //rest idk
    }

    //WIP
    public enum SoultType
    {
        None = -1,
        Befriender = 11,
        RandomPopper = 3,
        ScoreBooster = 16,
        AttackBooster = 15,
        SingleAttackerAndBefriender = 33,
        RangePopper = 1, //Includes "all popper"s
        AttackBoosterAndHeal = 24,
        DefenseBooster = 17,
        Healer = 9,
        BallMaker = 8,
        MoneyBooster = 13,
        Hider = 5,
        Inflator = 4,
        Stunner = 18,
        ItemDropBooster = 14,
        ExpBooster = 12,
        Tracer = 30,
        Slasher = 44,
        NoFillerTracer = 41,

    }
}
