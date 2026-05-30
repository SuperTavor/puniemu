using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using System.Collections.Concurrent;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.Logic
{
    public static class GachaPoolManager
    {
        //GachaId, rates
        public static Dictionary<int, GachaPoolItem> Gachas = new();

        private static bool _isLoaded = false;

        //gachaid, weightSum
        private static ConcurrentDictionary<int, double> _weightSumCache = new();

        private const int MAX_SOULT = 5;

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

        private static YokaiGetType CheckGetType(long yokaiId, TableParser<YwpUserYoukai> table)
        {
            YokaiGetType getType = YokaiGetType.SoultLevelUp;
            if (YoukaiManager.GetYoukaiIndex(table, yokaiId) < 0)
            {
                getType = YokaiGetType.NewYokai;
            }
            return getType;
        }

        public static GachaPrize RegisterYokaiAndGetPrize(long yokaiId, CapsuleColor capsule, RarityType rank, TableParser<YwpUserYoukai> userYokaiTable, TableParser<YwpUserYoukaiSkill> userSkillTable, TableParser.Logic.TableParser dictionaryListTable)
        {
            var getType = CheckGetType(yokaiId, userYokaiTable);
            SkillResult? skill = GachaService.CompureSkillPctg(userSkillTable, yokaiId);

            //Register yokai
            YoukaiManager.AddYoukai(userYokaiTable, yokaiId, userSkillTable);

            dictionaryListTable = DictionaryManager.EditDictionary(dictionaryListTable, yokaiId, false, true);

            return new GachaPrize
            {
                Item = null,
                CapsuleColor = capsule,
                PrizeType = PrizeType.Yokai,
                Icon = null,
                YMoney = null,
                Youkai = new YokaiWonPopup
                {
                    BonusEffectLevelAfter = 1,
                    StrongSkillLevelBefore = 0,
                    BonusEffectLevelBefore = 1,
                    LegendYoukaiId = 0,
                    StrongSkillLevelAfter = 0,
                    IsWBonusEffectOpen = false,
                    LevelAfter = 1,
                    LevelBefore = 1,
                    GetTypes = getType,
                    YoukaiId = yokaiId,
                    ReleaseType = 0,
                    Skill = skill,
                    ExchgYmoney = 0,
                    Exp = 0,
                    LimitLevelBefore = 0,
                    LimitLevelAfter = 0,
                    ReleaseLevelType = 0

                },
                RarityType = rank,
                ConvertItemInfo = null

            };
        }
        //returns null if gacha id doesnt exist, else return yokai id and rarity (Rank)
        public static GachaPrize? CrankReward(int gachaId, TableParser<YwpUserYoukai> userYokaiTable, TableParser<YwpUserYoukaiSkill> userSkillTable, TableParser.Logic.TableParser dictionaryListTable, TableParser<YwpUserItem> userItemTable)
        {
            EnsureLoaded();
            //Currently items still have a placeholder capsule
            //Yokai have them by rank
            var placeHolderCapsule = CapsuleColor.Gray;
            if(Gachas.TryGetValue(gachaId, out GachaPoolItem? gacha))
            {

                var pool = RollPool(gacha.Weights, gachaId);
                //Roll item
                if(pool.StartsWith("i"))
                {
                    var itemsToRoll = gacha.Items[pool];
                    var roll = Random.Shared.Next(itemsToRoll.Count);
                    var resultItem = itemsToRoll[roll];
                    //Register item
                    var idx = userItemTable.FindIndex([resultItem.ToString()]);
                    if(idx == -1)
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
                    return new GachaPrize
                    {
                        Youkai = null,
                        Icon = null,
                        YMoney = null,
                        CapsuleColor = placeHolderCapsule,
                        PrizeType = PrizeType.Item,
                        RarityType = RarityType.RarityB,
                        Item = new ItemWonPopup
                        {
                            Count = 1, 
                            ItemId = resultItem, 
                            IsLimitOver = 0
                        },
                        ConvertItemInfo = null
                    };
                }
                //Roll yokai
                else
                {
                    int value = int.Parse(pool);

                    if (Enum.IsDefined(typeof(RarityType), value))
                    {
                        var rank = (RarityType)value;
                        var yokaisToRoll = gacha.Yokais[pool];
                        var roll = Random.Shared.Next(yokaisToRoll.Count);
                        var resultYokai = yokaisToRoll[roll];

                        return RegisterYokaiAndGetPrize(resultYokai, GachaService.CAPSULE_CLRS[rank], rank, userYokaiTable, userSkillTable, dictionaryListTable);
                    }
                    else
                    {
                        throw new InvalidDataException("Invalid rarity for yokai pool in gachaPool");
                    }
                }            
            }
            else
            {
                return null;
            }
        }

        private static string RollPool(Dictionary<string, double> weights, int gachaId)
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

            throw new InvalidOperationException("Invalid weights for pool roll.");
        }

    }
}
