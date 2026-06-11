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

      

        public static GachaPrize RegisterYokaiAndGetPrize(long yokaiId, CapsuleColor capsule, RarityType rank, TableParser<YwpUserYoukai> userYokaiTable, TableParser<YwpUserYoukaiSkill> userSkillTable, TableParser.Logic.TableParser dictionaryListTable, TableParser<YwpUserItem> userItem, int gachaId, TableParser<YwpUserYoukaiBonusEffect> userBonus)
        {
            var prizeType = PrizeType.Yokai;
            var getType = YokaiWonPopup.CheckGetType(yokaiId, userYokaiTable, userSkillTable);
            YokaiWonPopup? yokai = null;
            ConvertItemInfo? convertItemInfo = null;
            ItemWonPopup? itemForConvert = null;
            if(getType != YokaiGetType.MaxLevel)
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
                ItemManager.AddItem(userItem, itemForConvert.ItemId, item_count:1);
            }
              
            //Register yokai if not registered
            YoukaiManager.AddYoukai(userYokaiTable, yokaiId, userSkillTable, userBonus);

            dictionaryListTable = DictionaryManager.EditDictionary(dictionaryListTable, yokaiId, true, true);

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
        //returns null if gacha id doesnt exist, else return yokai id and rarity (Rank)
        public static GachaPrize? CrankReward(int gachaId, TableParser<YwpUserYoukai> userYokaiTable, TableParser<YwpUserYoukaiSkill> userSkillTable, TableParser.Logic.TableParser dictionaryListTable, TableParser<YwpUserItem> userItemTable, TableParser<YwpUserYoukaiBonusEffect> userBonus)
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

                        return RegisterYokaiAndGetPrize(resultYokai, GachaService.CAPSULE_CLRS[rank], rank, userYokaiTable, userSkillTable, dictionaryListTable, userItemTable, gachaId, userBonus);
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
