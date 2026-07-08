using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Puniemu.Src.UserDataManager.DataClasses
{
    [Table("mail")]
    public class Mail : BaseModel
    {
        [Column("mail")]
        [PrimaryKey("mail", false)]
        public string? MailAddress { get; set; }

        [Column("currentUdkey")]
        public string CurrentUdkey { get; set; }

    }
}
