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
        DirectAttacker = 20, //when Unk1 in YwpMstYoukaiSkillLevel is 1 its single attacker and when Unk1 is 2 its All Attacker
        SummonOtherPuni = 2, //ex: Komasan soult
        PuniRearranger = 7, //mirapo damona
        MultipleAreaPopper = 22, //kingmera
        BonusBallsMaker = 23, //Mermother
        AttackerAndStunner = 25, //Poofessor
        InflatorBetter = 26, //Slurpent
        CrossAreaPopper = 27, //Darknyan
        AttackerAndHealer = 28, //Camellia
        AttackerAndHPScaling = 31, //Melonyan M
        PopperDissapear = 32, //Deals more Damage the more his puni disapeears ex: Whisper
        AttackerLuckScaling = 34, //Dressed-up Whisper (power changes based on luck)
        AttackerScalingOnPuni = 35, //Prancy-nyan Scale on the number of puni on Board (more is better)
        AllPopperHealerScalingOnPuni = 36, //Fancy-nyan Heal scale on the number of puni erased (more is better)
        BallMakerAndRecoverHp = 29, //BallMaker and recover hp
        AttackerScalingUnity = 37, //attacker with scaling unity
        RandomPopperScaling = 38, //random popper (The more you eliminate, the more damage you do)
        TimeStopperDamage = 39, //Stop time and deal massive damage by linking Yokai Puni
        AttackAndFeverBooster = 40, //Increases Attack and Fever Gauge Gain Rate
        FeverAndSoultGaugeBooster = 42, //Increased Fever Gauge and Soult Gauge Charge Rates
        HealerAndSoultBooster = 43, //Restores HP and increases the Soult gauge
        PopperAndFeverCharger = 45, //Popper and charge Fever Gauge
        BonusBallClearer = 46, //Clear bonus balls to deal damage
        ReflectingBeam = 47, //Fire a beam that reflects off the puzzle area (can change number of reflexion in skill level)
        TapSlasher = 48, //tap yourself will do a slash
        OwnPuniEraserAndOrganizer = 49, //Erase everything own puni and organize the others
        BlackholeFeverBooster = 50, //make a blackhole then increase fever (Umbral Enma)
        PuniTransformerAndSoultBooster = 51, //Transform others Puni and increase others Soult gauge (up to 2 i think)
        RowClearerBonusBallMaker = 52, //Clear two rows of Puni to create a bonus ball
        TapClearArea = 53, //Tap own puni to clear the Puni around
        ConnectPopper = 54, //When a puni is connected, it pops, along with the ones around it.
        AttackBoosterAndInflator = 55, //Atk boosters and inflator
        TraceEraserTime = 56, //Clears the Youkai Puni in the area you traced for a certain amount of time (deadcool)
        TreasureDropper = 57, //Clear Puni by dropping treasures
        TapAreaClearer = 58, //Clears Puni around the spot you tapped
        OrganizeLinkDamageScaling = 59, //The more you organize and link Puni, the more damage you deal (infinite enma)
        Beyblade = 60, //Beyblade (close to extreme popper)
        AttackBoosterAndDamageReducer = 61, //Increase attack and reduce damage taken
        ThreeDirectionSlasher = 62, //Slasher but 3 directions
        TapGiantPuniAndClear = 63, //Tap to create a giant Puni and clear around
        DirectAttackerFeverScaling = 64, //DirectAttacker and more damage in fever
        AllPopperAndStunner = 65, //AllPopper and Stun
        ExtremePopper = 66 //extreme popper
    }
}
