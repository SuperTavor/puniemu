using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.InitGacha.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using System.Buffers;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Logic
{
    public class ItemManager
    {
        public static TableParser.Logic.TableParser<YwpUserItem> AddItem(TableParser.Logic.TableParser<YwpUserItem> parser, long itemId, int item_count)
        {
            try
            {
                var item = parser.Items.Where(x => x.ItemId == itemId).First();
                item.Count += item_count;
            }
            catch
            {
                parser.Items.Add(new YwpUserItem
                {
                    ItemId = itemId,
                    Count = item_count
                });
            }
            return parser;
        }
        public static TableParser.Logic.TableParser<YwpUserItem> RemoveItem(TableParser.Logic.TableParser<YwpUserItem> parser, long itemId, int item_count)
        {
            var itemToRemove = parser.Items.Where(x => x.ItemId == itemId).First();
            if (itemToRemove.Count > item_count) itemToRemove.Count -= item_count;
            else parser.Items.Remove(itemToRemove);
            return parser;
        }
      
    }
}
