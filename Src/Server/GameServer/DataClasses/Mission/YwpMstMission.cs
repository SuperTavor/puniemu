namespace Puniemu.Src.Server.GameServer.DataClasses.Mission;
public class YwpMstMission {
    public int MissionID; //First 3 numbers from the left are Series ID and they unlock one after another
    public int Unk1;
    public string MissionName;
    public string MissionDescription;
    public string Unk2;
    public string Unk3;
    public int Unk4;
    public int Unk5;
    public int Unk6;
    public int Unk7;
    public int Unk8;
    public MissionType MissionType;
    public int Unk9;
    public int Unk10;
    public int Unk11;
    public string RewardName;
    public MissionRewardType RewardType;
    public int RewardID; //For example if the reward type is icon it will be icon ID, if the reward type is yokai it will be yokaiID etc
    public int YMoneySpiritCount; //For example if a mission has a reward type of YMoney this defines how much ymoney is given, if spirit it defines how much spirit is given
    public int Unk14;
}

