using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Puniemu.Src.UserDataManager.DataClasses
{
    [Table("account")]
    public class Account : BaseModel
    {
        [Column("gdkey")]
        [PrimaryKey("gdkey",false)]
        public string Gdkey { get; set; }

        [Column("ywp_user_tables")]
        public Dictionary<string,object> YwpUserTables { get; set; }

        [Column("last_lgn_time")]
        public string LastLoginTime { get; set; }

        [Column("start_date")]
        public long StartDate { get; set; }

        [Column("opening_tutorial_flag")]
        public bool OpeningTutorialFlag { get; set; }
    }
}
