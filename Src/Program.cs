using Microsoft.AspNetCore.Rewrite;
using Puniemu.Src.Server.GameServer.Requests.DefaultHandler.Logic;
using Puniemu.Src.Server.GameServer.Requests.GetL5IDStatus.Logic;
using Puniemu.Src.Server.GameServer.Requests.CreateUser.Logic;
using Puniemu.Src.Server.GameServer.Requests.Init.Logic;
using Puniemu.Src.Server.GameServer.Requests.GetMaster.Logic;
using Puniemu.Src.Server.L5ID.Requests.Active.Logic;
using Puniemu.Src.Server.L5ID.Requests.CreateGDKey.Logic;
using Puniemu.Src.ConfigManager.Logic;
using Puniemu.Src.Server.GameServer.UpdateProfile;
using Puniemu.Src.Server.GameServer.Requests.GetGdkeyAccounts.Logic;

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

        ConfigManager.Logic.ConfigManager.Initialize();

        //Init database connection
        UserDataManager.Logic.UserDataManager.Initialize();

        app.UseHttpsRedirection();
        //Assign handlers
        AssignL5IDHandlers(app);
        AssignGameServerHandlers(app);
        AssignDefault(app);
        app.Run();
    }

    static void AssignL5IDHandlers(WebApplication app)
    {
        const string L5ID_BASE = "/l5id/api/v1/";
        app.MapGet(L5ID_BASE+"active/", async ctx =>
        {
            await ActiveHandler.HandleAsync(ctx);
        });
        app.MapGet(L5ID_BASE + "create_gdkey/", async ctx =>
        {
            await CreateGDKeyHandler.HandleAsync(ctx);
        });
    }

    static void AssignGameServerHandlers(WebApplication app)
    {
        app.MapPost("/init.nhn", async ctx =>
        {
            await InitHandler.HandleAsync(ctx);
        });
        app.MapPost("/getMaster.nhn", async ctx =>
        {
            await GetMasterHandler.HandleAsync(ctx);
        });
        app.MapPost("/createUser.nhn", async ctx =>
        {
            await CreateUserHandler.HandleAsync(ctx);
        });
        app.MapPost("/getGdkeyAccounts.nhn", async ctx =>
        {
            await GetGdkeyAccountsHandler.HandleAsync(ctx);
        });
        app.MapPost("/getL5idStatus.nhn", GetL5IDStatusHandler.Handle);
        app.MapPost("/updateProfile.nhn", async ctx =>
        {
            await UpdateProfileHandler.HandleAsync(ctx);
        });
    }

    //Assigns all other, unknown request paths
    static void AssignDefault(WebApplication app)
    {
        app.MapFallback(async context =>
        {
            await DefaultHandler.HandleAsync(context);
        });
    }

}
