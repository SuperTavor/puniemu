using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;

namespace Puniemu.Src.Server.GameServer.Requests.UseItem.Logic
{
    public class UseItemService
    {
        private TableParser<YwpUserItem> _userItem;

        private TableParser<YwpUserYoukai> _userYoukai;

        private TableParser<YwpUserYoukaiSkill> _userYoukaiSkill;

        private YwpMstItem _itemInfo;

        private int _itemId;

        private long _youkaiId;
        public UseItemService(int itemId, long youkaiId, TableParser<YwpUserItem> userItem, TableParser<YwpUserYoukai> userYoukai, TableParser<YwpUserYoukaiSkill> userYoukaiSkill, TableParser<YwpMstItem> ywpMstItem)
        {
            _youkaiId = youkaiId;
            _itemId = itemId;
            _userItem = userItem;
            _userYoukai = userYoukai;
            _userYoukaiSkill = userYoukaiSkill;
            _itemInfo = ywpMstItem.Items.Where(x => x.ItemID == itemId).First();
        }

        private void SpendItem()
        {
            var itemInfo = _userItem.Items.Where(x => x.ItemId == _itemId).FirstOrDefault();
            if(itemInfo.Count == 0)
            {
                throw new KeyNotFoundException("Not enough of item");
            }
            itemInfo!.Count--;
        }

        public void UseExporb()
        {
            var exp = _itemInfo.ItemParam;




            SpendItem();
        }

        public void UseSoultBooster()
        {
            var userSkillItem = _userYoukaiSkill.Items.Where(x => x.YoukaiId == _youkaiId).First();




            SpendItem();
        }

        public void UseSkillBooster()
        {
            throw new NotImplementedException();
        }

    }
}
