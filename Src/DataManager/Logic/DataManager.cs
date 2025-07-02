namespace Puniemu.Src.DataManager.Logic
{
    public static class DataManager
    {
        public static GameDataManager GameDataManager = new GameDataManager();

        public static string? SupabaseKey { get; private set; }

        public static string? SupabaseURL { get; private set; }

        // If false, the data download will be downloaded from a different server rather than the Supabase storage. Supabase storage method isnt implemented yet
        public static bool IsDataDownloadFromSupabase { get; private set; }

        //0 if the data is retrieved from Supabase
        public static string? DataDownloadURL { get; private set; }

        public static string? GameVersion { get; private set; }

        //Can be displayed in text boxes
        public static string? ServerName { get; private set; }
        public static void StaticInit(IConfiguration config)
        {
            SupabaseKey = config["SupabaseKey"];
            SupabaseURL = config["SupabaseURL"];
            GameVersion = config["GameVersion"];
            ServerName = config["ServerName"];
            //now check if the data download link is 0. if it is, it means we will draw the data download files from supabase storage
            if (int.TryParse(config["DataDownloadSource"], out int result) && result == 0)
            {
                IsDataDownloadFromSupabase = true;
            }
            else DataDownloadURL = config["DataDownloadSource"];
        }
    }
}
