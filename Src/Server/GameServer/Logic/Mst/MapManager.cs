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
    public class MstMapManager
    {
        public static int GetMapIndex(List<YwpMstMap> parser, long mapId)
        {
            uint count = 0;
            foreach (YwpMstMap i in parser)
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
