using Newtonsoft.Json;
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
    }
}
