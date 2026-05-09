using Newtonsoft.Json;
using Puniemu.Src.DataManager.Logic;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Requests.CreateUser.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using Puniemu.Src.DBService.DataClasses;
using Puniemu.Src.DBService.Logic;
using Supabase.Postgrest;
using Supabase.Postgrest.Attributes;
using System.Buffers;
using System.Reflection;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Requests.CreateUser.Logic
{
    public class CreateUserHandler
    {

        public static (string Yokai,string Skill) CreateUserYoukaiSave()
        {
            TableParser<YwpUserYoukai> value = new("");
            TableParser<YwpUserYoukaiSkill> value2 = new("");

            YoukaiManager.AddYoukai(ref value, 2157000, ref value2);
            YoukaiManager.AddYoukai(ref value, 2213000, ref value2);
            YoukaiManager.AddYoukai(ref value, 2231000, ref value2);
            YoukaiManager.AddYoukai(ref value, 2235000, ref value2);
            YoukaiManager.AddYoukai(ref value, 2281000, ref value2);

            return (value.ToString(), value2.ToString());
        }
        public static async Task AddTables(Account acc, YwpUserData generatedUserData)
        {
            acc.YwpUserData = generatedUserData;

            
            acc.SetFieldByJsonName("ywp_user_youkai_collect", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_youkai_intro", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_goku_youkai_intro_release", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_goku_story", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_friend_request_recv", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_friend", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_present_box_list", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_crystal_menu", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_drive_progress", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_event_point", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_event_point_trade", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_event_ranking_reward", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_event_tutorial", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_friend_stage", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_gacha", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_medal_point_trade", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_mini_game_map", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_mini_game_map_friend", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_raid_boss", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_score_attack_reward", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_stage_rank", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_stage_relation_progress", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_steal_progress", new List<object> { });
            acc.SetFieldByJsonName("ywp_user_league_rank", null);

            // string
            acc.SetFieldByJsonName("ywp_user_gacha_stamp", null);
            acc.SetFieldByJsonName("ywp_user_youkai_strong_skill", null);
            acc.SetFieldByJsonName("ywp_user_youkai_legend_release_history", null);
            acc.SetFieldByJsonName("ywp_user_youkai_bonus_effect", null);
            acc.SetFieldByJsonName("ywp_user_treasure_series", null);
            acc.SetFieldByJsonName("ywp_user_treasure", null);
            acc.SetFieldByJsonName("ywp_user_shop_item_unlock", null);
            acc.SetFieldByJsonName("ywp_user_item", null);
            acc.SetFieldByJsonName("ywp_user_event_progress", null);
            acc.SetFieldByJsonName("ywp_user_conflate", null);
            acc.SetFieldByJsonName("ywp_user_youkai_collect", new List<object> {});
            acc.SetFieldByJsonName("ywp_user_youkai_intro", new List<object> {});
            acc.SetFieldByJsonName("ywp_user_goku_youkai_intro_release", new List<object> {});
            acc.SetFieldByJsonName("ywp_user_goku_story", new List<object> {});
            acc.SetFieldByJsonName("ywp_user_crystal_menu", new List<object> {});
            acc.SetFieldByJsonName("ywp_user_event_point", new List<object> {});
            acc.SetFieldByJsonName("ywp_user_mini_game_map", new List<object> {});

            acc.SetFieldByJsonName("ywp_user_mini_game_map_friend", new List<object> {});
            acc.SetFieldByJsonName("ywp_user_stage_relation_progress", new List<object> {});
            acc.SetFieldByJsonName("ywp_user_league_rank", null);
            acc.SetFieldByJsonName("ywp_user_gacha_stamp", null);
            acc.SetFieldByJsonName("ywp_user_youkai_strong_skill", null);
            acc.SetFieldByJsonName("ywp_user_youkai_legend_release_history", null);
            acc.SetFieldByJsonName("ywp_user_youkai_bonus_effect", null);

            // tables

            var selfRank = new FriendRankEntry
            {
                IconId = generatedUserData.IconID,
                PlayerName = generatedUserData.PlayerName,
                TitleId = generatedUserData.CharacterTitleID,
                GetStar = 0,
                UserId = generatedUserData.UserID,
                DicCnt = 0,
                Score = 0,
                YoukaiId = generatedUserData.YoukaiId,
                GetStarModiDt = null,
                HitodamaSendFlg = 1,
                OnedariSendFlg = 1,
                Rank = 1,
                Self = 1,
            };
            acc.SetFieldByJsonName("ywp_user_friend_star_rank", new List<FriendRankEntry> { selfRank });
            acc.SetFieldByJsonName("ywp_user_friend_rank", new List<FriendRankEntry> { selfRank });
            acc.SetFieldByJsonName("ywp_user_friend_dictionary_rank", new List<FriendRankEntry> { selfRank });
            acc.SetFieldByJsonName("ywp_user_self_rank", new SelfRank(generatedUserData));
            acc.SetFieldByJsonName("login_stamp", "0|0|0");
            acc.SetFieldByJsonName("ywp_user_youkai_medal_cnt", "0");
            //New puni profile app fields, will do later

            //acc.SetFieldByJsonName("ywp_user_player_plate", "1");
            //acc.SetFieldByJsonName("ywp_user_player_effect", "1");
            //acc.SetFieldByJsonName("ywp_user_player_codename", "1");

            (string,string) youkai = CreateUserYoukaiSave();
            acc.SetFieldByJsonName("ywp_user_youkai", youkai.Item1);
            acc.SetFieldByJsonName("ywp_user_youkai_skill", youkai.Item2);

        }
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<CreateUserRequest>(requestJsonString!);
            var dbres = await DBService.Logic.DBService.SupabaseClient!.From<Account>().Where(x => x.Gdkey == deserialized.Level5UserID).Get();
            var acc = dbres.Model!;
            ctx.Response.ContentType = "application/json";
            var generatedUserData = new YwpUserData((PlayerIcon)deserialized.IconID, (PlayerTitle)deserialized.IconID, deserialized.Level5UserID, deserialized.PlayerName);
            acc.StartDate = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            acc.CharacterId = generatedUserData.CharacterID;
            acc.UserId = generatedUserData.UserID;
            await acc.Update<Account>();
            try
            {
                await RegisterDefaultTables(deserialized, generatedUserData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ctx.Response.StatusCode = 500;
                await ctx.Response.WriteAsync("Internal server error");
                return;
            }
            var createUserResponse = new CreateUserResponse(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_user_tutorial_list_def"], generatedUserData);
            var marshalledResponse = JsonConvert.SerializeObject(createUserResponse);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);            
            
        }

        private static async Task RegisterDefaultTables(CreateUserRequest deserialized,YwpUserData generatedUserData)
        {
            var acc = await DBService.Logic.DBService.GetAccountObjectAsync(deserialized.Level5UserID);
            await AddTables(acc, generatedUserData);

            acc.OpeningTutorialFlag = false;
            foreach (var userTable in Consts.LOGIN_TABLES_PUNI.Where(x => x.Contains("ywp_user") && x != "ywp_user_data"))
            {

                if (DataManager.Logic.DataManager.GameDataManager!.GamedataCache.TryGetValue(userTable+"_def", out var data))
                {
                    acc.SetFieldByJsonName(userTable,data);
                }
                else
                {
                    throw new Exception();
                }
            }
            await DBService.Logic.DBService.SetAccountObjectAsync(deserialized.Level5UserID, acc);
        }
    }
}
