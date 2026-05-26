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
    public class MstYoukaiManager
    {
        public static int GetYoukaiIndex(TableParser<YwpMstYoukai> parser, long YoukaiId)
        {
            uint count = 0;
            foreach (YwpMstYoukai i in parser.Items)
            {
                if (i.YoukaiId == YoukaiId)
                {
                    return (int)count;
                }
                count += 1;
            }
            return -1;
        }
        public static int GetYoukaiLevelIndex(TableParser<YwpMstYoukaiLevel> parser, int LevelTtype, int Level)
        {
            uint count = 0;
            foreach (YwpMstYoukaiLevel i in parser.Items)
            {
                if (i.LevelTtype == LevelTtype && i.Level == Level)
                {
                    return (int)count;
                }
                count += 1;
            }
            return -1;
        }

        public static int GetYoukaiLevelOpenIndex(TableParser<YwpMstYoukaiLevelOpen> parser, int beforeLevel, int afterLevel, RarityType rarityType)
        {
            int res = -1;
            uint count = 0;
            uint currlevel = 0;
            foreach (YwpMstYoukaiLevelOpen i in parser.Items)
            {
                if (i.RarityType == rarityType && beforeLevel < i.Level && afterLevel >= i.Level && (i.Level < currlevel || currlevel == 0))
                {
                    currlevel = (uint)i.Level;
                    res = (int)count;
                }
                count += 1;
            }
            return res;
        }
    }
}
