using Newtonsoft.Json.Linq;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Requests.UseItem.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;

namespace Puniemu.Src.Server.GameServer.Requests.UseItem.Logic
{
    public class UseItemService
    {
        public TableParser<YwpUserItem> UserItem;

        public TableParser<YwpUserYoukai> UserYoukai;

        public TableParser<YwpUserYoukaiSkill> UserYoukaiSkill;

        public TableParser<YwpUserYoukaiBonusEffect> UserBonusEffect;
        private YwpMstItem _itemInfo;

        private int _itemId;

        private long _youkaiId;
        public UseItemService(int itemId, long youkaiId, TableParser<YwpUserItem> userItem, TableParser<YwpUserYoukai> userYoukai, 
            TableParser<YwpUserYoukaiSkill> userYoukaiSkill, TableParser<YwpMstItem> ywpMstItem, TableParser<YwpUserYoukaiBonusEffect> userBonusEffect)
        {
            _youkaiId = youkaiId;
            _itemId = itemId;
            UserItem = userItem;
            UserYoukai = userYoukai;
            UserYoukaiSkill = userYoukaiSkill;
            _itemInfo = ywpMstItem.Items.Where(x => x.ItemID == itemId).First();
            UserBonusEffect = userBonusEffect;
        }

        private void SpendItem()
        {
            var itemInfo = UserItem.Items.Where(x => x.ItemId == _itemId).FirstOrDefault();
            if(itemInfo.Count == 0)
            {
                throw new KeyNotFoundException("Not enough of item");
            }
            itemInfo!.Count--;
        }

        public async Task<UserYoukaiResultListRes> UseExporb(string gdkey)
        {
            SpendItem();
            var expToAdd = _itemInfo.ItemParam;

            var mstYokai = new TableParser<YwpMstYoukai>(DataManager.Logic.DataManager.GameDataManager.GetTableStringFromJson("ywp_mst_youkai"));

            var yokaiToGive = UserYoukai.Items.Where(x => x.YoukaiId == _youkaiId).First();
            YwpMstYoukai mstYokaiEntry = mstYokai.Items.Where(x => x.YoukaiId == _youkaiId).First();
            var result = new UserYoukaiResultListRes(yokaiToGive, mstYokaiEntry);
            await MoneyExpManager.GiveYoukaiExp(result, yokaiToGive, _youkaiId, expToAdd, mstYokaiEntry, gdkey);

            return result;
        }

        public UseItemSkillResult UseSoultBooster()
        {
            var skillItem = UserYoukaiSkill.Items.First(x => x.YoukaiId == _youkaiId);
            var currentLevel = skillItem.Level;
            if (currentLevel >= 7)
            {
                throw new MaxLevelException();
            }
            SpendItem();
        
            var soulGain = _itemInfo.ItemParam;

            var skillUpdateRes = YoukaiManager.AddExpToSkill(UserYoukaiSkill, _youkaiId, soulGain);

            skillItem.Points = skillUpdateRes.After.Exp;
            skillItem.Level = skillUpdateRes.After.Level;
            skillItem.PercentageDenominator = skillUpdateRes.After.ExpBar.Denominator;
            skillItem.PercentageNumerator = skillUpdateRes.After.ExpBar.Numerator;
            skillItem.Percentage = skillUpdateRes.After.ExpBar.Percentage;

            UseItemSkillResult res = new();
            res.IsMaxLevel = skillUpdateRes.isMaxLevel;
            res.Before = skillUpdateRes.Before;
            res.After = skillUpdateRes.After;
            res.YoukaiID = _youkaiId;
            return res;
        }

        public void UseBonusEffectBooster()
        {
            var bonusItem = UserBonusEffect.Items.FirstOrDefault(x => x.YoukaiID  == _youkaiId);
            if(bonusItem == null)
            {
                throw new InvalidOperationException("Yo-kai does not have bonus effect.");
            }
            if(bonusItem.BonusEffectLevel == 5)
            {
                throw new InvalidOperationException("Yo-kai is at max bonus effect level.");
            }
            SpendItem();

            bonusItem.BonusEffectLevel += 1;
        }

    }
}
