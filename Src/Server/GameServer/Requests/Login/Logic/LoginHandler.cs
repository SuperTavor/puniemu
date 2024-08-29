using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.Login.DataClasses;
using Puniemu.Src.UserDataManager.Logic;
using Puniemu.Src.Utils.GeneralUtils;
using System.Buffers;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Requests.Login.Logic
{
    public static class LoginHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<LoginRequest>(requestJsonString!);
            //Construct response
            var res = new LoginResponse()
            {
<<<<<<< Updated upstream
=======
                ResultType = 0,
                UserData = userdata,

>>>>>>> Stashed changes
                ServerDate = DateTimeOffset.Now.ToUnixTimeMilliseconds(),

                YwpUserMedalPointTrade = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<object>>(deserialized.Gdkey, "ywp_user_medal_point_trade"),

                LeaderYoukaiBgm = ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["leaderYoukaiBGM"],

                YwpUserConflate = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_conflate"),

                YwpUserTreasureSeries = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_treasure_series"),

                YwpUserYoukai = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_youkai"),

                YwpUserStageRank = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<object>>(deserialized.Gdkey, "ywp_user_stage_rank"),

                YwpUserYoukaiLegendReleaseHistory = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_youkai_legend_release_history"),

                OpeningTutorialFlag = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<int>(deserialized.Gdkey, "do_opening_tuto"),

                YwpTutorialList = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_tutorial_list"),

                YwpUserRaidBoss = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<object>>(deserialized.Gdkey, "ywp_user_raid_boss"),

                YwpUserEventCond = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<object>>(deserialized.Gdkey, "ywp_user_event_condition"),

                YwpUserItem = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_item"),

                YwpMstEventTxt = GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<Dictionary<string, object>>("ywp_mst_event_tutorial_message"),

                YwpUserShopItemUnlock = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_shop_item_unlock"),

                YwpUserPlayerTitle = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_player_title"),

                YwpUserAdsPlay = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<object>>(deserialized.Gdkey, "ywp_user_ads_play"),

                NoticePageListFlag = int.Parse(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["noticePageListFlg"]),

                MstVersionMaster = int.Parse(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["mstVersionMaster"]),

                YwpUserFriendStage = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<object>>(deserialized.Gdkey, "ywp_user_friend_stage"),

                ResultCode = 0,

                NextScreenType = 0,

                MonthlyPurchasableLeft = 0,

                YwpMstVersionMaster = ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_mst_version_master"],

                YwpUserDriveProgress = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<object>>(deserialized.Gdkey, "ywp_user_drive_progress"),

                HitodamaShopSaleList = GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<int>>("hitodamaShopSaleList"),

                YwpMstYoukaiBonusEffectExclude = GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<object>>("ywp_mst_youkai_bonus_effect_exclude"),

                YwpUserHistYoukaiTotal = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_hist_youkai_total"),

                YwpUserMiniGameMapFriend = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<object>>(deserialized.Gdkey, "ywp_user_mini_game_map_friend"),

                YwpUserMenuFunc = ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_user_menufunc"],

                YwpUserDict = ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_user_dictionary"],

                YwpUserEventGroupAssistDisp = ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_mst_event_group_assist_disp"],

                YwpUserHistTotal = ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_user_hist_total"],

                YwpUserHistPuzzleDaily = ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_user_hist_puzzle_daily"],

                YMoneyShopSaleList = GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<object>>("ymoneyShopSaleList"),

                YwpToken = "",

                YwpUserStageRelationProgress = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<object>>(deserialized.Gdkey, "ywp_user_stage_relation_progress"),

                YwpMstEvent = GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<object>>("ywp_mst_event"),

                Token = "",

                YwpMstEventYoukaiAssistDisp = ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_mst_event_youkai_assist_disp"],

                YwpUserStage = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Gdkey, "ywp_user_stage"),

                DialogMsg = "",

                YwpUserScoreAttackReward = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<object>>(deserialized.Gdkey, "ywp_user_score_attack_reward"),

                YwpMstEventCondition = ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_mst_event_condition"],

                YwpUserYoukaiDeck = ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_user_youkai_deck"],

                YwpUserScoreAttackPointTrade = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<Dictionary<string, object>>(deserialized.Gdkey, "ywp_user_score_attack_point_trade"),

                YwpUserWatch = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<Dictionary<string, object>>>(deserialized.Gdkey, "ywp_user_watch"),

                YwpUserEventTutorial = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<object>>(deserialized.Gdkey, "ywp_user_event_tutorial"),

                YwpUserEventRankingReward = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<List<object>>(deserialized.Gdkey, "ywp_user_event_ranking_reward")
            };
<<<<<<< Updated upstream
=======
            //await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Gdkey, "do_opening_tuto", 1);
            var marshalledResponse = JsonConvert.SerializeObject(res);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
>>>>>>> Stashed changes

        }
    }
}
