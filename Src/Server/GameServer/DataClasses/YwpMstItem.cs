namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class YwpMstItem
    {
        public int ItemID;
        public ItemType ItemType;
        public string ItemName;
        public int ItemParam;
        public int Unk0;
        public string ItemDescription;
        public string ItemIconPath;
        public int Unk1; //always empty
        public int Unk2;
    }


    public enum ItemType
    {
        MapWarpItem = 9, //Param is map ID to warp
        CrankCoin = 4, //Param is 1 always
        SoultBooster = 3, //Param is for example 1200 = 1.2x xp to soult
        Exporb = 1, //Param is how much XP to add 
        FuseItem = 5, //Param is 0 always
        Watchpart = 7, //Param is 0 always
        Food = 2, //Param value is unknown
    }
}
