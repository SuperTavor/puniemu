using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Puniemu.Src.UserDataManager.DataClasses
{
    [Table("device")]
    public class Device : BaseModel
    {
        [Column("udkey")]
        [PrimaryKey("udkey",false)]
        public string UdKey { get; set; }

        [Column("gdkeys")]
        public List<string> Gdkeys { get; set; }

    }
}
