using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses.Mission;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using System.Collections.Concurrent;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.Logic
{
    /// Controls what happens when a rolled yokai is already at max level (fully unlocked/maxed).
    public enum GachaMaxedYokaiMode
    {
        /// Default/legacy behavior: convert the maxed yokai into its associated convert-item.
        ConvertToItem,
        
        //maxable crank
        RerollUntilValid
    }

    public static class GachaPoolManager
    {
        //GachaId, rates
        public static Dictionary<int, GachaPoolItem> Gachas = new();

        private static bool _isLoaded = false;

        //gachaid, weightSum
        private static ConcurrentDictionary<int, double> _weightSumCache = new();

        public static void EnsureLoaded()
        {
            if (_isLoaded)
                return;

            var gachaTxt =
                DataManager.Logic.DataManager.GameDataManager.GamedataCache["gacha_pool"];

            Gachas =
                JsonConvert.DeserializeObject<Dictionary<int, GachaPoolItem>>(gachaTxt)
                ?? throw new InvalidDataException("Bad gacha_pool.json");

            _isLoaded = true;
        }



        public static async Task<GachaPrize> RegisterYokaiAndGetPrize(long yokaiId, CapsuleColor capsule, RarityType rank,
            TableParser<YwpUserYoukai> userYokaiTable, TableParser<YwpUserYoukaiSkill> userSkillTable, TableParser<YwpUserDictionary> dictionaryListTable,
            TableParser<YwpUserItem> userItem, int gachaId, TableParser<YwpUserYoukaiBonusEffect> userBonus, string gdkey)
        {
            var prizeType = PrizeType.Yokai;
            var getType = YokaiWonPopup.CheckGetType(yokaiId, userYokaiTable, userSkillTable);
            YokaiWonPopup? yokai = null;
            ConvertItemInfo? convertItemInfo = null;
            ItemWonPopup? itemForConvert = null;
            if (getType != YokaiGetType.MaxLevel)
            {
                yokai = new YokaiWonPopup(yokaiId, userYokaiTable, userSkillTable);
            }
            else
            {
                convertItemInfo = new()
                {
                    OGPrizeType = PrizeType.Yokai,
                    OGPrizeID = yokaiId,
                    SkillMaxYoukaiID = yokaiId
                };

                itemForConvert = Gachas[gachaId].ConvertItem[((int)rank).ToString()];

                prizeType = PrizeType.ConvertItem;
                ItemManager.AddItem(userItem, itemForConvert.ItemId, item_count: 1);
            }

            //Register yokai if not registered
            await YoukaiManager.AddYoukai(userYokaiTable, yokaiId, userSkillTable, userBonus, gdkey);

            DictionaryManager.EditDictionary(ref dictionaryListTable, yokaiId, true, true);

            return new GachaPrize
            {
                Item = itemForConvert,
                CapsuleColor = capsule,
                PrizeType = prizeType,
                Icon = null,
                YMoney = null,
                Youkai = yokai,
                RarityType = rank,
                ConvertItemInfo = convertItemInfo

            };
        }

        //Registers an item prize (extracted so both the normal path and the reroll path can share it).
        private static YwpUserItem RegisterItemAndGetPrize(TableParser<YwpUserItem> userItemTable, long resultItem, out ItemWonPopup itemWon)
        {
            var idx = userItemTable.FindIndex([resultItem.ToString()]);
            if (idx == -1)
            {
                userItemTable.Items.Add(new YwpUserItem
                {
                    ItemId = resultItem,
                    Count = 1,
                });
            }
            else
            {
                userItemTable.Items[idx].Count += 1;
            }

            itemWon = new ItemWonPopup
            {
                Count = 1,
                ItemId = resultItem,
                IsLimitOver = 0
            };

            return userItemTable.Items[idx == -1 ? userItemTable.Items.Count - 1 : idx];
        }

        //returns null if gacha id doesnt exist, else return yokai id and rarity (Rank)
        //
        //mode controls what happens if the rolled pool comes up maxed:
        //  - ConvertToItem (default, legacy behavior): converts the maxed yokai into its convert-item.
        //  - RerollUntilValid:
        //      1. If the rolled yokai is maxed, first tries the other non-maxed yokai in that
        //         same weighted bucket (exact, single pass, no retry probability).
        //      2. If that whole bucket is maxed out, rerolls the *pool selection itself* among
        //         the remaining pools (other yokai rarities, item pools) — so a maxed-out rarity
        //         tier doesn't just dead-end into a convert-item, it can land on a completely
        //         different reward type. Each exhausted pool is excluded so this always makes
        //         forward progress and terminates in at most (pool count) iterations.
        //      3. Only if literally every yokai across every pool is maxed (so no yokai reward
        //         is obtainable at all) does it fall back to yielding a plain item.
        public static async Task<GachaPrize>? CrankReward(int gachaId, TableParser<YwpUserYoukai> userYokaiTable, TableParser<YwpUserYoukaiSkill> userSkillTable,
            TableParser<YwpUserDictionary> dictionaryListTable, TableParser<YwpUserItem> userItemTable, TableParser<YwpUserYoukaiBonusEffect> userBonus, string gdkey,
            GachaMaxedYokaiMode mode)
        {
            EnsureLoaded();
            //Currently items still have a placeholder capsule
            //Yokai have them by rank
            var placeHolderCapsule = CapsuleColor.Gray;

            if (!Gachas.TryGetValue(gachaId, out GachaPoolItem? gacha))
            {
                return null;
            }

            HashSet<string>? excludedPools = null;
            long lastMaxedYokai = -1;
            RarityType lastMaxedRank = default;

            while (true)
            {
                var pool = RollPool(gacha.Weights, gachaId, excludedPools);

                if (pool == null)
                {
                    return GetFallbackItemPrize(gacha, gachaId, userItemTable, lastMaxedYokai, lastMaxedRank, placeHolderCapsule);
                }

                //Roll item
                if (pool.StartsWith("i"))
                {
                    var itemsToRoll = gacha.Items[pool];
                    var roll = Random.Shared.Next(itemsToRoll.Count);
                    var resultItem = itemsToRoll[roll];

                    RegisterItemAndGetPrize(userItemTable, resultItem, out var itemWon);

                    return new GachaPrize
                    {
                        Youkai = null,
                        Icon = null,
                        YMoney = null,
                        CapsuleColor = placeHolderCapsule,
                        PrizeType = PrizeType.Item,
                        RarityType = RarityType.RarityB,
                        Item = itemWon,
                        ConvertItemInfo = null
                    };
                }
                //Roll yokai
                else
                {
                    int value = int.Parse(pool);

                    if (!Enum.IsDefined(typeof(RarityType), value))
                    {
                        throw new InvalidDataException("Invalid rarity for yokai pool in gachaPool");
                    }

                    var rank = (RarityType)value;
                    var yokaisToRoll = gacha.Yokais[pool];
                    var resultYokai = PickYokai(yokaisToRoll, gacha.RateUp);

                    if (mode == GachaMaxedYokaiMode.RerollUntilValid)
                    {
                        var getType = YokaiWonPopup.CheckGetType(resultYokai, userYokaiTable, userSkillTable);
                        if (getType == YokaiGetType.MaxLevel)
                        {
                            var candidates = new List<long>(yokaisToRoll.Count);
                            foreach (var candidate in yokaisToRoll)
                            {
                                if (candidate == resultYokai)
                                    continue; // already know this one is maxed

                                if (YokaiWonPopup.CheckGetType(candidate, userYokaiTable, userSkillTable) != YokaiGetType.MaxLevel)
                                    candidates.Add(candidate);
                            }

                            if (candidates.Count > 0)
                            {
                                resultYokai = PickYokai(candidates, gacha.RateUp);
                            }
                            else
                            {
                                //Whole bucket is maxed — this pool is a dead end. Exclude it and
                                //reroll pool selection among what's left (other rarities, items).
                                lastMaxedYokai = resultYokai;
                                lastMaxedRank = rank;

                                excludedPools ??= new HashSet<string>();
                                excludedPools.Add(pool);
                                continue;
                            }
                        }
                    }

                    return await RegisterYokaiAndGetPrize(resultYokai, GachaService.CAPSULE_CLRS[rank], rank, userYokaiTable, userSkillTable, dictionaryListTable, userItemTable, gachaId, userBonus, gdkey);
                }
            }
        }

        //Used only when RerollUntilValid has excluded every pool (every yokai in every
        //rarity bucket is already maxxxed) and there were no item pools to land on along the way.
        //Reusses the existing convert-item mapping so the player still gets a concrete, valid item
        //rather than nothing.
        private static GachaPrize GetFallbackItemPrize(GachaPoolItem gacha, int gachaId, TableParser<YwpUserItem> userItemTable,
            long lastMaxedYokai, RarityType lastMaxedRank, CapsuleColor placeHolderCapsule)
        {
            var itemForConvert = gacha.ConvertItem[((int)lastMaxedRank).ToString()];
            ItemManager.AddItem(userItemTable, itemForConvert.ItemId, item_count: 1);

            return new GachaPrize
            {
                Youkai = null,
                Icon = null,
                YMoney = null,
                CapsuleColor = placeHolderCapsule,
                PrizeType = PrizeType.ConvertItem,
                RarityType = lastMaxedRank,
                Item = itemForConvert,
                ConvertItemInfo = new ConvertItemInfo
                {
                    OGPrizeType = PrizeType.Yokai,
                    OGPrizeID = lastMaxedYokai,
                    SkillMaxYoukaiID = lastMaxedYokai
                }
            };
        }

        //Picks one yokai from a bucket. If the gacha defines rateUp weights, the pick is
        //weighted (a yokai's weight is its rateUp value, or 1.0 if it isn't listed); otherwise
        //every yokai in the bucket is equally likely (legacy uniform behavior).
        private static long PickYokai(List<long> yokais, Dictionary<long, double>? rateUp)
        {
            if (rateUp == null || rateUp.Count == 0)
                return yokais[Random.Shared.Next(yokais.Count)];

            double totalWeight = 0;
            foreach (var yokai in yokais)
                totalWeight += rateUp.TryGetValue(yokai, out var w) ? w : 1.0;

            //No positive weight in this bucket (e.g. every member explicitly set to 0) -> fall back to uniform.
            if (totalWeight <= 0)
                return yokais[Random.Shared.Next(yokais.Count)];

            double roll = Random.Shared.NextDouble() * totalWeight;
            double cumulative = 0;
            foreach (var yokai in yokais)
            {
                cumulative += rateUp.TryGetValue(yokai, out var w) ? w : 1.0;
                if (roll <= cumulative)
                    return yokai;
            }

            return yokais[yokais.Count - 1]; //floating-point safety net
        }

        private static string? RollPool(Dictionary<string, double> weights, int gachaId, ISet<string>? excludedPools = null)
        {
            if (excludedPools == null || excludedPools.Count == 0)
            {
                double totalWeight;
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

                throw new InvalidOperationException("Invalid weights for pool roll.");
            }
            else
            {
                double totalWeight = 0.0;
                foreach (var entry in weights)
                {
                    if (!excludedPools.Contains(entry.Key))
                        totalWeight += entry.Value;
                }

                if (totalWeight <= 0)
                    return null; // every pool has been excluded — nothing left to roll

                double roll = Random.Shared.NextDouble() * totalWeight;
                double cumulative = 0;
                string? last = null;

                foreach (var entry in weights)
                {
                    if (excludedPools.Contains(entry.Key))
                        continue;

                    last = entry.Key;
                    cumulative += entry.Value;

                    if (roll <= cumulative)
                    {
                        return entry.Key;
                    }
                }

                return last;
            }
        }

    }
}