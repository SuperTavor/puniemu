namespace Puniemu.Src.Server.GameServer.DataClasses.Mission;
public class YwpMstMission {
    public int MissionID { get; set; }
    public int Unk1 { get; set; }
    public string MissionName { get; set; }
    public string MissionDescription { get; set; }
    public string Unk2 { get; set; }
    public string Unk3 { get; set; }
    public int Unk4 { get; set; }
    public int Unk5 { get; set; }
    public int Unk6 { get; set; }
    public int Unk7 { get; set; }
    public int Unk8 { get; set; }
    public MissionType MissionType { get; set; }
    public int Unk9 { get; set; }
    public int Unk10 { get; set; }
    public int Unk11 { get; set; }
    public string RewardName { get; set; }
    public RewardType RewardType { get; set; }
    public int RewardID { get; set; } //For example if the reward type is icon it will be icon ID, if the reward type is yokai it will be yokaiID etc
    public int YMoneySpiritCount { get; set; } //For example if a mission has a reward type of YMoney this defines how much ymoney is given, if spirit it defines how much spirit is given
    public int Unk14 { get; set; }

}

