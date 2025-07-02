using Newtonsoft.Json;
using Puniemu.Src.UserDataManager;

using Puniemu.Src.Server.GameServer.Requests.LoginStamp.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;
using System.Buffers;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.Server.GameServer.Logic;

namespace Puniemu.Src.Server.GameServer.Requests.LoginStamp.Logic
{
    public class LoginStampHandler
    {
        //randomly choose stamp for avoiding need to edit it manually
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<LoginStampRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";
            var userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID, "ywp_user_data");
            List<string> LOGIN_STAMP_TABLES = new();

            var res = new LoginStampResponse();
            
            // used to define variable
            var playerIconTable = new TableParser.Logic.TableParser("");
            var itmesListTable = new TableParser.Logic.TableParser("");
            var userYoukaiTable = new TableParser.Logic.TableParser("");
            var dictionaryYoukaiTable = new TableParser.Logic.TableParser("");


            var LoginStamp = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager!.GamedataCache["ywp_mst_login_stamp"]!)!["tableData"]);
            var LoginStampReward = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager!.GamedataCache["ywp_mst_login_stamp_reward"]!)!["tableData"]);

            var LoginUserStamp = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserID!, "login_stamp");
            var LoginUserStampTable = new TableParser.Logic.TableParser(LoginUserStamp!);

            int LoginStamLenght = LoginStamp.Table.Count;
           
            // random when choosing new stamp
            Random random = new Random();

            // if walk == 1, it means the login stamp has been updated
            var walk = 0;

            // add new stamp is not defined already
            if (LoginUserStampTable.Table[0][0] == "0" || LoginUserStampTable.Table[0][1] == "0" || LoginUserStampTable.Table[0][2] == "0")
            {
                long daysSinceEpoch = (long)Math.Floor((DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 1.0));
                int number = random.Next(1, LoginStamLenght);
                LoginUserStampTable.Table[0][0] = daysSinceEpoch.ToString();
                LoginUserStampTable.Table[0][1] = number.ToString();
                LoginUserStampTable.Table[0][2] = "1";
                walk = 1;
            }
            int day = int.Parse(LoginUserStampTable.Table[0][2]);
            var StampId = int.Parse(LoginUserStampTable.Table[0][1]);

            long daysSinceEpoch2 = (long)Math.Floor((DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 1.0));

            // change stamp if stamp finished
            foreach (string[] elements in LoginStamp.Table)
            {
                if (int.Parse(elements[0]) == StampId && daysSinceEpoch2 > long.Parse(LoginUserStampTable.Table[0][0]) && (day + 1) > int.Parse(elements[2]))
                {
                    StampId = random.Next(1, LoginStamLenght);
                    walk = 1;
                    int number = random.Next(1, LoginStamLenght);
                    daysSinceEpoch2 = (long)Math.Floor((DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 1.0));
                    LoginUserStampTable.Table[0][0] = daysSinceEpoch2.ToString();
                    LoginUserStampTable.Table[0][1] = StampId.ToString();
                    LoginUserStampTable.Table[0][2] = "1";
                    day = int.Parse(LoginUserStampTable.Table[0][2]);
                }
            }

            // update day
            if (daysSinceEpoch2 > long.Parse(LoginUserStampTable.Table[0][0]))
            {
                walk = 1;
                LoginUserStampTable.Table[0][0] = daysSinceEpoch2.ToString();
                day += 1;
                LoginUserStampTable.Table[0][2] = day.ToString();
            }

            // set ywp_mst_login_stamp_reward
            var current_item_count = 0;
            var current_item_id = 0L;
            var current_item_type = 0;
            res.LoginStampReward = new();
            foreach (string[] elements in LoginStampReward.Table)
            {
                if (int.Parse(elements[0]) == StampId)
                {
                    LoginStampReward entry = new LoginStampReward();
                    entry.StampId = StampId;
                    entry.RewardDayCnt = int.Parse(elements[1]);
                    entry.RewardItemType = int.Parse(elements[2]);
                    entry.RewardItemId = int.Parse(elements[3]);
                    entry.RewardItemCnt = int.Parse(elements[4]);
                    if (entry.RewardDayCnt == day)
                    {
                        current_item_count = entry.RewardItemCnt;
                        current_item_id = entry.RewardItemId;
                        current_item_type = entry.RewardItemType;
                    }
                    res.LoginStampReward.Add(entry);
                }
            }

            // set ywp_mst_login_stamp
            res.LoginStampRes = new();
            foreach (string[] elements in LoginStamp.Table)
            {
                if (int.Parse(elements[0]) == StampId)
                {
                    LoginStampType entry = new LoginStampType();
                    entry.StampId = StampId;
                    if (elements[8] != "null")
                    {
                        entry.CautionResName = elements[8];
                    }
                    entry.FooterResName = elements[7];
                    entry.Description = elements[1];
                    entry.TitleResName = elements[5];
                    entry.MainResName = elements[4];
                    entry.HeaderResName = elements[6];
                    res.LoginStampRes.Add(entry);
                }
            }

            // add won youkai, money, item... (in the future we might create a signle function to add item/youkai for the whole server)
            if (walk == 1)
            {
                if (current_item_type == 1)
                {
                    var itemsList = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserID!, "ywp_user_item");
                    itmesListTable = ItemManager.AddItem(new TableParser.Logic.TableParser(itemsList!), current_item_id, current_item_count);
                    res.ItemPopupResult = new();
                    res.ItemPopupResult.IsLimitOver = 0;
                    res.ItemPopupResult.Count = current_item_count;
                    res.ItemPopupResult.ItemId = current_item_id;
                    await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserID!, "ywp_user_item", itmesListTable.ToString());
                    LOGIN_STAMP_TABLES.Add("ywp_user_item");
                }
                if (current_item_type == 2)
                {
                    var dictionaryYoukai = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserID!, "ywp_user_dictionary");
                    var userYoukai = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserID!, "ywp_user_youkai");

                    dictionaryYoukaiTable = DictionaryManager.EditDictionary(new TableParser.Logic.TableParser(dictionaryYoukai!), current_item_id, false, true);
                    userYoukaiTable = YoukaiManager.AddYoukai(new TableParser.Logic.TableParser(userYoukai!), current_item_id);


                    res.YoukaiPopupResult = new();
                    res.YoukaiPopupResult.IsWBonusEffectOpen = false; //IDK : TODO
                    res.YoukaiPopupResult.BonusEffectLevelBefore = 0; //IDK Todo
                    res.YoukaiPopupResult.StrongSkillLevelAfter = 0; //IDK Todo
                    res.YoukaiPopupResult.BonusEffectLevelAfter = 0; //IDK Todo
                    res.YoukaiPopupResult.StrongSkillLevelBefore = 0; //IDK Todo
                    res.YoukaiPopupResult.LegendYoukaiId = 0; // Legendary youkai flg Todo
                    res.YoukaiPopupResult.LevelBefore = 1; //level Todo
                    res.YoukaiPopupResult.LevelAfter = 1; //level todo
                    res.YoukaiPopupResult.GetTypes = 10; //IDK Todo
                    res.YoukaiPopupResult.YoukaiId = current_item_id;
                    res.YoukaiPopupResult.ReleaseType = 0; //IDK todo
                    // skill data is null in the response for now so : IDK | TODO
                    res.YoukaiPopupResult.ExchgYmoney = 0; //IDK TODO
                    res.YoukaiPopupResult.LimitLevelAfter = 0; //IDK todo
                    res.YoukaiPopupResult.LimitLevelBefore = 0; //IDK todo
                    res.YoukaiPopupResult.ReleaseLevelType = 0; //IDK TODO
                    await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserID!, "ywp_user_dictionary", dictionaryYoukaiTable.ToString());
                    await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserID!, "ywp_user_youkai", userYoukaiTable.ToString());
                    LOGIN_STAMP_TABLES.Add("ywp_user_youkai");
                    LOGIN_STAMP_TABLES.Add("ywp_user_dictionary");
                }
                if (current_item_type == 3)
                {
                    userData!.YMoney += current_item_count;
                }
                if (current_item_type == 4)
                {
                    userData!.Hitodama += current_item_count;
                }
                if (current_item_type == 12)
                {
                    var playerIcon = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Level5UserID!, "ywp_user_player_icon");
                    playerIconTable = PlayerIconManager.AddIcon(new TableParser.Logic.TableParser(playerIcon!), (int)current_item_id);
                    
                    await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserID!, "ywp_user_player_icon", playerIconTable.ToString());
                    LOGIN_STAMP_TABLES.Add("ywp_user_player_icon");
                }
            }
            res.UserLoginStamp = new();
            UserLoginStamp entry_res = new();
            entry_res.UserId = userData!.UserID;
            entry_res.StampId = StampId;
            entry_res.LoginDayCnt = day;
            entry_res.IsStep = walk;
            res.UserLoginStamp.Add(entry_res);
            res.StampDt = DateTime.Today.ToString("yyyy-MM-dd");


            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_data", userData);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Level5UserID!, "login_stamp", LoginUserStampTable.ToString());
            res.UserData = userData;
            var resdict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(res));

            // actually it's not const but we use this to dont send youkai, item, dictionary... tables if not modified
            await GeneralUtils.AddTablesToResponse(LOGIN_STAMP_TABLES, resdict!, true, deserialized!.Level5UserID!);
            var encryptedRes = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(resdict));
            ctx.Response.Headers.ContentType = "application/json";
            await ctx.Response.WriteAsync(encryptedRes);
        }
    }
}
