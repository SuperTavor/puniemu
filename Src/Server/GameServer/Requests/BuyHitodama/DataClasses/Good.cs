namespace Puniemu.Src.Server.GameServer.Requests.BuyHitodama.DataClasses
{
    public class Good
    {
        //Price in ymoney
        public int Cost;
        //Amount of energy that is given from this good (no sale)
        public int RewardedHitodama;
        //Amount of energy that is given in addition that RewardedHitodama (sale)
        public int BonusSalesHitodama;
        public Good(int cost, int hitodamaReward, int hitodamaBonus)
        {
            Cost = cost;
            RewardedHitodama = hitodamaReward;
            BonusSalesHitodama = hitodamaBonus;
        }
    }
}
