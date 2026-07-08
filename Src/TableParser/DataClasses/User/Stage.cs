using System.Reflection;
using System.Text;

namespace Puniemu.Src.TableParser.DataClasses
{
    // ywp_user_stage table definition
    public class YwpUserStage
    {
        public long StageId { get; set; } = 0;
        public int StageStatus { get; set; } = 0;
        public int Star1 { get; set; } = 0;
        public int Star2 { get; set; } = 0;
        public int Star3 { get; set; } = 0;
        public long Score { get; set; } = 0;
        public int NumClear { get; set; } = 0; // maybe number of time cleared but not sure
        public int Unk2 { get; set; } = 0;
    }
}