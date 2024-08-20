namespace Puniemu.Src.ConfigManager.DataClasses
{
    public struct SConfigStructure
    {
        public string FirestoreDatabaseProjectID { get; set; }
        public string BaseDataDownloadURL { get; set; }
        public string GameDataPath { get; set; }
        public string ClientVersion { get; set; }
        public string ServerName { get; set; }
        public bool IsMaintenance { get; set; }
        public string MaintenanceEndTime { get; set; }
    }
}
