using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using NpgsqlTypes;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.UserDataManager.DataClasses;
using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace Puniemu.Src.UserDataManager.Logic
{
    public static class UserDataManager
    {
        public class TableNotFoundException : Exception
        {
            public TableNotFoundException() : base() { }
        }

        // Thrown when the account cache is at capacity and a not-yet-cached player tries to load
        public class ServerFullException : Exception
        {
            public ServerFullException() : base() { }
        }

        private static NpgsqlDataSource? _dataSource;

        private static ConcurrentDictionary<string, Account> _accountCache = new();

        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _accountLocks = new();
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _deviceLocks = new();

        // Timer variables for background flushing
        private static PeriodicTimer? _flushTimer;
        private static Task? _flushTask;
        private static CancellationTokenSource _cts = new();

        private static SemaphoreSlim GetAccountLock(string gdkey) => _accountLocks.GetOrAdd(gdkey, _ => new SemaphoreSlim(1, 1));

        private static SemaphoreSlim GetDeviceLock(string udkey) => _deviceLocks.GetOrAdd(udkey, _ => new SemaphoreSlim(1, 1));

        // connect to postgres
        public static void Initialize()
        {
            try
            {
                _dataSource = NpgsqlDataSource.Create(DataManager.Logic.DataManager.PostgresConnectionString!);

                _flushTimer = new PeriodicTimer(TimeSpan.FromSeconds(60));
                _flushTask = FlushLoopAsync(_cts.Token);
                Console.WriteLine("db service started");
            }
            catch
            {
                Console.WriteLine("Couldn't create postgres data source.");
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
            await Parallel.ForEachAsync(_accountCache.Keys, async (gdkey, _) =>
            {
                await FlushAccount(gdkey);
                Console.WriteLine($"Saved account. gdkey:" + gdkey);
            });
        }

        private static async Task FlushAccount(string gdkey)
        {
            if (!_accountCache.TryGetValue(gdkey, out var account)) return;

            var accountLock = GetAccountLock(gdkey);
            await accountLock.WaitAsync();
            try
            {
                if (account.IsDirty)
                {
                    account.IsDirty = false;

                    try
                    {
                        await UpdateAccountNoLockAsync(account);
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
            finally
            {
                accountLock.Release();
            }
        }
        public static async Task ShutdownAsync()
        {
            _cts.Cancel();
            await FlushAllDirtyAccountsAsync();
        }

        public static async Task<Account> GetAccountFromGdkeyAsync(string gdkey)
        {
            if (_accountCache.TryGetValue(gdkey, out Account? acc))
            {
                return acc;
            }
            else
            {
                ThrowIfCacheFull();

                await using var cmd = _dataSource!.CreateCommand(
                    "SELECT gdkey, character_id, udkey, user_id, ywp_user_tables, last_lgn_time, start_date, opening_tutorial_flag FROM account WHERE gdkey = @gdkey");
                cmd.Parameters.AddWithValue("gdkey", gdkey);
                await using var reader = await cmd.ExecuteReaderAsync();
                Account? account = null;
                if (await reader.ReadAsync())
                {
                    account = ReadAccount(reader);
                    _accountCache[gdkey] = account;
                }
                return account!;
            }
        }

        public static async Task<string> NewDeviceAsync(string? udkey = null)
        {
            udkey ??= Guid.NewGuid().ToString();
            await using var cmd = _dataSource!.CreateCommand(
                "INSERT INTO device (udkey, gdkeys) VALUES (@udkey, @gdkeys) RETURNING udkey");
            cmd.Parameters.AddWithValue("udkey", udkey);
            cmd.Parameters.AddWithValue("gdkeys", new List<string>());
            var newUdkey = (string)(await cmd.ExecuteScalarAsync())!;
            return newUdkey;
        }

        // checks if udkey exists
        public static async Task<bool> IsDeviceExists(string udkey)
        {
            await using var cmd = _dataSource!.CreateCommand("SELECT COUNT(*) FROM device WHERE udkey = @udkey");
            cmd.Parameters.AddWithValue("udkey", udkey);
            var count = (long)(await cmd.ExecuteScalarAsync())!;
            return count > 0;
        }

        private static void ThrowIfCacheFull()
        {
            if (_accountCache.Count >= DataManager.Logic.DataManager.MaxCachedAccounts)
            {
                throw new ServerFullException();
            }
        }

        // returns the gdkey of the newly created account
        public static async Task<string> NewAccountAsync()
        {
            ThrowIfCacheFull();

            var fc = GenerateFriendCode();
            var acc = new Account()
            {
                Gdkey = Guid.NewGuid().ToString(),
                YwpUserTables = new(),
                LastLoginTime = "",
                CharacterId = fc,
                UserId = System.IO.Hashing.Crc32.HashToUInt32(System.Text.Encoding.UTF8.GetBytes(fc)).ToString()
            };
            await using var cmd = _dataSource!.CreateCommand(
                "INSERT INTO account (gdkey, character_id, udkey, user_id, ywp_user_tables, last_lgn_time, start_date, opening_tutorial_flag) " +
                "VALUES (@gdkey, @character_id, @udkey, @user_id, @ywp_user_tables, @last_lgn_time, @start_date, @opening_tutorial_flag)");
            AddAccountParameters(cmd, acc);
            await cmd.ExecuteNonQueryAsync();

            // Immediately cache the new account
            _accountCache[acc.Gdkey!] = acc;
            return acc.Gdkey!;
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

            var accountLock = GetAccountLock(gdkey);
            await accountLock.WaitAsync();
            try
            {
                account.YwpUserTables![tableId] = data;

                // Mark as dirty instead of updating database immediately
                account.IsDirty = true;
            }
            finally
            {
                accountLock.Release();
            }
        }

        public static async Task SetYwpUserDictAsync(string gdkey, Dictionary<string, object?> data)
        {
            var account = await GetAccountFromGdkeyAsync(gdkey);

            var accountLock = GetAccountLock(gdkey);
            await accountLock.WaitAsync();
            try
            {
                foreach (var kvp in data)
                {
                    account.YwpUserTables![kvp.Key] = kvp.Value!;
                }

                // Mark as dirty instead of updating database immediately
                account.IsDirty = true;
            }
            finally
            {
                accountLock.Release();
            }
        }

        public static async Task SetEntireUserData(string gdkey, Dictionary<string, object?> data)
        {
            var account = await GetAccountFromGdkeyAsync(gdkey);

            var accountLock = GetAccountLock(gdkey);
            await accountLock.WaitAsync();
            try
            {
                account.YwpUserTables = data;

                account.IsDirty = true;
            }
            finally
            {
                accountLock.Release();
            }
        }

        public static async Task DeleteUser(string udkey, string gdkey)
        {
            await RemoveGdkeyFromUdkey(udkey, gdkey);

            var accountLock = GetAccountLock(gdkey);
            await accountLock.WaitAsync();
            try
            {
                await using var cmd = _dataSource!.CreateCommand("DELETE FROM account WHERE gdkey = @gdkey");
                cmd.Parameters.AddWithValue("gdkey", gdkey);
                await cmd.ExecuteNonQueryAsync();

                _accountCache.TryRemove(gdkey, out _);
            }
            finally
            {
                accountLock.Release();
            }
        }

        private static async Task RemoveGdkeyFromUdkey(string udkey, string gdkey)
        {
            var deviceLock = GetDeviceLock(udkey);
            await deviceLock.WaitAsync();
            try
            {
                await using var cmd = _dataSource!.CreateCommand(
                    "UPDATE device SET gdkeys = array_remove(gdkeys, @gdkey) WHERE udkey = @udkey");
                cmd.Parameters.AddWithValue("gdkey", gdkey);
                cmd.Parameters.AddWithValue("udkey", udkey);
                await cmd.ExecuteNonQueryAsync();
            }
            finally
            {
                deviceLock.Release();
            }
        }

        // Gets user data from specific account
        public static async Task<T?> GetYwpUserAsync<T>(string gdkey, string tableId)
        {
            var account = await GetAccountFromGdkeyAsync(gdkey);
            if (account == null) return default;
            if (!account.YwpUserTables.TryGetValue(tableId, out var tbl) || tbl == null)
                return default;
            JToken token = JToken.FromObject(tbl);
            var obj = token.ToObject<T>();

            if (obj is YwpUserData ywp)
            {
                await ywp.HitodamaRecover(gdkey);
            }

            return obj;
        }

        public static async Task DeleteYwpUserAsync(string gdkey, string tableId)
        {
            var account = await GetAccountFromGdkeyAsync(gdkey);

            var accountLock = GetAccountLock(gdkey);
            await accountLock.WaitAsync();
            try
            {
                account.YwpUserTables!.Remove(tableId);
                account.IsDirty = true;
            }
            finally
            {
                accountLock.Release();
            }
        }
        public static async Task<T?> GetYwpUserFromJson<T>(string tableId, Dictionary<string, object?> YwpUserTables, string? gdkey)
        {
            var tbl = YwpUserTables[tableId];
            if (tbl == null)
                return default;
            JToken token = JToken.FromObject(tbl);
            var obj = token.ToObject<T>();

            if (gdkey != null && obj is YwpUserData ywp)
            {
                await ywp.HitodamaRecover(gdkey);
            }
            return obj;
        }

        public static async Task<Dictionary<string, object?>> GetEntireUserData(string gdkey)
        {
            var account = await GetAccountFromGdkeyAsync(gdkey);
            return account.YwpUserTables!;
        }

        public static async Task<string> GetGdkeyFromCharacterId(string charId)
        {
            await using var cmd = _dataSource!.CreateCommand("SELECT gdkey FROM account WHERE character_id = @character_id");
            cmd.Parameters.AddWithValue("character_id", charId);
            var gdkey = await cmd.ExecuteScalarAsync() as string;
            return gdkey ?? string.Empty;
        }

        public static async Task TransferGdkeys(string udkeyFrom, string udkeyTo)
        {
            // Always take the locks in the same order to avoid deadlocks
            var firstKey = string.CompareOrdinal(udkeyFrom, udkeyTo) <= 0 ? udkeyFrom : udkeyTo;
            var secondKey = firstKey == udkeyFrom ? udkeyTo : udkeyFrom;
            var firstLock = GetDeviceLock(firstKey);
            var secondLock = GetDeviceLock(secondKey);

            await firstLock.WaitAsync();
            try
            {
                if (secondKey != firstKey) await secondLock.WaitAsync();
                try
                {
                    // Both updates happen in one transaction so a crash can't leave the transfer half-applied
                    await using var conn = await _dataSource!.OpenConnectionAsync();
                    await using var tx = await conn.BeginTransactionAsync();

                    var fromGdkeys = await GetDeviceGdkeysAsync(udkeyFrom, conn, tx);
                    var toGdkeys = await GetDeviceGdkeysAsync(udkeyTo, conn, tx);

                    if (fromGdkeys == null)
                        throw new Exception($"Device {udkeyFrom} not found");

                    if (toGdkeys == null)
                        throw new Exception($"Device {udkeyTo} not found");

                    var newToGdkeys = new List<string>();

                    foreach (var gdkey in fromGdkeys)
                    {
                        if (!newToGdkeys.Contains(gdkey))
                            newToGdkeys.Add(gdkey);
                    }

                    await SetDeviceGdkeysAsync(udkeyTo, newToGdkeys, conn, tx);
                    await SetDeviceGdkeysAsync(udkeyFrom, new List<string>(), conn, tx);

                    await tx.CommitAsync();
                }
                finally
                {
                    if (secondKey != firstKey) secondLock.Release();
                }
            }
            finally
            {
                firstLock.Release();
            }
        }
        public static async Task<Mail> GetDataByMail(string email)
        {
            await using var cmd = _dataSource!.CreateCommand("SELECT mail, \"currentUdkey\" FROM mail WHERE mail = @mail");
            cmd.Parameters.AddWithValue("mail", email);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Mail
                {
                    MailAddress = reader.GetString(0),
                    CurrentUdkey = reader.IsDBNull(1) ? null : reader.GetString(1)
                };
            }
            return null!;
        }
        public static async Task AddOrEditEmail(string email, string udkey)
        {
            await using var cmd = _dataSource!.CreateCommand(
                "INSERT INTO mail (mail, \"currentUdkey\") VALUES (@mail, @udkey) " +
                "ON CONFLICT (mail) DO UPDATE SET \"currentUdkey\" = EXCLUDED.\"currentUdkey\"");
            cmd.Parameters.AddWithValue("mail", email);
            cmd.Parameters.AddWithValue("udkey", udkey);
            await cmd.ExecuteNonQueryAsync();
        }
        public static async Task<string> GetGdkeyFromUserId(string userId)
        {
            await using var cmd = _dataSource!.CreateCommand("SELECT gdkey FROM account WHERE user_id = @user_id");
            cmd.Parameters.AddWithValue("user_id", userId);
            var gdkey = await cmd.ExecuteScalarAsync() as string;
            return gdkey ?? string.Empty;
        }

        public static async Task<string> GetLastLoginTime(string gdkey)
        {
            var account = await GetAccountFromGdkeyAsync(gdkey); // Pull from cache first
            return account?.LastLoginTime!;
        }

        // Gets all corresponding GDKeys from under a specified UDKey.
        public static async Task<List<string>> GetGdkeysFromUdkeyAsync(string udkey)
        {
            var gdkeys = await GetDeviceGdkeysAsync(udkey);

            if (gdkeys != null)
            {
                foreach (var gdkey in gdkeys)
                {
                    var account = await GetAccountFromGdkeyAsync(gdkey);

                    if (string.IsNullOrEmpty(account.Udkey))
                    {
                        var accountLock = GetAccountLock(gdkey);
                        await accountLock.WaitAsync();
                        try
                        {
                            account.Udkey = udkey;
                            account.IsDirty = true;
                        }
                        finally
                        {
                            accountLock.Release();
                        }
                    }
                }
            }

            return gdkeys ?? new List<string>();
        }

        // Add a gdkey association to a udkey
        public static async Task AddAccountToDevice(string udkey, string gdkey)
        {
            var deviceLock = GetDeviceLock(udkey);
            await deviceLock.WaitAsync();
            try
            {
                await using var cmd = _dataSource!.CreateCommand(
                    "UPDATE device SET gdkeys = array_append(gdkeys, @gdkey) WHERE udkey = @udkey");
                cmd.Parameters.AddWithValue("gdkey", gdkey);
                cmd.Parameters.AddWithValue("udkey", udkey);
                await cmd.ExecuteNonQueryAsync();
            }
            finally
            {
                deviceLock.Release();
            }
        }

        // Writes the account back to the database immediately
        public static async Task UpdateAccountAsync(Account account)
        {
            var accountLock = GetAccountLock(account.Gdkey!);
            await accountLock.WaitAsync();
            try
            {
                await UpdateAccountNoLockAsync(account);
            }
            finally
            {
                accountLock.Release();
            }
        }

        private static async Task UpdateAccountNoLockAsync(Account account)
        {
            await using var cmd = _dataSource!.CreateCommand(
                "UPDATE account SET character_id = @character_id, udkey = @udkey, user_id = @user_id, ywp_user_tables = @ywp_user_tables, " +
                "last_lgn_time = @last_lgn_time, start_date = @start_date, opening_tutorial_flag = @opening_tutorial_flag WHERE gdkey = @gdkey");
            AddAccountParameters(cmd, account);
            await cmd.ExecuteNonQueryAsync();
        }

        private static void AddAccountParameters(NpgsqlCommand cmd, Account account)
        {
            cmd.Parameters.AddWithValue("gdkey", account.Gdkey!);
            cmd.Parameters.AddWithValue("character_id", (object?)account.CharacterId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("udkey", (object?)account.Udkey ?? DBNull.Value);
            cmd.Parameters.AddWithValue("user_id", (object?)account.UserId ?? DBNull.Value);
            cmd.Parameters.Add(new NpgsqlParameter("ywp_user_tables", NpgsqlDbType.Jsonb)
            {
                Value = account.YwpUserTables != null ? JsonConvert.SerializeObject(account.YwpUserTables) : DBNull.Value
            });
            cmd.Parameters.AddWithValue("last_lgn_time", (object?)account.LastLoginTime ?? DBNull.Value);
            // start_date is a text column in the schema
            cmd.Parameters.AddWithValue("start_date", account.StartDate.ToString());
            cmd.Parameters.AddWithValue("opening_tutorial_flag", account.OpeningTutorialFlag);
        }

        private static Account ReadAccount(NpgsqlDataReader reader)
        {
            var tablesJson = reader.IsDBNull(4) ? null : reader.GetString(4);
            var startDateText = reader.IsDBNull(6) ? null : reader.GetString(6);
            return new Account
            {
                Gdkey = reader.GetString(0),
                CharacterId = reader.IsDBNull(1) ? null : reader.GetString(1),
                Udkey = reader.IsDBNull(2) ? null : reader.GetString(2),
                UserId = reader.IsDBNull(3) ? null : reader.GetString(3),
                YwpUserTables = tablesJson != null ? JsonConvert.DeserializeObject<Dictionary<string, object?>>(tablesJson) : null,
                LastLoginTime = reader.IsDBNull(5) ? null : reader.GetString(5),
                StartDate = long.TryParse(startDateText, out var startDate) ? startDate : 0,
                OpeningTutorialFlag = !reader.IsDBNull(7) && reader.GetBoolean(7)
            };
        }

        // Returns null if the device doesn't exist. Pass conn/tx to run inside an existing transaction.
        private static async Task<List<string>?> GetDeviceGdkeysAsync(string udkey, NpgsqlConnection? conn = null, NpgsqlTransaction? tx = null)
        {
            await using var cmd = conn != null
                ? new NpgsqlCommand("SELECT gdkeys FROM device WHERE udkey = @udkey", conn, tx)
                : _dataSource!.CreateCommand("SELECT gdkeys FROM device WHERE udkey = @udkey");
            cmd.Parameters.AddWithValue("udkey", udkey);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetFieldValue<List<string>>(0);
            }
            return null;
        }

        private static async Task SetDeviceGdkeysAsync(string udkey, List<string> gdkeys, NpgsqlConnection? conn = null, NpgsqlTransaction? tx = null)
        {
            await using var cmd = conn != null
                ? new NpgsqlCommand("UPDATE device SET gdkeys = @gdkeys WHERE udkey = @udkey", conn, tx)
                : _dataSource!.CreateCommand("UPDATE device SET gdkeys = @gdkeys WHERE udkey = @udkey");
            cmd.Parameters.AddWithValue("gdkeys", gdkeys);
            cmd.Parameters.AddWithValue("udkey", udkey);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
