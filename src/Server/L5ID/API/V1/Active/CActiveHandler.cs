using Puniemu.Src.Server.L5ID.API.V1.Active.DataClasses;
using Puniemu.Src.Server.L5ID.DataClasses;
namespace Puniemu.Src.Server.L5ID.API.V1.Active
{
    //This call is seeimgly used to check the validity of udkeys and gdkeys on the L5id server.
    public class CActiveHandler
    {
        public static async void HandleAsync(HttpContext ctx)
        {
            var queryParams = ctx.Request.Query;
            ctx.Response.ContentType = "application/json";
            if(!queryParams.ContainsKey("udkey"))
            {
                var res = new CGoodActiveResponse(CKey.GenerateKey("-d"));
            }
        }
    }
}
