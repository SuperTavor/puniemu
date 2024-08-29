namespace Puniemu.Src.Server.GameServer.Requests.GetL5IDStatus.Logic
{
    public class GetL5IDStatusHandler
    {
        //Doesn't need to be async cause we're literally assigning a variable so it will run sync anyway
        public static void Handle(HttpContext ctx)
        {
            //Returning a valid response is enough for this request.
            ctx.Response.StatusCode = 200;
        }
    }
}
