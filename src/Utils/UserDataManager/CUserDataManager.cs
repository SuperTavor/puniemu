using Google.Cloud.Firestore;
using Puniemu.Src.ConfigManager;
namespace Puniemu.src.Utils.UserDataManager
{
    public static class CUserDataManager
    {
        private static FirestoreDb? _db;
        public static void Initialize()
        {
            if(!CloudfireCredentialsExist())
            {
               Console.WriteLine("Please make sure your GOOGLE_APPLICATION_CREDENTIALS environment variable is set to your credentials path.");
               Environment.Exit(1);
            }
            else
            {
                var firestoreProjectId = CConfigManager.Shared!.FirestoreDatabaseProjectID;
                _db = FirestoreDb.Create(firestoreProjectId);
            }
        }

        private static bool CloudfireCredentialsExist()
        {
            var credPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            //GetEnvironmentVariable returns null when the environment variable is not found.
            return credPath == null;
        }

        public static async Task<List<string>> GetGdkeysFromUdkeyAsync(string udkey)
        {
            List<string> existingGdkeys = new List<string>();
            DocumentReference udkeyDoc = _db.Collection("UDKeys").Document(udkey);
            DocumentSnapshot snap = await udkeyDoc.GetSnapshotAsync();

            if (!snap.Exists)
            {
                // Return empty list if the UDKey is not found.
                return new List<string>();
            }
            else
            {
                var gdkeys = snap.GetValue<List<string>>("gdkeys");
                // Return gdkeys, or if it's null just return an empty list
                return gdkeys ?? new List<string>();
            }
        }
    }
}
