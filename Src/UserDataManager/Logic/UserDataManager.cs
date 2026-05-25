using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.UserDataManager.DataClasses;
using Supabase;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Security.Cryptography;
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

        private static ConcurrentDictionary<string, Account> _accountCache = new();

        // Timer variables for background flushing
        private static PeriodicTimer? _flushTimer;
        private static Task? _flushTask;
        private static CancellationTokenSource _cts = new();

        // connect to supbase
        public static void Initialize()
        {
            try
            {
                SupabaseClient = new Supabase.Client(DataManager.Logic.DataManager.SupabaseURL!, DataManager.Logic.DataManager.SupabaseKey!,
                new SupabaseOptions
                {
                    AutoRefreshToken = true,
                });

                // Start the 5-minute background flush loop
                _flushTimer = new PeriodicTimer(TimeSpan.FromMinutes(5));
                _flushTask = FlushLoopAsync(_cts.Token);
                Console.WriteLine("db service started");
            }
            catch
            {
                Console.WriteLine("Couldn't create supabase client.");
                Environment.Exit(1);
            }
        }

        private static async Task FlushLoopAsync(CancellationToken token)
        {
            try
            {
                while (await _flushTimer!.WaitForNextTickAsync(token))
                {
                    await FlushAllDirtyAccountsAsync();
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("bg flush stopped gracefully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in background flush loop: {ex.Message}");
            }
        }

        private static async Task FlushAllDirtyAccountsAsync()
        {
            foreach (var kvp in _accountCache)
            {
                await FlushAccount(kvp.Key);
                Console.WriteLine($"Saved account. gdkey:" + kvp.Key);
            }
        }

        private static async Task FlushAccount(string gdkey)
        {
            var account = _accountCache[gdkey];

            if (account.IsDirty)
            {
                account.IsDirty = false;

                try
                {
                    await account.Update<Account>();
                }
                catch (Exception ex)
                {
                    account.IsDirty = true;
                    Console.WriteLine($"Failed to flush account {gdkey}: {ex.Message}");
                }
            }
            else
            {
                _accountCache.TryRemove(gdkey, out _);
            }
        }
        public static async Task ShutdownAsync()
        {
            _cts.Cancel(); 
            await FlushAllDirtyAccountsAsync();
        }

        private static async Task<Account> GetAccountFromGdkeyAsync(string gdkey)
        {
            if (_accountCache.TryGetValue(gdkey, out Account? acc))
            {
                return acc;
            }
            else
            {
                var response = await SupabaseClient!.From<Account>().Where(a => a.Gdkey == gdkey).Get();
                var account = response.Models.FirstOrDefault();
                if (account != null)
                {
                    _accountCache[gdkey] = account;
                }
                return account!;
            }
        }

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

        // checks if udkey exists
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
                YwpUserTables = new(),
                LastLoginTime = "",
                CharacterId = GenerateFriendCode()
            };
            var response = await SupabaseClient!.From<Account>().Insert(acc);
            var newAcc = response.Models.First();

            // Immediately cache the new account
            _accountCache[newAcc.Gdkey!] = newAcc;
            return newAcc.Gdkey!;
        }

        private static string GenerateFriendCode()
        {
            char[] LetterBytes = "abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
            const int CodeLength = 8;
            var code = new char[CodeLength];

            byte[] buffer = new byte[CodeLength];

            RandomNumberGenerator.Fill(buffer);

            for (int i = 0; i < code.Length; i++)
            {
                code[i] = LetterBytes[buffer[i] % LetterBytes.Length];
            }

            return new string(code);
        }

        // Sets user data for specific account
        public static async Task SetYwpUserAsync(string gdkey, string tableId, object data)
        {
            var account = await GetAccountFromGdkeyAsync(gdkey);
            account.YwpUserTables![tableId] = data;

            // Mark as dirty instead of updating database immediately
            account.IsDirty = true;
        }

        public static async Task SetYwpUserDictAsync(string gdkey, Dictionary<string, object?> data)
        {
            var account = await GetAccountFromGdkeyAsync(gdkey);
            foreach (var kvp in data)
            {
                account.YwpUserTables![kvp.Key] = kvp.Value!;
            }

            // Mark as dirty instead of updating database immediately
            account.IsDirty = true;
        }

        public static async Task SetEntireUserData(string gdkey, Dictionary<string, object?> data)
        {
            var account = await GetAccountFromGdkeyAsync(gdkey);
            account.YwpUserTables = data;

            account.IsDirty = true;
        }

        public static async Task DeleteUser(string udkey, string gdkey)
        {
            await RemoveGdkeyFromUdkey(udkey, gdkey);
            await SupabaseClient!.From<Account>().Where(a => a.Gdkey == gdkey).Delete();

            _accountCache.TryRemove(gdkey, out _);
        }

        private static async Task RemoveGdkeyFromUdkey(string udkey, string gdkey)
        {
            var response = await SupabaseClient!.From<Device>().Where(d => d.UdKey == udkey).Get();
            var device = response.Models.FirstOrDefault()!;
            device.Gdkeys!.Remove(gdkey);
            await device.Update<Device>();
        }

        // Gets user data from specific account
        public static async Task<T?> GetYwpUserAsync<T>(string gdkey, string tableId)
        {
            var account = await GetAccountFromGdkeyAsync(gdkey);
            var tbl = account.YwpUserTables![tableId];
            if (tbl == null)
                return default;
            JToken token = JToken.FromObject(tbl);
            return token.ToObject<T>();
        }

        public static T? GetYwpUserFromJson<T>(string tableId, Dictionary<string, object?> YwpUserTables)
        {
            var tbl = YwpUserTables[tableId];
            if (tbl == null)
                return default;
            JToken token = JToken.FromObject(tbl);
            return token.ToObject<T>();
        }

        public static async Task<Dictionary<string, object?>> GetEntireUserData(string gdkey)
        {
            var account = await GetAccountFromGdkeyAsync(gdkey);
            return account.YwpUserTables!;
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
            var account = await GetAccountFromGdkeyAsync(gdkey); // Pull from cache first
            return account?.LastLoginTime!;
        }

        // Gets all corresponding GDKeys from under a specified UDKey.
        public static async Task<List<string>> GetGdkeysFromUdkeyAsync(string udkey)
        {
            var response = await SupabaseClient!.From<Device>().Where(d => d.UdKey == udkey).Get();
            var device = response.Models.FirstOrDefault();
            return device?.Gdkeys!;
        }

        // Add a gdkey association to a udkey
        public static async Task AddAccountToDevice(string udkey, string gdkey)
        {
            var response = await SupabaseClient!.From<Device>().Where(d => d.UdKey == udkey).Get();
            var device = response.Models.FirstOrDefault()!;
            if (device.Gdkeys == null) device.Gdkeys = new List<string>();
            device.Gdkeys.Add(gdkey);
            await device.Update<Device>();
        }
    }
}