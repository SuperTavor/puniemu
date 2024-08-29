using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.CreateUser.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;
using System.Buffers;

namespace Puniemu.Src.Server.GameServer.Requests.CreateUser.Logic
{
    public class CreateUserHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<CreateUserRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";
            var generatedUserData = new YwpUserData((PlayerIcon)deserialized.IconID, (PlayerTitle)deserialized.IconID, deserialized.Level5UserID, deserialized.PlayerName);
            var createUserResponse = new CreateUserResponse(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_user_tutorial_list_def"], generatedUserData);
            var marshalledResponse = JsonConvert.SerializeObject(createUserResponse);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
            //Register the default tables on another thread and return from the handler, so createUser doesn't take too much time
            _ = Task.Run(() => RegisterDefaultTables(deserialized, generatedUserData));
            
        }

        private static async Task RegisterDefaultTables(CreateUserRequest deserialized,YwpUserData generatedUserData)
        {
            //The icon ID is provided as also the title ID, as that's the only way to generate the first title ID, as the ids match
<<<<<<< Updated upstream
            var generatedUserData = new YwpUserData((PlayerIcon)deserialized.IconID, (PlayerTitle)deserialized.IconID, deserialized.Level5UserID, deserialized.PlayerName);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_data", generatedUserData);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_tutorial_list", ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_user_tutorial_list_def"]);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_player_icon", Consts.DEFAULT_OBTAINED_ICONS_AND_TITLES);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_player_title", Consts.DEFAULT_OBTAINED_ICONS_AND_TITLES);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "start_date", DateTimeOffset.Now.ToUnixTimeMilliseconds());
            
            var createUserResponse = new CreateUserResponse(ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_user_tutorial_list_def"], generatedUserData);
            var marshalledResponse = JsonConvert.SerializeObject(createUserResponse);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
=======
            string ywp_user_def_table = "ywp_user_youkai|ywp_user_tutorial_list|ywp_user_player_icon|ywp_user_event|ywp_user_hist_youkai_daily|ywp_user_hist_puzzle_weekly|ywp_user_map|ywp_user_youkai_medal_cnt|ywp_user_youkai_skill|ywp_user_icon_budge|ywp_user_shop_item_remain_cnt|ywp_user_player_title|ywp_user_hist_youkai_total|ywp_user_menufunc|ywp_user_dictionary|ywp_user_hist_total|ywp_user_hist_puzzle_daily|ywp_user_stage|ywp_user_youkai_deck";
            string[] table_def = ywp_user_def_table.Split('|');
            string ywp_user_def_list = "ywp_user_event_condition|ywp_user_ads_play|ywp_user_watch";
            string[] list_def = ywp_user_def_list.Split('|');
            string ywp_user_def_dict = "ywp_user_score_attack_point_trade";
            string[] dict_def = ywp_user_def_dict.Split('|');
            string ywp_user_empty_table = "ywp_user_conflate|ywp_user_treasure_series|ywp_user_youkai_legend_release_history|ywp_user_youkai_bonus_effect|ywp_user_item|ywp_user_youkai_strong_skill|ywp_user_shop_item_unlock|ywp_user_treasure";
            string[] empty_table = ywp_user_empty_table.Split('|');
            string ywp_user_empty_list = "ywp_user_medal_point_trade|ywp_user_stage_rank|ywp_user_raid_boss|ywp_user_steal_progress|ywp_user_mini_game_map|ywp_user_event_point_trade|ywp_user_friend_stage|ywp_user_drive_progress|ywp_user_mini_game_map_friend|ywp_user_stage_relation_progress|ywp_user_score_attack_reward|ywp_user_event_tutorial|ywp_user_event_ranking_reward";
            string[] empty_list = ywp_user_empty_list.Split('|');
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_data", generatedUserData);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "do_opening_tuto", 1);
            foreach (string mot in empty_table)
            {
                await UserDataManager.Logic.UserDataManager.SetYwpUserAsync<string>(deserialized.Level5UserID, mot, "");
            }
            foreach (string mot in empty_list)
            {
                await UserDataManager.Logic.UserDataManager.SetYwpUserAsync<List<object>>(deserialized.Level5UserID, mot, new List<object>());
            }
            foreach (string mot in table_def)
            {
                await UserDataManager.Logic.UserDataManager.SetYwpUserAsync<string>(deserialized.Level5UserID, mot, ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache[mot + "_def"]);
            }
            foreach (string mot in dict_def)
            {
                await UserDataManager.Logic.UserDataManager.SetYwpUserAsync<Dictionary<string, object>>(deserialized.Level5UserID, mot, GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<Dictionary<string, object>>(mot + "_def"));
            }
            foreach (string mot in list_def)
            {
                await UserDataManager.Logic.UserDataManager.SetYwpUserAsync<List<object>>(deserialized.Level5UserID, mot, GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<object>>(mot + "_def"));
            }
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "start_date", DateTimeOffset.Now.ToUnixTimeSeconds());
>>>>>>> Stashed changes
        }
    }
}
