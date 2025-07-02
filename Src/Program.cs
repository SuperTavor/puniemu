using Microsoft.AspNetCore.Rewrite;
using Puniemu.Src.Server.GameServer.Requests.DefaultHandler.Logic;
using Puniemu.Src.Server.GameServer.Requests.GetL5IDStatus.Logic;
using Puniemu.Src.Server.GameServer.Requests.CreateUser.Logic;
using Puniemu.Src.Server.GameServer.Requests.Init.Logic;
using Puniemu.Src.Server.GameServer.Requests.GetMaster.Logic;
using Puniemu.Src.Server.L5ID.Requests.Active.Logic;
using Puniemu.Src.Server.L5ID.Requests.CreateGDKey.Logic;
using Puniemu.Src.Server.GameServer.Requests.GetGdkeyAccounts.Logic;
using Puniemu.Src.Server.GameServer.Requests.UpdateProfile.Logic;
using Puniemu.Src.Server.GameServer.Requests.DeleteUser.Logic;
using Puniemu.Src.Server.GameServer.Requests.UserInfoRefresh.Logic;
using Puniemu.Src.Server.GameServer.Requests.UserStageRanking.Logic;
using Puniemu.Src.Server.GameServer.Requests.Login.Logic;
using Puniemu.Src.Server.GameServer.Requests.BuyHitodama.Logic;
using Puniemu.Src.Server.GameServer.Requests.InitBilling.Logic;
using Puniemu.Src.Server.GameServer.Requests.GameStart.Logic;
using Puniemu.Src.Server.GameServer.Requests.DeckEdit.Logic;
using Puniemu.Src.Server.GameServer.Requests.UpdateTutorialFlag.Logic;
using Puniemu.Src.Server.GameServer.Requests.GameEnd.Logic;
using Puniemu.Src.Server.GameServer.Requests.Rename.Logic;
using Puniemu.Src.Server.GameServer.Requests.GameUseItem.Logic;
using Puniemu.Src.Server.GameServer.Requests.GameContinue.Logic;
using Puniemu.Src.Server.GameServer.Requests.GameRetire.Logic;
using Puniemu.Src.Server.GameServer.Requests.LoginStamp.Logic;
using Puniemu.Src.Utils.GeneralUtils;

namespace Puniemu.Src;
class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Add the config to DataManager so it can be used globally
        DataManager.Logic.DataManager.StaticInit(builder.Configuration);

        var app = builder.Build();
        //Rewrite to redirect mainly all .NHN requests to .NHN/, as ASP.NET Core thinks it's static serving otherwise or something
        //second rewrite is in case it's for example /////////////////////init.nhn it makes it /init.nhn
        var rewriteOptions = new RewriteOptions()
              .AddRewrite(@"^/+(.+)$", "/$1", skipRemainingRules: false)
              .AddRewrite(@"(.*\..*)$", "$1/", skipRemainingRules: true);


        app.UseRewriter(rewriteOptions);

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
        app.MapPost("/updateTutorialFlg.nhn", async ctx =>
        {
            await UpdateTutorialFlagHandler.HandleAsync(ctx);
        });
        app.MapPost("/getL5idStatus.nhn", async ctx =>
        {
            await GetL5IDStatusHandler.HandleAsync(ctx);
        });
        app.MapPost("/updateProfile.nhn", async ctx =>
        {
            await UpdateProfileHandler.HandleAsync(ctx);
        });
        app.MapPost("/deleteUser.nhn", async ctx =>
        {
            await DeleteUserHandler.HandleAsync(ctx);
        });
        app.MapPost("/userInfoRefresh.nhn", async ctx =>
        {
            await UserInfoRefreshHandler.HandleAsync(ctx);
        });
        app.MapPost("/userStageRanking.nhn", async ctx =>
        {
            await UserStageRankingHandler.HandleAsync(ctx);
        });
        app.MapPost("/login.nhn", async ctx =>
        {
            await LoginHandler.HandleAsync(ctx);
        });
        app.MapPost("/buyHitodama.nhn", async ctx =>
        {
            await BuyHitodamaHandler.HandleAsync(ctx);
        });
        app.MapPost("/initBilling.nhn", async ctx =>
        {
            await InitBillingHandler.HandleAsync(ctx);
        });
        app.MapPost("/gameStart.nhn", async ctx =>
        {
            await GameStartHandler.HandleAsync(ctx);
        });
        app.MapPost("/deckEdit.nhn", async ctx =>
        {
            await DeckEditHandler.HandleAsync(ctx);
        });
        app.MapPost("/gameEnd.nhn", async ctx =>
        {
            await GameEndHandler.HandleAsync(ctx);
        });
        app.MapPost("/rename.nhn", async ctx =>
        {
            await RenameHandler.HandleAsync(ctx);
        });
        app.MapPost("/gameUseItem.nhn", async ctx =>
        {
            await GameUseItemHandler.HandleAsync(ctx);
        });
        app.MapPost("/gameRetire.nhn", async ctx =>
        {
            await GameRetireHandler.HandleAsync(ctx);
        });
        app.MapPost("/gameContinue.nhn", async ctx =>
        {
            await GameContinueHandler.HandleAsync(ctx);
        });
        app.MapPost("/loginStamp.nhn", async ctx =>
        {
            await LoginStampHandler.HandleAsync(ctx);
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
