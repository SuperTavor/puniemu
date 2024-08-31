namespace Puniemu.Src.Server.GameServer.Requests.BuyHitodama.DataClasses
{
    public static class HitodamaGoods
    {
        public static readonly Dictionary<int, Good> Goods = new()
        {
            {1001, new(cost:1000,hitodamaReward:5)}
        };
    }
}
