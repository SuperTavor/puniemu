namespace Puniemu.Src.UserDataManager.DataClasses
{
    // Maps to the "device" table
    public class Device
    {
        // column: udkey (primary key)
        public string? UdKey { get; set; }

        // column: gdkeys (text[])
        public List<string>? Gdkeys { get; set; }

    }
}
