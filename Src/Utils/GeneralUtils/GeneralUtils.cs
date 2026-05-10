using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.DBService.Logic;
using Puniemu.Src.DBService.DataClasses;
using System.Linq.Expressions;

namespace Puniemu.Src.Utils.GeneralUtils
{
    public static class GeneralUtils
    {
        public static async Task SendBadRequest(HttpContext ctx)
        {
            ctx.Response.Headers.ContentType = "text/plain";
            ctx.Response.StatusCode = 400;
            await ctx.Response.WriteAsync("Bad request");
        }

        public static T DeserializeGameDataToTypeAndCheckValidity<T>(string gamedataName)
        {
            T? output = JsonConvert.DeserializeObject<T>(DataManager.Logic.DataManager.GameDataManager!.GamedataCache[gamedataName]);
            if (output == null) throw new FormatException($"{gamedataName} static gamedata should be a(n) {typeof(T).FullName}");
            return output;
        }

        public static async Task AddTablesToResponse(
       IEnumerable<string> tables,
       Dictionary<string, object?> resultDictionary,
       bool preloadUserTables,
       string gdkey = "")
        {
            DBService.DataClasses.Account? acc = null;

            if (preloadUserTables && !string.IsNullOrEmpty(gdkey))
            {
                acc = await DBService.Logic.DBService.GetAccountObjectAsync(gdkey);
            }

            foreach (var table in tables)
            {
                object? tableObj = null;

                // -----------------------------
                // USER TABLES (ywp_user_*)
                // -----------------------------
                if (table.StartsWith("ywp_user"))
                {
                    if (preloadUserTables && acc != null)
                    {
                        tableObj = acc.GetFieldByJsonName<object>(table, true);
                    }
                    else
                    {
                        tableObj = await DBService.Logic.DBService.GetYwpUserAsync<object>(gdkey, table);
                    }
                }
                // -----------------------------
                // STATIC GAME DATA
                // -----------------------------
                else
                {
                    string? tableText =
                        DataManager.Logic.DataManager.GameDataManager!.GamedataCache[table];

                    if (!string.IsNullOrEmpty(tableText))
                    {
                        try
                        {
                            JToken token = JToken.Parse(tableText);

                            if (token is JObject obj)
                            {
                                if (obj.TryGetValue("data", out JToken? data))
                                {
                                    tableObj = data;
                                }
                                else if (obj.TryGetValue("tableData", out JToken? tableData))
                                {
                                    tableObj = tableData;
                                }
                                else
                                {
                                    tableObj = obj;
                                }
                            }
                            else
                            {
                                tableObj = token;
                            }
                        }
                        catch
                        {
                            tableObj = tableText;
                        }
                    }
                }

                resultDictionary[table] = tableObj;
            }
        }
    }
}
