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
        public static async Task AddDynamicDefaults(Account acc, YwpUserData generatedUserData)
        {
            acc.YwpUserData = generatedUserData;
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
            await AddDynamicDefaults(acc, generatedUserData);

            acc.OpeningTutorialFlag = false;
            foreach (var userTable in Consts.LOGIN_TABLES_PUNI.Where(x => (x.Contains("ywp_user") || (x.Contains("login_stamp") && !x.Contains("ywp_mst"))) && x != "ywp_user_data"))
            {
                if (DataManager.Logic.DataManager.GameDataManager!.GamedataCache.TryGetValue(userTable+"_def", out var data))
                {
                    acc.SetFieldByJsonName(userTable,data);
                }
                else
                {
                    continue;
                }
            }
            await DBService.Logic.DBService.SetAccountObjectAsync(deserialized.Level5UserID, acc);
        }
    }
}
