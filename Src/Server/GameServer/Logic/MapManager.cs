using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.InitGacha.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using System.Buffers;
using System.IO;
using System.Numerics;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Logic
{
    public class MapManager
    {
        public static void AddMap(TableParser<YwpUserMap> parser, long mapId)
        {
            var MapIndex = GetMapIndex(parser, mapId);
            if (MapIndex == -1)
            {
                parser.AddItem(new YwpUserMap {MapId = mapId, IsUnlocked = 0, FriendCount = 0 });
            }
        }
        public static void UpdateMap(TableParser<YwpUserMap> parser, long mapId, int isUnlockd)
        {
            var MapIndex = GetMapIndex(parser, mapId);
            if (MapIndex != -1)
            {
                if (isUnlockd == 1 && parser.Items[MapIndex].IsUnlocked == 0)
                {
                    parser.Items[MapIndex].IsUnlocked = 1;
                }
            }
        }
        public static int GetMapIndex(TableParser<YwpUserMap> parser, long mapId)
        {
            uint count = 0;
            foreach (YwpUserMap i in parser.Items)
            {
                if (i.MapId == mapId)
                {
                    return (int)count;
                }
                count += 1;
            }
            return -1;
        }
    }
}
