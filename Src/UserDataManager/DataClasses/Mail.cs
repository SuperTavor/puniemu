namespace Puniemu.Src.UserDataManager.DataClasses
{
    // Maps to the "mail" table
    public class Mail
    {
        // column: mail (primary key)
        public string? MailAddress { get; set; }

        // column: "currentUdkey"
        public string? CurrentUdkey { get; set; }

    }
}
