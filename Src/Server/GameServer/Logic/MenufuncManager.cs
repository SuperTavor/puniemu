using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.InitGacha.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using System.Buffers;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Logic
{
    public class MenufuncManager
    {
        public static TableParser.Logic.TableParser<YwpUserMenufunc> AddApp(TableParser.Logic.TableParser<YwpUserMenufunc> parser, long appId, int flg)
        {
            var index = SearchApp(parser, appId);

            if (index == -1)
            {
                var menufunc = new YwpUserMenufunc();
                menufunc.AppId = appId;
                menufunc.AppFlg = flg;
                parser.Items.Add(menufunc);
                return parser;
            }
            if (flg == 1)
            {
                parser.Items[index].AppFlg = flg;
            }
            return parser;
        }
        public static int SearchApp(TableParser.Logic.TableParser<YwpUserMenufunc> parser, long appId)
        {
            var index = 0;
            foreach (YwpUserMenufunc item in parser.Items)
            {
                if (item.AppId == appId)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }
      
    }
}
