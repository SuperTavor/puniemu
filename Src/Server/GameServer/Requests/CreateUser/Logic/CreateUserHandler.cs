using Newtonsoft.Json;
using Puniemu.Src.DataManager.Logic;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.CreateUser.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.UserDataManager.DataClasses;
using Puniemu.Src.UserDataManager.Logic;
using Puniemu.Src.TableParser.Logic;
using Supabase.Postgrest;
using System.Buffers;
using System.Text;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Requests.Init.InitGacha.DataClasses.WibWob;

namespace Puniemu.Src.Server.GameServer.Requests.CreateUser.Logic
{
    public class CreateUserHandler
    {

        public static Tuple<string, string> CreateUserYoukaiSave()
        {
            TableParser<YwpUserYoukai> value = new("");
            TableParser<YwpUserYoukaiSkill> value2 = new("");

            YoukaiManager.AddYoukai(value, 2157000, value2);
            YoukaiManager.AddYoukai(value, 2213000, value2);
            YoukaiManager.AddYoukai(value, 2231000, value2);
            YoukaiManager.AddYoukai(value, 2235000, value2);
            YoukaiManager.AddYoukai(value, 2281000, value2);

            return new Tuple<string, string> (value.ToString(), value2.ToString());
        }
        public static void CreateSave(Dictionary<string, object?> tables, YwpUserData generatedUserData)
        {
            tables.Add("ywp_user_data", generatedUserData);



            // empty tables

            tables.Add("ywp_user_youkai_collect", new List<object> {});
            tables.Add("ywp_user_youkai_intro", new List<object> {});
            tables.Add("ywp_user_goku_youkai_intro_release", new List<object> {});
            tables.Add("ywp_user_goku_story", new List<object> {});
            tables.Add("ywp_user_friend_request_recv", new List<object> {});
            tables.Add("ywp_user_friend", new List<object> {});
            tables.Add("ywp_user_present_box_list", new List<object> {});
            tables.Add("ywp_user_crystal_menu", new List<object> {});
            tables.Add("ywp_user_drive_progress", new List<object> {});
            tables.Add("ywp_user_event_point", new List<object> {});
            tables.Add("ywp_user_event_point_trade", new List<object> {});
            tables.Add("ywp_user_event_ranking_reward", new List<object> {});
            tables.Add("ywp_user_event_tutorial", new List<object> {});
            tables.Add("ywp_user_friend_stage", new List<object> {});
            if (DataManager.Logic.DataManager.IsWibWob) tables.Add("ywp_user_gacha", new List<WibWobUserGachaEntry> { new WibWobUserGachaEntry { GachaType = 3, FeverPctg = 0 } });
            else tables.Add("ywp_user_gacha", new List<object> {});
            tables.Add("ywp_user_medal_point_trade", new List<object> {});
            tables.Add("ywp_user_mini_game_map", new List<object> {});
            tables.Add("ywp_user_mini_game_map_friend", new List<object> {});
            tables.Add("ywp_user_raid_boss", new List<object> {});
            tables.Add("ywp_user_score_attack_reward", new List<object> {});
            tables.Add("ywp_user_stage_rank", new List<object> {});
            tables.Add("ywp_user_stage_relation_progress", new List<object> {});
            tables.Add("ywp_user_steal_progress", new List<object> {});
            tables.Add("ywp_user_league_rank", null);

            // string
            tables.Add("ywp_user_gacha_stamp", null);
            tables.Add("ywp_user_youkai_strong_skill", null);
            tables.Add("ywp_user_youkai_legend_release_history", null);
            tables.Add("ywp_user_youkai_bonus_effect", null);
            tables.Add("ywp_user_treasure_series", null);
            tables.Add("ywp_user_treasure", null);
            tables.Add("ywp_user_shop_item_unlock", null);
            tables.Add("ywp_user_item", null);
            tables.Add("ywp_user_event_progress", null);
            tables.Add("ywp_user_conflate", null);


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
            tables.Add("ywp_user_friend_star_rank", new List<FriendRankEntry> {selfRank});
            tables.Add("ywp_user_friend_rank", new List<FriendRankEntry> {selfRank});
            tables.Add("ywp_user_friend_dictionary_rank", new List<FriendRankEntry> {selfRank});
            tables.Add("ywp_user_self_rank", new SelfRank(generatedUserData));
            tables.Add("login_stamp", "0|0|0");
            tables.Add("ywp_user_youkai_medal_cnt", "0");
            tables.Add("ywp_user_player_plate", "1");
            tables.Add("ywp_user_player_effect", "1");
            tables.Add("ywp_user_player_codename", "1");

            Tuple<string, string> youkai = CreateUserYoukaiSave();
            tables.Add("ywp_user_youkai", youkai.Item1);
            tables.Add("ywp_user_youkai_skill", youkai.Item2);

        }
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<CreateUserRequest>(requestJsonString!);
            var dbres = await UserDataManager.Logic.UserDataManager.SupabaseClient!.From<Account>().Where(x => x.Gdkey == deserialized.Level5UserID).Get();
            var acc = dbres.Model!;
            ctx.Response.ContentType = "application/json";
            // in wibwob the title is not gender specific in puni yes
            var title = PlayerTitle.Kun_Little;
            if (!DataManager.Logic.DataManager.IsWibWob) title = (PlayerTitle)deserialized.IconID;
            var generatedUserData = new YwpUserData((PlayerIcon)deserialized.IconID, title, deserialized.Level5UserID, deserialized.PlayerName);
            acc.StartDate = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            generatedUserData.CharacterID = acc.CharacterId;
            generatedUserData.UserID = System.IO.Hashing.Crc32.HashToUInt32(System.Text.Encoding.UTF8.GetBytes(generatedUserData.CharacterID)).ToString();
            acc.UserId = generatedUserData.UserID;
            acc.IsDirty = true;
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
            Dictionary<string,object?> tables = new Dictionary<string,object?>();
            CreateSave(tables, generatedUserData);

            tables["opening_tutorial_flg"] = false;
            foreach (var userTable in Consts.LOGIN_TABLES_PUNI.Where(x => x.Contains("ywp_user") && x != "ywp_user_data"))
            {
                //initialize with default if exists, else 
                if (tables.ContainsKey(userTable) == true)
                {
                    continue;
                }
                if (DataManager.Logic.DataManager.GameDataManager!.GamedataCache.TryGetValue(userTable+"_def", out var data))
                {
                    object? deserializedDefaultUserTable = null!;
                    try
                    {
                        deserializedDefaultUserTable = JsonConvert.DeserializeObject<object>(data);
                    }
                    catch
                    {
                        deserializedDefaultUserTable = data;
                    }

                    tables.Add(userTable, deserializedDefaultUserTable!);
                }
                else
                {
                    throw new Exception();
                }
            }
            await UserDataManager.Logic.UserDataManager.SetEntireUserData(deserialized.Level5UserID, tables!);
        }
    }
}
