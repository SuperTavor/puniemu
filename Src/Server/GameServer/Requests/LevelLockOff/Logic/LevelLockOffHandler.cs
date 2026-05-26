using Newtonsoft.Json;
using System.Text;
using System.Buffers;
using Puniemu.Src.Server.GameServer.Requests.LevelLockOff.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
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

            var userYokai = new TableParser.Logic.TableParser<YwpUserYoukai>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID!, "ywp_user_youkai"));
            var userYokaiSkill = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID!, "ywp_user_youkai_skill");
            var userYokaiStrongSkill = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID!, "ywp_user_youkai_strong_skill");
            var userYokaiBonusEff = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID!, "ywp_user_youkai_bonus_effect");
            var userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID!, "ywp_user_data");

            //Pay price
            if(userData.YMoney < deserialized.LockOffCost)
            {
                var errSession = new MsgBoxResponse("You don't have enough Y-Money.", "Too expensive");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errSession)));
                return;
            }

            userData.YMoney -= deserialized.LockOffCost;

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
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_youkai", res.UserYoukai);
            var marshalledResponse = JsonConvert.SerializeObject(res);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
        }
    }
}
