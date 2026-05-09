using Newtonsoft.Json;
using Puniemu.Src.DataManager.Logic;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Requests.CreateUser.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using Puniemu.Src.UserDataManager.DataClasses;
using Puniemu.Src.UserDataManager.Logic;
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
        public static void AddTables(Account acc, YwpUserData generatedUserData)
        {
            acc.YwpUserData = generatedUserData;


            // empty tables
            acc.YwpUserFriendRequestRecv = "[]";
            acc.YwpUserFriend = "[]";
            acc.YwpUserPresentBoxList = "[]";
            acc.YwpUserDriveProgress = "[]";
            acc.YwpUserEventPointTrade = "[]";
            acc.YwpUserEventRankingReward = "[]";
            acc.YwpUserEventTutorial = "[]";
            acc.YwpUserFriendStage = "[]";
            acc.YwpUserGacha = "[]";
            acc.YwpUserMedalPointTrade = "[]";
            acc.YwpUserRaidBoss = "[]";
            acc.YwpUserScoreAttackReward = "[]";
            acc.YwpUserStageRank = "[]";
            acc.YwpUserStealProgress = "[]";
            acc.YwpUserTreasureSeries = "";
            acc.YwpUserTreasure = "";
            acc.YwpUserShopItemUnlock = "";
            acc.YwpUserItem = "";
            acc.YwpUserEventProgress = "";
            acc.YwpUserConflate = "";
            //Puni exclusilve tables
            
            //tables.Add("ywp_user_youkai_collect", new List<object> {});
            //tables.Add("ywp_user_youkai_intro", new List<object> {});
            //tables.Add("ywp_user_goku_youkai_intro_release", new List<object> {});
            //tables.Add("ywp_user_goku_story", new List<object> {});
            //tables.Add("ywp_user_crystal_menu", new List<object> {});
            //tables.Add("ywp_user_event_point", new List<object> {});
            //tables.Add("ywp_user_mini_game_map", new List<object> {});
            //tables.Add("ywp_user_mini_game_map_friend", new List<object> {});
            //tables.Add("ywp_user_stage_relation_progress", new List<object> {});
            //tables.Add("ywp_user_league_rank", null);
            //tables.Add("ywp_user_gacha_stamp", null);
            //tables.Add("ywp_user_youkai_strong_skill", null);
            //tables.Add("ywp_user_youkai_legend_release_history", null);
            //tables.Add("ywp_user_youkai_bonus_effect", null);
            //tables.Add("ywp_user_player_plate", "1");
            //tables.Add("ywp_user_player_effect", "1");
            //tables.Add("ywp_user_player_codename", "1");

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
            acc.YwpUserFriendStarRank = JsonConvert.SerializeObject(new List<FriendRankEntry> { selfRank });
            acc.YwpUserFriendRank = JsonConvert.SerializeObject(new List<FriendRankEntry> { selfRank });
            acc.YwpUserFriendDictionaryRank = JsonConvert.SerializeObject(new List<FriendRankEntry> { selfRank });
            acc.YwpUserSelfRank = JsonConvert.SerializeObject(new SelfRank(generatedUserData));
            acc.LoginStamp = JsonConvert.SerializeObject("0|0|0");
            acc.YwpUserYoukaiMedalCnt = JsonConvert.SerializeObject(0);

            (string Yokai ,string Skill) youkai = CreateUserYoukaiSave();
            acc.YwpUserYoukai = youkai.Yokai;
            acc.YwpUserYoukaiSkill = youkai.Skill;

        }
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<CreateUserRequest>(requestJsonString!);
            var dbres = await UserDataManager.Logic.DBService.SupabaseClient!.From<Account>().Where(x => x.Gdkey == deserialized.Level5UserID).Get();
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
            var acc = await DBService.GetAccountObjectAsync(deserialized.Level5UserID);
            AddTables(acc, generatedUserData);

            acc.OpeningTutorialFlag = false;
            foreach (var userTable in Consts.LOGIN_TABLES_PUNI.Where(x => x.Contains("ywp_user") && x != "ywp_user_data"))
            {
                var prop = typeof(Account)
                    .GetProperties()
                    .FirstOrDefault(p =>
                    {
                        var attr = p.GetCustomAttribute<ColumnAttribute>();
                        return attr != null && attr.ColumnName == userTable;
                    });

                if (prop == null)
                    //table doesnt exist
                    continue;

                if (DataManager.Logic.DataManager.GameDataManager!.GamedataCache.TryGetValue(userTable+"_def", out var data))
                {
                    prop.SetValue(acc,data);
                }
                else
                {
                    throw new Exception();
                }
            }
            await DBService.SetAccountObjectAsync(deserialized.Level5UserID, acc);
        }
    }
}
