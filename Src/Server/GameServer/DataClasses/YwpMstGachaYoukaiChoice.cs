namespace Puniemu.Src.Server.GameServer.DataClasses
{
    //Says which yokai can be provided in requestYoukaiId in the gacha
    public class YwpMstGachaYoukaiChoice
    {
        public int GachaID { get; set; }

        public int YokaiID { get; set; }
    }
}
