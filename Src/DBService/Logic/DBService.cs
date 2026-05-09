using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.UserDataManager.DataClasses;
using Supabase;
using Supabase.Postgrest.Attributes;
using System.Reflection;

namespace Puniemu.Src.UserDataManager.Logic
{
    public static class DBService
    {
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
        public static async Task<string> NewDeviceAsync(string? udkey = null)
        {
            Device dev = new Device();
            dev.Gdkeys = new();
            if (udkey == null)
            {
                dev.UdKey = Guid.NewGuid().ToString();
            }
            else dev.UdKey = udkey;
            var response = await SupabaseClient!.From<Device>().Insert(dev);
            var newDevice = response.Models.First();
            return newDevice.UdKey!;
        }

        //checks if udkey exists
        public static async Task<bool> IsDeviceExists(string udkey)
        {
            var response = await SupabaseClient!
                .From<Device>()
                .Where(d => d.UdKey == udkey)
                .Count(Supabase.Postgrest.Constants.CountType.Exact);

            return response > 0;
        }
        // returns the gdkey of the newly created account
        public static async Task<string> NewAccountAsync()
        {
            var acc = new Account()
            {
                Gdkey = Guid.NewGuid().ToString(),
                LastLoginTime = "",
                CharacterId = ""
            };
            //response is saved to get the generated id
            var response = await SupabaseClient!.From<Account>().Insert(acc);
            var newAcc = response.Models.First();
            return newAcc.Gdkey!;
        }
        //Gets user data from specific account
        public static async Task SetYwpUserAsync(string gdkey, string tableId, object data)
        {
            var response = await SupabaseClient!
                .From<Account>()
                .Where(a => a.Gdkey == gdkey)
                .Get();

            var account = response.Models.FirstOrDefault();
            if (account == null)
                return;

            string json = JsonConvert.SerializeObject(data);

            var prop = typeof(Account)
                .GetProperties()
                .FirstOrDefault(p =>
                {
                    var attr = p.GetCustomAttribute<ColumnAttribute>();
                    return attr != null && attr.ColumnName == tableId;
                });

            if (prop == null)
                throw new ArgumentException($"Unknown ywp_user column: {tableId}");

            prop.SetValue(account, json);

            await account.Update<Account>();
        }

        public static async Task DeleteUser(string udkey, string gdkey)
        {
            await RemoveGdkeyFromUdkey(udkey, gdkey);
            await SupabaseClient!.From<Account>().Where(a => a.Gdkey == gdkey).Delete();
        }

        private static async Task RemoveGdkeyFromUdkey(string udkey, string gdkey)
        {
            var response = await SupabaseClient!.From<Device>().Where(d => d.UdKey == udkey).Get();
            var device = response.Models.FirstOrDefault()!;
            device.Gdkeys!.Remove(gdkey);
            await device.Update<Device>();
        }
        //GETS user data for specific account
        public static async Task<T?> GetYwpUserAsync<T>(string gdkey, string tableId)
        {
            var response = await SupabaseClient!
                .From<Account>()
                .Where(a => a.Gdkey == gdkey)
                .Get();

            var account = response.Models.FirstOrDefault();
            if (account == null)
                return default;

            var prop = typeof(Account)
                .GetProperties()
                .FirstOrDefault(p =>
                {
                    var attr = p.GetCustomAttribute<ColumnAttribute>();
                    return attr != null && attr.ColumnName == tableId;
                });

            if (prop == null)
                return default;

            var raw = prop.GetValue(account) as string;

            if (string.IsNullOrEmpty(raw))
                return default;

            return JsonConvert.DeserializeObject<T>(raw);
        }

        public static async Task<Account> GetAccountObjectAsync(string gdkey)
        {
            var response = await SupabaseClient!.From<Account>().Where(a => a.Gdkey == gdkey).Get();

            var account = response.Models.FirstOrDefault();

            return account;
        }

        public static async Task SetAccountObjectAsync(string gdkey, Account acc)
        {
            await SupabaseClient!
                .From<Account>()
                .Where(a => a.Gdkey == gdkey)
                .Update(acc);
        }
        public static async Task<string> GetGdkeyFromCharacterId(string charId)
        {
            var response = await SupabaseClient!.From<Account>().Where(a => a.CharacterId == charId).Get();

            var account = response.Models.FirstOrDefault();
            return account?.Gdkey ?? string.Empty;
        }
        public static async Task<string> GetGdkeyFromUserId(string userId)
        {
            var response = await SupabaseClient!.From<Account>().Where(a => a.UserId == userId).Get();

            var account = response.Models.FirstOrDefault();
            return account?.Gdkey ?? string.Empty;
        }
        public static async Task<string> GetLastLoginTime(string gdkey)
        {
            var response = await SupabaseClient!.From<Account>().Where(a => a.Gdkey == gdkey).Get();

            var account = response.Models.FirstOrDefault();
            return account?.LastLoginTime!;
        }

        //Gets all corresponding GDKeys from under a specified UDKey.
        public static async Task<List<string>> GetGdkeysFromUdkeyAsync(string udkey)
        {
            var response = await SupabaseClient!.From<Device>().Where(d => d.UdKey == udkey).Get();
            var device = response.Models.FirstOrDefault();
            return device?.Gdkeys!;
        }
        //Add a gdkey association to a udkey
        public static async Task AddAccountToDevice(string udkey, string gdkey)
        {
            // get the correct device
            var response = await SupabaseClient!.From<Device>().Where(d => d.UdKey == udkey).Get();
            var device = response.Models.FirstOrDefault()!;
            if(device.Gdkeys == null) device.Gdkeys = new List<string>();
            device.Gdkeys.Add(gdkey); 
            await device.Update<Device>();
        }
    }
}
