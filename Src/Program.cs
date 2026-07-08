using Microsoft.AspNetCore.Rewrite;
using Puniemu.Src.Server.GameServer.Requests.DefaultHandler.Logic;
using Puniemu.Src.Server.GameServer.Requests.GetL5IDStatus.Logic;
using Puniemu.Src.Server.GameServer.Requests.CreateUser.Logic;
using Puniemu.Src.Server.GameServer.Requests.Init.Logic;
using Puniemu.Src.Server.GameServer.Requests.GetMaster.Logic;
using Puniemu.Src.Server.L5ID.Requests.CreateGDKey.Logic;
using Puniemu.Src.Server.GameServer.Requests.GetGdkeyAccounts.Logic;
using Puniemu.Src.Server.GameServer.Requests.UpdateProfile.Logic;
using Puniemu.Src.Server.GameServer.Requests.DeleteUser.Logic;
using Puniemu.Src.Server.GameServer.Requests.UserInfoRefresh.Logic;
using Puniemu.Src.Server.GameServer.Requests.UserStageRanking.Logic;
using Puniemu.Src.Server.GameServer.Requests.Login.Logic;
using Puniemu.Src.Server.GameServer.Requests.BuyHitodama.Logic;
using Puniemu.Src.Server.GameServer.Requests.InitBilling.Logic;
using Puniemu.Src.Server.GameServer.Requests.DeckEdit.Logic;
using Puniemu.Src.Server.GameServer.Requests.GameEnd.Logic;
using Puniemu.Src.Server.GameServer.Requests.GameEnd;
using Puniemu.Src.Server.GameServer.Requests.Rename.Logic;
using Puniemu.Src.Server.GameServer.Requests.GameUseItem.Logic;
using Puniemu.Src.Server.GameServer.Requests.GameContinue.Logic;
using Puniemu.Src.Server.GameServer.Requests.LoginStamp.Logic;
using Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.Logic;
using Puniemu.Src.Server.GameServer.Requests.InitCollectMenu.Logic;
using Puniemu.Src.Server.GameServer.Requests.Friend.Logic;
using Puniemu.Src.Server.GameServer.Requests.FriendRequest.Logic;
using Puniemu.Src.Server.GameServer.Requests.FriendSearch.Logic;
using Puniemu.Src.Server.GameServer.Requests.FriendRequestDelete.Logic;
using Puniemu.Src.Server.GameServer.Requests.InitGoku.Logic;
using Puniemu.Src.Server.GameServer.Requests.UpdateGokuStory.Logic;
using Puniemu.Src.Server.GameServer.Requests.UpdateGokuMenu.Logic;
using Puniemu.Src.Server.GameServer.Requests.InitCrystal.Logic;
using Puniemu.Src.Server.GameServer.Requests.UpdateCrystalMenu.Logic;
using Puniemu.Src.Server.GameServer.Requests.FriendDelete.Logic;
using Puniemu.Src.Server.GameServer.Requests.GetPresentBox.Logic;
using Puniemu.Src.Server.GameServer.Requests.GameEndScoreAttack.Logic;
using Puniemu.Src.Server.GameServer.Requests.InitScoreAttack.Logic;
using Puniemu.Src.Server.GameServer.Requests.StartScoreAttack.Logic;
using Puniemu.Src.Server.GameServer.Requests.MapWarp.Logic;
using Puniemu.Src.Server.GameServer.Requests.MapUnLock.Logic;


using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.Server.GameServer.Requests.FriendRequestAccept.Logic;
using Puniemu.Src.Server.GameServer.Requests.GetRanking.Logic;
using Puniemu.Src.Server.L5ID.Requests;
using Puniemu.Src.DataManager.Logic;
using Puniemu.Src.Server.GameServer.Requests.UpdateTutorialFlag.Logic;
using Puniemu.Src.Server.GameServer.Requests.Game.GameStart.Logic;
using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.Init.InitGacha.Logic.Puni;
using Puniemu.Src.Server.GameServer.Requests.Init.InitGacha.Logic.WibWob;
using Puniemu.Src.Server.GameServer.Requests.BuyItem.Logic;
using Puniemu.Src.Server.GameServer.Requests.Map.Map.Logic;
using Puniemu.Src.Server.GameServer.Requests.LevelLockOff.Logic;
using Puniemu.Src.Server.GameServer.Requests.GetMission.Logic;
using Puniemu.Src.Server.GameServer.Requests.UseItem.Logic;
using Puniemu.Src.Server.GameServer.Requests.Watch.InitWatch.Logic;
using Puniemu.Src.Server.GameServer.Requests.Watch.UpdateWatchReadFlg.Logic;
using Puniemu.Src.Server.GameServer.Requests.Conflate.Logic;
using Puniemu.Src.Server.GameServer.Requests.UseAddition.Logic;
using Puniemu.Src.Server.GameServer.Requests.MissionReward.Logic;
using Puniemu.Src.Server.CustomAuth.Requests.Link.Logic;
using Puniemu.Src.Server.GameServer.Requests.SerialConfirm.Logic;
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
           .AddRewrite(@"^(.+\.nhn)$", "$1/", skipRemainingRules: false);


        app.UseRewriter(rewriteOptions);

        //Init database connection
        UserDataManager.Logic.UserDataManager.Initialize();

        //Add shutdown async from userdatadb
        app.Lifetime.ApplicationStopping.Register(() =>
        {
            Console.WriteLine("server stopping: flushing accounts");

            try
            {
                Task.Run(async () =>
                {
                    await UserDataManager.Logic.UserDataManager.ShutdownAsync();
                }).GetAwaiter().GetResult();

                Console.WriteLine("flush complete");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        });

        app.UseHttpsRedirection();
        app.Use(async (ctx, next) =>
        {
            var path = ctx.Request.Path.Value;

            if (path != null && path.StartsWith("/eal/") && path.EndsWith("/"))
            {
                ctx.Request.Path = path.TrimEnd('/');
            }

            await next();
        });
        //Assign handlers
        AssignCustomAuthHandlers(app);
        AssignDataDownloadHandler(app);
        AssignL5IDHandlers(app);
        AssignGameServerHandlers(app);
        AssignDefault(app);
        app.Run();
    }

    static void AssignDataDownloadHandler(WebApplication app)
    {
        app.MapGet("/eal/{*filePath}", async (HttpContext ctx, string filePath) =>
        {
            Console.WriteLine(filePath);

            if (string.IsNullOrEmpty(filePath))
            {
                ctx.Response.StatusCode = 400;
                await ctx.Response.WriteAsync("no file bro");
                return;
            }

            string storageRoot = Path.Combine(Directory.GetCurrentDirectory(), "dataDownload");

            string fullPath = Path.GetFullPath(Path.Combine(storageRoot, filePath));

            if (!fullPath.StartsWith(storageRoot, StringComparison.OrdinalIgnoreCase) || !File.Exists(fullPath))
            {
                ctx.Response.StatusCode = 404;
                await ctx.Response.WriteAsync("404 ");
                return;
            }

            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fullPath, out string? contentType))
            {
                contentType = "application/octet-stream"; 
            }

            string fileName = Path.GetFileName(fullPath);
            ctx.Response.ContentType = contentType;

            ctx.Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{fileName}\"");

            await ctx.Response.SendFileAsync(fullPath);
        });
    }
    static void AssignL5IDHandlers(WebApplication app)
    {
        const string L5ID_BASE = "/api/v1/";
        // for Puni 
        app.MapGet("/l5id" + L5ID_BASE +"active/", async ctx =>
        {
            await Puniemu.Src.Server.L5ID.Requests.Active.Logic.Puni.ActiveHandler.HandleAsync(ctx);
        });
        app.MapGet("/l5id" + L5ID_BASE + "create_gdkey/", async ctx =>
        {
            await CreateGDKeyHandler.HandleAsync(ctx);
        });
        //For WibWob
        app.MapGet(L5ID_BASE + "active.nhn/", async ctx =>
        {
            await Puniemu.Src.Server.L5ID.Requests.Active.Logic.WibWob.ActiveHandler.HandleAsync(ctx);
        });
        app.MapGet(L5ID_BASE + "create_gdkey.nhn/", async ctx =>
        {
            await CreateGDKeyHandler.HandleAsync(ctx);
        });
    }

    static void AssignCustomAuthHandlers(WebApplication app)
    {
        app.MapPost("/auth/link", async ctx =>
        {
            await InitAccountActionHandler.HandleAsync(ctx, true);
        });
        app.MapPost("/auth/restore", async ctx =>
        {
            await InitAccountActionHandler.HandleAsync(ctx, false);
        });
    }


    static void AssignGameServerHandlers(WebApplication app)
    {
        //Used as a bootstrap for custom auth
        app.MapGet("/help/inquiry/top.nhn", async ctx =>
        {
            var html = await File.ReadAllTextAsync(
                Path.Combine(app.Environment.ContentRootPath, "dataDownload/help.html"));

            var userId = ctx.Request.Query["userId"].ToString();
            var appVer = ctx.Request.Query["appVer"].ToString();
            var sdkVer = ctx.Request.Query["sdkVer"].ToString();

            var inject = $$"""
        <script>
        window.__PARAMS__ = {
            userId: {{System.Text.Json.JsonSerializer.Serialize(userId)}},
            appVer: {{System.Text.Json.JsonSerializer.Serialize(appVer)}},
            sdkVer: {{System.Text.Json.JsonSerializer.Serialize(sdkVer)}}
        };
        </script>
        """;

            html = inject + html;

            ctx.Response.ContentType = "text/html; charset=utf-8";
            await ctx.Response.WriteAsync(html);
        });

        app.MapPost("/init.nhn", async ctx =>
        {
            await InitHandler.HandleAsync(ctx);
        });
        app.MapPost("/initWatch.nhn", async ctx =>
        {
            await InitWatchHandler.HandleAsync(ctx);
        });
        app.MapPost("/updateWatchReadFlg.nhn", async ctx =>
        {
            await UpdateWatchReadFlgHandler.HandleAsync(ctx);
        });
        app.MapPost("/serialConfirm.nhn", async ctx =>
        {
            await SerialConfirmHandler.HandleAsync(ctx);
        });
        app.MapPost("/getMaster.nhn", async ctx =>
        {
            await GetMasterHandler.HandleAsync(ctx);
        });
        app.MapPost("/ageConfirm.nhn", async ctx =>
        {
            await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(new Src.Server.GameServer.DataClasses.MsgBoxResponse("Puniemu does not support\npaid content.", "Support NHN"))));
        });
        app.MapPost("/createUser.nhn", async ctx =>
        {
            await CreateUserHandler.HandleAsync(ctx);
        });
        app.MapPost("/conflate.nhn", async ctx =>
        {
            await ConflateHandler.HandleAsync(ctx);
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
        app.MapPost("/missionReward.nhn", async ctx =>
        {
            await MissionRewardHandler.HandleAsync(ctx);
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
        app.MapPost("/levelLockOff.nhn", async ctx =>
        {
            await LevelLockOffHandler.HandleAsync(ctx);
        });
        app.MapPost("/buyHitodama.nhn", async ctx =>
        {
            await BuyHitodamaHandler.HandleAsync(ctx);
        });
        app.MapPost("/map.nhn", async ctx =>
        {
            await MapHandler.HandleAsync(ctx);
        });
        app.MapPost("/getMission.nhn", async ctx =>
        {
            await GetMissionHandler.HandleAsync(ctx, 0);
        });
        app.MapPost("/buyItem.nhn", async ctx =>
        {
            await BuyItemHandler.HandleAsync(ctx);
        });
        app.MapPost("/useItem.nhn", async ctx =>
        {
            await UseItemHandler.HandleAsync(ctx);
        });
        app.MapPost("/getLimitHitodama.nhn", async ctx =>
        {
            //an empty response works. seems to be unused, also a wibwob exclusive request.
            await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse("{}"));
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
            await GameEndHandler.HandleAsync(ctx, GameEndType.GameEnd);
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
            await GameEndHandler.HandleAsync(ctx, GameEndType.GameRetire);
        });
        app.MapPost("/gameContinue.nhn", async ctx =>
        {
            await GameContinueHandler.HandleAsync(ctx);
        });
        app.MapPost("/loginStamp.nhn", async ctx =>
        {
            await LoginStampHandler.HandleAsync(ctx);
        });
        app.MapPost("/initGacha.nhn", async ctx =>
        {
            if(DataManager.Logic.DataManager.IsWibWob) await Src.Server.GameServer.Requests.Init.InitGacha.Logic.WibWob.InitGachaHandler.HandleAsync(ctx);
            else await Src.Server.GameServer.Requests.Init.InitGacha.Logic.Puni.InitGachaHandler.HandleAsync(ctx);
        });
        app.MapPost("/executeGacha.nhn", async ctx =>
        {
            await ExecuteGachaHandler.HandleAsync(ctx);
        });
        app.MapPost("/gacha.nhn", async ctx =>
        {
            await ExecuteGachaHandler.HandleAsync(ctx);
        });
        app.MapPost("/friend.nhn", async ctx =>
        {
            await FriendsHandler.HandleAsync(ctx);
        });
        app.MapPost("/initCollectMenu.nhn", async ctx =>
        {
            await InitCollectMenuHandler.HandleAsync(ctx);
        });
        app.MapPost("/useAddition.nhn", async ctx =>
        {
            await UseAdditionHandler.HandleAsync(ctx);
        });
        app.MapPost("/initGoku.nhn", async ctx =>
        {
            await InitGokuHandler.HandleAsync(ctx);
        });
        app.MapPost("/updateGokuStory.nhn", async ctx =>
        {
            await UpdateGokuStoryHandler.HandleAsync(ctx);
        });
        app.MapPost("/friendSearch.nhn", async ctx =>
        {
            await FriendSearchHandler.HandleAsync(ctx);
        });
        app.MapPost("/friendRequest.nhn", async ctx =>
        {
            await FriendRequestHandler.HandleAsync(ctx);
        });
        app.MapPost("/friendRequestDelete.nhn", async ctx =>
        {
            await FriendRequestDeleteHandler.HandleAsync(ctx);
        });
        app.MapPost("/friendRequestAccept.nhn", async ctx =>
        {
            await FriendRequestAcceptHandler.HandleAsync(ctx);
        });
        app.MapPost("/initCrystal.nhn", async ctx =>
        {
            await InitCrystalHandler.HandleAsync(ctx);
        });
        app.MapPost("/updateGokuMenu.nhn", async ctx =>
        {
            await UpdateGokuMenuHandler.HandleAsync(ctx);
        });
        app.MapPost("/updateCrystalMenu.nhn", async ctx =>
        {
            await UpdateCrystalMenuHandler.HandleAsync(ctx);
        });
        app.MapPost("/friendDelete.nhn", async ctx =>
        {
            await FriendDeleteHandler.HandleAsync(ctx);
        });
        app.MapPost("/getPresentBox.nhn", async ctx =>
        {
            await GetPresentBoxHandler.HandleAsync(ctx);
        });
        app.MapPost("/getRanking.nhn", async ctx =>
        {
            await GetRankingHandler.HandleAsync(ctx);
        });
        app.MapPost("/mapWarp.nhn", async ctx =>
        {
            await MapWarpHandler.HandleAsync(ctx);
        });
        app.MapPost("/gameStartScoreAttack.nhn", async ctx =>
        {
            await StartScoreAttackHandler.HandleAsync(ctx);
        });
        app.MapPost("/gameEndScoreAttack.nhn", async ctx =>
        {
            await GameEndScoreAttackHandler.HandleAsync(ctx);
        });
        app.MapPost("/initScoreAttack.nhn", async ctx =>
        {
            await InitScoreAttackHandler.HandleAsync(ctx);
        });
        app.MapPost("/mapUnLock.nhn", async ctx =>
        {
            await MapUnLockHandler.HandleAsync(ctx);
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
