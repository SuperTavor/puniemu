using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using Puniemu.Src.ConfigManager.Logic;
namespace Puniemu.Src.UserDataManager.Logic
{
    public static class UserDataManager
    {
        public class TableNotFoundException : Exception
        {
            public TableNotFoundException() : base() { }
        }

        private static IFirebaseClient _client;
        //Check credentials and connect to the Firestore database.
        public static void Initialize()
        {
            try
            {
                _client = new FirebaseClient(new FirebaseConfig()
                {
                    BasePath = ConfigManager.Logic.ConfigManager.Cfg!.Value.FirebaseBasePath,
                    AuthSecret = ConfigManager.Logic.ConfigManager.Cfg!.Value.FirebaseAuthSecret
                });
            }
            catch
            {
                Console.WriteLine("Couldn't create firebase client.");
                Environment.Exit(1);
            }
        }

        //Gets user data from specific account
        public static async Task SetYwpUserAsync<T>(string gdkey, string tableId, T data)
        {
            var tableKey = $"UserData/{gdkey}/Tables/{tableId}";
            //Set table data
            await _client.SetAsync(tableKey, data);
        }

        public static async Task DeleteUser(string udkey, string gdkey)
        {
            await _client.DeleteAsync($"UserData/{gdkey}");
            await RemoveGdkeyFromUdkey(udkey, gdkey);
        }

        private static async Task RemoveGdkeyFromUdkey(string udkey, string gdkey)
        {
            var accountsPath = $"Devices/{udkey}/Accounts";
            var deviceGdkeys = await _client.GetAsync(accountsPath);
            if (deviceGdkeys.Body == "null")
            {
                return;
            }
            else
            {
                var list = deviceGdkeys.ResultAs<List<string>>();
                list.Remove(gdkey);
                await _client.SetAsync(accountsPath, list);
            }
        }
        //Sets user data for specific account
        public static async Task<T?> GetYwpUserAsync<T>(string gdkey, string tableId)
        {
            var res = await _client.GetAsync($"UserData/{gdkey}/Tables/{tableId}");
            var converted = res.ResultAs<T>();
            return converted;
        }

        public static async Task<Dictionary<string,object>> GetEntireUserData(string gdkey)
        {
            var tablesRef = await _client.GetAsync($"UserData/{gdkey}/Tables");
            var tables = tablesRef.ResultAs<Dictionary<string, object>>();
            return tables;
        }

        public static async Task SetEntireUserData(string gdkey, Dictionary<string,object> data)
        {
            await _client.SetAsync($"UserData/{gdkey}/Tables", data);
        }
        //Gets all corresponding GDKeys from under a specified UDKey.
        public static async Task<List<string>> GetGdkeysFromUdkeyAsync(string udkey)
        {
            var accountsPath = $"Devices/{udkey}/Accounts";
            var deviceGdkeys = await _client.GetAsync(accountsPath);
            if(deviceGdkeys.Body == "null")
            {
                return new List<string>();
            }
            else
            {
                return deviceGdkeys.ResultAs<List<string>>();
            }
        }
        //Add a gdkey association to a udkey
        public static async Task RegisterGdKeyInUdKeyAsync(string udkey, string gdkey)
        {
            var accountsPath = $"Devices/{udkey}/Accounts";
            var deviceGdkeys = await _client.GetAsync(accountsPath);
            List<string> gdkeys = new();
            if (deviceGdkeys.Body != "null")
            {
                gdkeys = deviceGdkeys.ResultAs<List<string>>();
            }
            gdkeys.Add(gdkey);

            await _client.SetAsync(accountsPath, gdkeys);
        }

    }
}
