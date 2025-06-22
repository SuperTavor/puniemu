using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.InitGacha.DataClasses;
using System.Buffers;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Logic
{
    public class PlayerIconManager
    {
        public static TableParser.Logic.TableParser AddIcon(TableParser.Logic.TableParser parser, int iconId)
        {
            int idx = 0;
            bool found = false;
            foreach (string[] elements in parser.Table)
            {
                if (int.Parse(elements[0]) == iconId)
                {
                    found = true;
                    break;
                }
                idx += 1;
            }
            if (found == false)
            {
                parser.AddRow([iconId.ToString()]);
            }
            return new TableParser.Logic.TableParser(parser.ToString());
        }
    }
}
