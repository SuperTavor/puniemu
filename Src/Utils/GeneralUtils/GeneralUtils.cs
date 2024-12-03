using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.ConfigManager.Logic;

namespace Puniemu.Src.Utils.GeneralUtils
{
    public class GeneralUtils
    {
        public static async Task SendBadRequest(HttpContext ctx)
        {
            ctx.Response.Headers.ContentType = "text/plain";
            ctx.Response.StatusCode = 400;
            await ctx.Response.WriteAsync("Bad request");
        }

        public static T DeserializeGameDataToTypeAndCheckValidity<T>(string gamedataName)
        {
            T? output = JsonConvert.DeserializeObject<T>(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache[gamedataName]);
            if (output == null) throw new FormatException($"{gamedataName} static gamedata should be a(n) {typeof(T).FullName}");
            return output;
        }

        public static async Task AddTablesToResponse(IEnumerable<string> tables, Dictionary<string,object> resultDictionary, bool isDownloadOnce, string gdkey = "")
        {
            Dictionary<string, object>? userTables = null;

            if(isDownloadOnce && gdkey != string.Empty)
            {
                userTables = await UserDataManager.Logic.UserDataManager.GetEntireUserData(gdkey);
            }
            foreach(var table in tables)
            {
                string? tableText = null!;
                object? tableObj = new();
                if (table.StartsWith("ywp_user"))
                {
                    if(isDownloadOnce && userTables != null)
                    {
                        tableObj = userTables[table];
                    }
                    else
                    {
                        tableObj = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<object>(gdkey, table);
                    }
                }
                //Meaning it's constant
                else tableText = ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache[table];

                if (tableText != null)
                {
                    //if we can't deserialize json it means it's not a json and we store it as is
                    try
                    {
                        // If was cud structure, only send the data
                        tableObj = JsonConvert.DeserializeObject<object>(tableText);
                        try
                        {
                            var dict = (Dictionary<string, object>)tableObj;
                            if (dict!.ContainsKey("data"))
                            {
                                tableObj = dict["data"];
                            }
                            else if (dict.ContainsKey("tableData"))
                            {
                                tableObj = dict["tableData"];
                            }

                        }
                        catch
                        {
                            //Just continue everything as normal if it's not a dict
                        }
                        
                    }
                    catch
                    {
                        tableObj = tableText;
                    }
                }
                resultDictionary[table] = tableObj!;
            }
        }
    }
}
