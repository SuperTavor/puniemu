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

        private YwpMstItem _itemInfo;

        private int _itemId;

        private long _youkaiId;
        public UseItemService(int itemId, long youkaiId, TableParser<YwpUserItem> userItem, TableParser<YwpUserYoukai> userYoukai, TableParser<YwpUserYoukaiSkill> userYoukaiSkill, TableParser<YwpMstItem> ywpMstItem)
        {
            _youkaiId = youkaiId;
            _itemId = itemId;
            UserItem = userItem;
            UserYoukai = userYoukai;
            UserYoukaiSkill = userYoukaiSkill;
            _itemInfo = ywpMstItem.Items.Where(x => x.ItemID == itemId).First();
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

        public UserYoukaiResultListRes UseExporb()
        {
            var expToAdd = _itemInfo.ItemParam;

            var mstYokai = new TableParser<YwpMstYoukai>(DataManager.Logic.DataManager.GameDataManager.GetTableStringFromJson("ywp_mst_youkai"));
            var mstYokaiLevel = new TableParser<YwpMstYoukaiLevel>(DataManager.Logic.DataManager.GameDataManager.GetTableStringFromJson("ywp_mst_youkai_level"));
            var mstYokaiLevelOpen = new TableParser<YwpMstYoukaiLevelOpen>(DataManager.Logic.DataManager.GameDataManager.GetTableStringFromJson("ywp_mst_youkai_level_open"));

            var yokaiToGive = UserYoukai.Items.Where(x => x.YoukaiId == _youkaiId).First();
            var result = MoneyExpManager.GiveYoukaiExp(yokaiToGive, _youkaiId, expToAdd, mstYokai, mstYokaiLevel, mstYokaiLevelOpen);

            SpendItem();

            return result;
        }

        public UseItemSkillResult UseSoultBooster()
        {
            var skillItem = UserYoukaiSkill.Items.First(x => x.YoukaiId == _youkaiId);
            var currentLevel = skillItem.Level;
            if(currentLevel >= 7)
            {
                throw new MaxLevelException();
            }
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

            SpendItem();

            return res;
        }

        public void UseSkillBooster()
        {
            throw new NotImplementedException();
        }

    }
}
