namespace Puniemu.Src.Server.GameServer.Requests.BuyHitodama.DataClasses
{
    public class Good
    {
        //Price in ymoney
        public int Cost;
        //Amount of energy that is given from this good
        public int RewardedHitodama;
        public Good(int cost, int hitodamaReward)
        {
            Cost = cost;
            RewardedHitodama = hitodamaReward;
        }
    }
}
