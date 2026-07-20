using Newtonsoft.Json;

namespace Puniemu.Src.UserDataManager.DataClasses
{
    // Maps to the "account" table
    public class Account
    {
        // column: gdkey (primary key)
        public string? Gdkey { get; set; }

        // column: character_id
        public string? CharacterId { get; set; }

        // column: udkey
        public string? Udkey { get; set; }

        // column: user_id
        public string? UserId { get; set; }

        // column: ywp_user_tables (jsonb)
        public Dictionary<string, object?>? YwpUserTables { get; set; }

        // column: last_lgn_time
        public string? LastLoginTime { get; set; }

        // column: start_date (stored as text)
        public long StartDate { get; set; }

        // column: opening_tutorial_flag
        public bool OpeningTutorialFlag { get; set; }

        [JsonIgnore]
        //used for the account cache
        public bool IsDirty { get; set; } = false;
    }
}
