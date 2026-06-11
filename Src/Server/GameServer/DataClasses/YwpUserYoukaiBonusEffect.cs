namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class YwpUserYoukaiBonusEffect
    {
        public long YoukaiID { get; set; }

        public int BonusEffectLevel { get; set; }

        public int BonusEff2ActivatedFlg { get; set; } 

        public int BonusEff3ActivatedFlg { get; set; } //some yokai's second bonus effect is random between 2 options. if the first option is picked, the flag above is activated, if the second, this one
    }
}
