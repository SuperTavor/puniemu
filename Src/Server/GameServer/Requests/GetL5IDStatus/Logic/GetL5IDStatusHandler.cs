namespace Puniemu.Src.Server.GameServer.Requests.GetL5IDStatus.Logic
{
    public class GetL5IDStatusHandler
    {
        //Doesn't need to be async cause we're literally assigning a variable so it will run sync anyway
        public static void Handle(HttpContext ctx)
        {
            //Returning a valid response is enough for this request.
            //For some reason, sometimes the game can keep throwing error if he dont have good answer so we might need to implement it
            ctx.Response.StatusCode = 200;
        }
    }
}
