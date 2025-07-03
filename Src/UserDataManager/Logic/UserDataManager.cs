using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.UserDataManager.DataClasses;
using Supabase;
using System.Collections;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Puniemu.Src.UserDataManager.Logic
{
    public static class UserDataManager
    {
        public class TableNotFoundException : Exception
        {
            public TableNotFoundException() : base() { }
        }

        public static Supabase.Client? SupabaseClient;

        //Check credentials and connect to the Firestore database.
        public static void Initialize()
        {
            try
            {
                SupabaseClient = new Supabase.Client(DataManager.Logic.DataManager.SupabaseURL!, DataManager.Logic.DataManager.SupabaseKey!,
                new SupabaseOptions
                {
                    AutoRefreshToken = true,
                });
            }
            catch
            {
                Console.WriteLine("Couldn't create supabase client.");
                Environment.Exit(1);
            }
        }

        // returns the udkey of the newly created device
        public static async Task<string> NewDeviceAsync()
        {
            var device = new Device() 
            { 
                UdKey = Guid.NewGuid().ToString(),
                Gdkeys = new()
            };
            var response = await SupabaseClient!.From<Device>().Insert(device);
            var newDevice = response.Models.First();
            return newDevice.UdKey;
        }
        // returns the udkey of the newly created device
        public static async Task<string> NewAccountAsync()
        {
            var acc = new Account()
            {
                Gdkey = Guid.NewGuid().ToString(),
                YwpUserTables = new(),
                LastLoginTime = ""
            };
            //response is saved to get the generated id
            var response = await SupabaseClient!.From<Account>().Insert(acc);
            var newAcc = response.Models.First();
            return newAcc.Gdkey;
        }
        //Gets user data from specific account
        public static async Task SetYwpUserAsync(string gdkey, string tableId, object data)
        {
            var response = await SupabaseClient!.From<Account>().Where(a => a.Gdkey == gdkey).Get();
            var account = response.Models.FirstOrDefault();
            account.YwpUserTables[tableId] = data;
            await account.Update<Account>();
        }

        public static async Task DeleteUser(string udkey, string gdkey)
        {
            await RemoveGdkeyFromUdkey(udkey, gdkey);
            await SupabaseClient.From<Account>().Where(a => a.Gdkey == gdkey).Delete();
        }

        private static async Task RemoveGdkeyFromUdkey(string udkey, string gdkey)
        {
            var response = await SupabaseClient.From<Device>().Where(d => d.UdKey == udkey).Get();
            var device = response.Models.FirstOrDefault();
            device.Gdkeys.Remove(gdkey);
        }
        //Sets user data for specific account
        public static async Task<T?> GetYwpUserAsync<T>(string gdkey, string tableId)
        {
            var response = await SupabaseClient!.From<Account>().Where(a => a.Gdkey == gdkey).Get();
            var account = response.Models.FirstOrDefault();
            var tbl = account.YwpUserTables[tableId];
            JToken token = JToken.FromObject(tbl);
            return token.ToObject<T>();
        }
        public static async Task<Dictionary<string,object>> GetEntireUserData(string gdkey)
        {
            var response = await SupabaseClient!.From<Account>().Where(a => a.Gdkey == gdkey).Get();
            var account = response.Models.FirstOrDefault();
            return account.YwpUserTables;
        }
        public static async Task SetEntireUserData(string gdkey, Dictionary<string,object> data)
        {
            var response = await SupabaseClient!.From<Account>().Where(a => a.Gdkey == gdkey).Get();
            var account = response.Models.FirstOrDefault();
            account.YwpUserTables = data;
            await account.Update<Account>();
        }
        //Gets all corresponding GDKeys from under a specified UDKey.
        public static async Task<List<string>> GetGdkeysFromUdkeyAsync(string udkey)
        {
            var response = await SupabaseClient.From<Device>().Where(d => d.UdKey == udkey).Get();
            var device = response.Models.FirstOrDefault();
            return device.Gdkeys;
        }
        //Add a gdkey association to a udkey
        public static async Task AddAccountToDevice(string udkey, string gdkey)
        {
            // get the correct device
            var response = await SupabaseClient.From<Device>().Where(d => d.UdKey == udkey).Get();
            var device = response.Models.FirstOrDefault();
            if(device.Gdkeys == null) device.Gdkeys = new List<string>();
            device.Gdkeys.Add(gdkey); 
            await device.Update<Device>();
        }
    }
}
