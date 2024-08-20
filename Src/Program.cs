using Puniemu.src.Utils.UserDataManager;
using Puniemu.Src.ConfigManager;
using Puniemu.Src.Server.GameServer;
using Puniemu.Src.Server.GameServer.Init;
using Puniemu.Src.Server.L5ID.API.V1.Active;

namespace Puniemu.Src;
class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var app = builder.Build();
        //Init database connection
        CUserDataManager.Initialize();
        CConfigManager.Initialize();

        app.UseHttpsRedirection();
        //Assign handlers
        AssignL5IDHandlers(app);

        app.Run();
    }

    static void AssignL5IDHandlers(WebApplication app)
    {
        app.MapGet("/l5id/api/v1/active", CActiveHandler.HandleAsync);
    }

    static void AssignGameServerHandlers(WebApplication app)
    {
        app.MapGet("/init.nhn", CInitHandler.HandleAsync);
    }

    //Assigns all other, unknown request paths
    static void AssignDefault(WebApplication app)
    {
        app.MapGet("/",CDefaultHandler.HandleAsync);    
    }
}