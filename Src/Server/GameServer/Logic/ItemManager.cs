using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.InitGacha.DataClasses;
using System.Buffers;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Logic
{
    public class ItemManager
    {
        public static TableParser.Logic.TableParser AddItem(TableParser.Logic.TableParser parser, long itemId, int item_count)
        {
            int idx = 0;
            bool edited = false;
            foreach (string[] elements in parser.Table)
            {
                if (int.Parse(elements[0]) == itemId)
                {
                    edited = true;
                    parser.Table[idx][1] = (int.Parse(parser.Table[idx][1]) + item_count).ToString();
                    break;
                }
                idx += 1;
            }
            if (edited == false)
            {
                parser.AddRow([itemId.ToString(), item_count.ToString()]);
            }
            return new TableParser.Logic.TableParser(parser.ToString());
        }
        public static TableParser.Logic.TableParser RemoveItem(TableParser.Logic.TableParser parser, long itemId, int item_count)
        {
            int idx = 0;
            foreach (string[] elements in parser.Table)
            {
                if (int.Parse(elements[0]) == itemId)
                {
                    var removed_item = int.Parse(parser.Table[idx][1]) - item_count;
                    if (removed_item < 0)
                    {
                        removed_item = 0;
                    }
                    parser.Table[idx][1] = removed_item.ToString();
                    break;
                }
                idx += 1;
            }
            return new TableParser.Logic.TableParser(parser.ToString());
        }
        public static int GetItemCount(TableParser.Logic.TableParser parser, long itemId)
        {
            int idx = 0;
            int item_count = 0;
            foreach (string[] elements in parser.Table)
            {
                if (int.Parse(elements[0]) == itemId)
                {
                    item_count = int.Parse(elements[1]);
                    break;
                }
                idx += 1;
            }
            return item_count;
        }
    }
}
