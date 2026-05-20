using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Collections.Concurrent;
using System.Reflection.Metadata.Ecma335;

namespace Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.DataClasses
{
    public static class GachaPoolManager
    {
        //GachaId, rates
        private static Dictionary<int, GachaPoolItem> _gachas = new();

        private static bool _isLoaded = false;

        //gachaid, weightSum
        private static ConcurrentDictionary<int, double> _weightSumCache = new();

        private static void EnsureLoaded()
        {
            if (_isLoaded)
                return;

            var gachaTxt =
                DataManager.Logic.DataManager.GameDataManager.GamedataCache["gacha_pool"];

            _gachas =
                JsonConvert.DeserializeObject<Dictionary<int, GachaPoolItem>>(gachaTxt)
                ?? throw new InvalidDataException("Bad gacha_pool.json");

            _isLoaded = true;
        }

        //returns null if gacha id doesnt exist, else return yokai id and rarity (Rank)
        public static YokaiGachaResult? GetYokai(int gachaId)
        {
            EnsureLoaded();
            if(_gachas.TryGetValue(gachaId, out GachaPoolItem? pool))
            {
                var rarity = RollRarity(pool.Weights, gachaId);
                //Roll yokai
                var yokaisToRoll = pool.Yokais[rarity];
                var roll = Random.Shared.Next(yokaisToRoll.Count);
                return new() { YokaiRank = rarity, YokaiID = yokaisToRoll[roll] };
            }
            else
            {
                return null;
            }
        }

        private static RarityType RollRarity(Dictionary<RarityType, double> weights, int gachaId)
        {
            double totalWeight = 0.0;
            if (_weightSumCache.TryGetValue(gachaId, out double sum)) totalWeight = sum;
            else 
            { 
                totalWeight = weights.Values.Sum();
                _weightSumCache[gachaId] = totalWeight;
            }
            double roll = Random.Shared.NextDouble() * totalWeight;

            double cumulative = 0;

            foreach (var entry in weights)
            {
                cumulative += entry.Value;

                if (roll <= cumulative)
                {
                    return entry.Key;
                }
            }

            throw new InvalidOperationException("Invalid weights for rarity roll.");
        }

    }
}
