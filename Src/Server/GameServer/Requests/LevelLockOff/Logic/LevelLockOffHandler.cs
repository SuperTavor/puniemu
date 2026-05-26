using Newtonsoft.Json;
using System.Text;
using System.Buffers;
using Puniemu.Src.Server.GameServer.Requests.LevelLockOff.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.TableParser.Logic;
namespace Puniemu.Src.Server.GameServer.Requests.LevelLockOff.Logic
{
    public class LevelLockOffHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<LevelLockOffRequest>(requestJsonString!)!;
            var mstYokaiRaw = (string)JsonConvert.DeserializeObject<Dictionary<string, object>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai"])["tableData"];
            var mstYokai = new TableParser<YwpMstYoukai>(mstYokaiRaw);
            var userYokai = new TableParser.Logic.TableParser<YwpUserYoukai>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID!, "ywp_user_youkai"));
            var userYokaiSkill = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID!, "ywp_user_youkai_skill");
            var userYokaiStrongSkill = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID!, "ywp_user_youkai_strong_skill");
            var userYokaiBonusEff = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID!, "ywp_user_youkai_bonus_effect");
            var userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID!, "ywp_user_data");
            var iconBudge = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_icon_budge");
            //Calculate price
            var rawLevelOpen = DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai_level_open"];
            var levelOpenDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(rawLevelOpen);
            var levelOpenData = new TableParser<YwpMstYoukaiLevelOpen>((string)levelOpenDict["tableData"]);

            //Only ymoney unlock is supported now
            var yokaiRarityType = mstYokai.Items.Where(x => x.YoukaiId == deserialized.YokaiID).First().YoukaiRarity;
            var yokaiLevel = userYokai.Items.Where(x => x.YoukaiId == deserialized.YokaiID).First().Level;
            var levelOpenEntry = levelOpenData.Items.Where(x => x.RarityType == yokaiRarityType && x.Level == yokaiLevel).First();
            var price = levelOpenEntry.YmoneyCost;
            if(userData.YMoney < price)
            {
                var errSession = new MsgBoxResponse("You don't have enough Y-Money.", "Too expensive");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                return;
            }

            Console.WriteLine($"[LevelLockOff] Calculated price: {price}");
            userData.YMoney -= price;

            //Unlock yokai
            var yokai = userYokai.Items.Where(x => x.YoukaiId == deserialized.YokaiID).First();
            yokai.IsLockedLevel = 0;

            //Construct response
            var res = new LevelLockOffResponse();
            res.UserData = userData;
            res.UserYoukai = userYokai.ToString();
            res.UserYoukaiSkill = userYokaiSkill;
            res.UserYoukaiBonusEffect = userYokaiBonusEff;
            res.UserYoukaiStrongSkill = userYokaiStrongSkill;
            res.IconBudge = iconBudge;
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_youkai", res.UserYoukai);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_data", res.UserData);
            var marshalledResponse = JsonConvert.SerializeObject(res);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
        }
    }
}
