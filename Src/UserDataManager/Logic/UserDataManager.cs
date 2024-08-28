using Google.Cloud.Firestore;
using Puniemu.Src.ConfigManager.Logic;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;
namespace Puniemu.Src.UserDataManager.Logic
{
    public static class UserDataManager
    {
        public class TableNotFoundException : Exception
        {
            public TableNotFoundException() : base() { }
        }
        private static FirestoreDb _db = null!;
        //Check credentials and connect to the Firestore database.
        public static void Initialize()
        {
            if (!CredsExist())
            {
                Console.WriteLine("Please make sure your GOOGLE_APPLICATION_CREDENTIALS environment variable is set to your credentials path.");
                Environment.Exit(1);
            }
            else
            {
                var firestoreProjectId = ConfigManager.Logic.ConfigManager.Cfg!.Value.FirestoreDatabaseProjectID;
                _db = FirestoreDb.Create(firestoreProjectId);
            }
        }
        //Checks if the Google Application credentials exist.
        private static bool CredsExist()
        {
            var credPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            //GetEnvironmentVariable returns null when the environment variable is not found.
            return credPath != null;
        }

        //Gets user data from specific account
        public static async Task SetYwpUserAsync(string gdkey, string tableId, object data)
        {
            var dataRef = _db!.Collection("users")
                                .Document(gdkey)
                                .Collection("data")
                                .Document(tableId);

            await dataRef.SetAsync(data);
        }

        public static async Task DeleteUser(string udkey, string gdkey)
        {
            var gdkeyRef = _db!.Collection("users")
                                .Document(gdkey);
            await gdkeyRef.DeleteAsync();
            await RemoveGdkeyFromUdkey(udkey,gdkey);
        }

        private static async Task RemoveGdkeyFromUdkey(string udkey, string gdkey)
        {
            var deviceRef = _db!.Collection("devices")
                                .Document(udkey);
            var snap = await deviceRef.GetSnapshotAsync();
            if(!snap.Exists)
            {
                //This should absolutely never happen but who knows
                return;
            }
            var gdkeys = snap.ConvertTo<List<string>>();
            var removed = gdkeys.Remove(gdkey);
            if(!removed)
            {
                return;
            }
            else
            {
                await deviceRef.SetAsync(gdkeys);
            }
        }
        //Sets user data for specific account
        public static async Task<T?> GetYwpUserAsync<T>(string gdkey, string tableId)
        {
            var dataRef = _db!.Collection("users")
                           .Document(gdkey)
                           .Collection("data")
                           .Document(tableId);

            var snap = await dataRef.GetSnapshotAsync();

            if (!snap.Exists)
            {
                throw new TableNotFoundException();
            }

            var ywpUserData = snap.ConvertTo<T>();

            return ywpUserData;

        }

        //Gets all corresponding GDKeys from under a specified UDKey.
        public static async Task<List<string>> GetGdkeysFromUdkeyAsync(string udkey)
        {
            List<string> gdkeys = new List<string>();
            DocumentReference userDoc = _db!.Collection("devices")
                                            .Document(udkey);

            DocumentSnapshot snap = await userDoc.GetSnapshotAsync();

            if (!snap.Exists)
            {
                // Return empty list if the UDKey is not found.
                gdkeys = new List<string>();
            }
            else
            {
                //get all registed gdkeys
                gdkeys = snap.ConvertTo<List<string>>();
            }
            return gdkeys;
        }

        public static async Task RegisterGdKeyInUdKeyAsync(string udkey, string gdkey)
        {
            var deviceRef = _db!.Collection("devices")
                                .Document(udkey);

            var deviceSnap = await deviceRef.GetSnapshotAsync();
            List<string> registeredGdKeys = new();
            if (!deviceSnap.Exists)
            {
                registeredGdKeys = new();
            }
            else registeredGdKeys = deviceSnap.ConvertTo<List<string>>(); 
            registeredGdKeys.Add(gdkey);

            await deviceRef.SetAsync(registeredGdKeys);
        }

    }
}
