using System.Reflection;
using System.Text;

namespace Puniemu.Src.TableParser.DataClasses
{
    // ywp_user_stage table definition
    public class YwpUserStage
    {
        public long StageId { get; set; }
        public int StageStatus { get; set; }
        public int Star1 { get; set; }
        public int Star2 { get; set; }
        public int Star3 { get; set; }
        public long Score { get; set; }
        public int NumClear { get; set; } // maybe number of time cleared but not sure
        public int Unk2 { get; set; }
    }
}