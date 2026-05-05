using Puniemu.Src.Server.GameServer.DataClasses;
using System.Reflection;
using System.Text;

namespace Puniemu.Src.TableParser.DataClasses
{
    public class YwpMstYoukaiLevel
    {
        public int LevelTtype { get; set; } //0
        public int Level { get; set; } //1
        public int BaseExp { get; set; } // 2
        public int MaxExp { get; set; } //3
    }
}
