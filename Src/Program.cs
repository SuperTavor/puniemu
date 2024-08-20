using Puniemu.src.Utils.UserDataManager;
using Puniemu.Src.ConfigManager;
using Puniemu.Src.Server.GameServer;
using Puniemu.Src.Server.GameServer.Init;
using Puniemu.Src.Server.L5ID.API.V1.Active;
using Microsoft.AspNetCore.Rewrite;

namespace Puniemu.Src;
class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var app = builder.Build();
        //Rewrite to redirect mainly all .NHN requests to .NHN/, as ASP.NET Core thinks it's static serving otherwise or something
        var rewriteOptions = new RewriteOptions()
            .AddRewrite(@"(.*\..*)$", "$1/", skipRemainingRules: true);
        app.UseRewriter(rewriteOptions);

        CConfigManager.Initialize();

        //Init database connection
        CUserDataManager.Initialize();

        app.UseHttpsRedirection();
        //Assign handlers
        AssignL5IDHandlers(app);
        AssignGameServerHandlers(app);
        AssignDefault(app);
        app.Run();
    }

    static void AssignL5IDHandlers(WebApplication app)
    {
        const string L5ID_BASE = "/l5id/api/v1";
        app.MapGet(L5ID_BASE+"/active", async ctx =>
        {
            await CActiveHandler.HandleAsync(ctx);
        });
    }

    static void AssignGameServerHandlers(WebApplication app)
    {
        app.MapPost("/init.nhn", async ctx =>
        {
            await CInitHandler.HandleAsync(ctx);
        });
    }

    //Assigns all other, unknown request paths
    static void AssignDefault(WebApplication app)
    {
        app.MapFallback(async context =>
        {
            await CDefaultHandler.HandleAsync(context);
        });
    }

}