namespace Puniemu.Src.Server.GameServer.Requests.BuyHitodama.DataClasses
{
    public static class HitodamaGoods
    {
        public static readonly Dictionary<int, Good> Goods = new()
        {
            {1001, new(cost:1000,hitodamaReward:5,hitodamaBonus:0)},
            {1002, new(cost:3000,hitodamaReward:15,hitodamaBonus:5)},
            {1003, new(cost:6000,hitodamaReward:30,hitodamaBonus:20)}
        };
    }
}
