using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using Newtonsoft.Json;
using Puniemu.Src.ConfigManager.Logic;
using System.Collections;
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
            await _client.SetAsync(tableKey, JsonConvert.SerializeObject(data));
        }
        public static async Task AssignGdkeyToCharacterID(string characterId, string gdkey)
        {
            await _client.SetAsync($"CharacterIdGdkeyAssignment/{characterId}", gdkey);
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
            var converted = JsonConvert.DeserializeObject<T>(res.ResultAs<string>());
            return converted;
        }

        public static async Task<Dictionary<string,object>> GetEntireUserData(string gdkey)
        {
            var tablesRef = await _client.GetAsync($"UserData/{gdkey}/Tables");
            var tables = tablesRef.ResultAs<Dictionary<string, string>>();
            var convertedTables = new Dictionary<string, object>();
            foreach(var table in tables)
            {
                if(table.Value != null)
                {
                    try
                    {
                        convertedTables[table.Key] = JsonConvert.DeserializeObject<object>(table.Value);
                    }
                    catch
                    {
                        convertedTables[table.Key] = table.Value;
                    }
                }
            }
            return convertedTables;
        }
        public static async Task SetEntireUserData(string gdkey, Dictionary<string,object> data)
        {
            var jsonifiedData = new Dictionary<string, string>();
            foreach(var item in data)
            {
                if(item.Value != null)
                {
                    jsonifiedData[item.Key] = JsonConvert.SerializeObject(item.Value);
                }
            }
            await _client.SetAsync($"UserData/{gdkey}/Tables", jsonifiedData);
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
