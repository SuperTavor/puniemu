using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using System.Reflection;
using System.Text;

namespace Puniemu.Src.TableParser.DataClasses
{
    // ywp_user_deck table definition

    public class YwpUserYoukaiDeck
    {
        public long Unk1 { get; set; } //maybe deck id
        public long MiddleYoukaiId { get; set; }
        public long MiddleLeftYoukaiId { get; set; }
        public long MiddleRightYoukaiId { get; set; }
        public long FarLeftYoukaiId { get; set; }
        public long FarRightYoukaiId { get; set; }
        public int Unk2 { get; set; } // maybe is current deck
        public long WatchId { get; set; }
    }
}