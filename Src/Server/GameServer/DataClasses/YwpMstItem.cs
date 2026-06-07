namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class YwpMstItem
    {
        public int ItemID { get; set; }
        public ItemType ItemType { get; set; }
        public string ItemName { get; set; }
        public int ItemParam { get; set; }
        public int Unk0 { get; set; }
        public string ItemDescription { get; set; }
        public string ItemIconPath { get; set; }
        public int Unk1 { get; set; } //always empty
        public int Unk2 { get; set; }
    }


    public enum ItemType
    {
        MapWarpItem = 9, //Param is map ID to warp
        CrankCoin = 4, //Param is 1 always
        SoultBooster = 3, //Param is for example 1200 = 1.2x xp to soult
        Exporb = 1, //Param is how much XP to add 
        FuseItem = 5, //Param is 0 always
        Watchpart = 7, //Param is 0 always
        Food = 2, //Param value is food type
        SkillBooster = 10, //Param is always 1
    }
}
