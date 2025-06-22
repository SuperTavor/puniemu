using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.InitGacha.DataClasses;
using System.Buffers;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Logic
{
    public class TutorialFlagManager
    {
        public static TableParser.Logic.TableParser EditTutorialFlg(TableParser.Logic.TableParser parser, int target_tutorial_type, int target_tutorial_id, int target_tutorital_value)
        {
            int idx = 0;
            bool edited = false;
            foreach (string[] elements in parser.Table)
            {
                if (int.Parse(elements[1]) == target_tutorial_id)
                {
                    edited = true;
                    parser.Table[idx][0] = target_tutorial_type.ToString();
                    parser.Table[idx][2] = target_tutorital_value.ToString();
                    break;
                }
                idx += 1;
            }
            if (edited == false)
            {
                parser.AddRow([target_tutorial_type.ToString(), target_tutorial_id.ToString(), target_tutorital_value.ToString()]);
            }
            return new TableParser.Logic.TableParser(parser.ToString());
        }
        public static int GetStatus(TableParser.Logic.TableParser parser, int target_tutorial_id)
        {
            int idx = 0;
            int status = -1;
            foreach (string[] elements in parser.Table)
            {
                if (int.Parse(elements[1]) == target_tutorial_id)
                {
                    status = int.Parse(elements[2]);
                    break;
                }
                idx += 1;
            }
            return status;
        }
    }
}
