using Puniemu.Src.Server.GameServer.DataClasses;
using System.Reflection;
using System.Text;

namespace Puniemu.Src.TableParser.DataClasses
{
    public class YwpMstYoukaiLevelOpen
    {
        public int Level { get; set; } //0
        public int RarityType { get; set; } //1
        public int YmoneyCost { get; set; } // 2
        public int YpointCost { get; set; } //3
        public int DiscountId { get; set; } //4
    }
}
