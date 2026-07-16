using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;
using System.Buffers;
using Puniemu.Src.Server.GameServer.Requests.EvolveYoukai.DataClasses;
using Puniemu.Src.TableParser.Logic;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.Server.GameServer.Logic;
namespace Puniemu.Src.Server.GameServer.Requests.EvolveYoukai.Logic
{
    public class EvolveYoukaiHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<EvolveYoukaiRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";
            var mstYokai = new TableParser.Logic.TableParser<YwpMstYoukai>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai"]);
            var userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID, "ywp_user_data");
            var userYokai = new TableParser<YwpUserYoukai>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_youkai"));
            var userSkill = new TableParser<YwpUserYoukaiSkill>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_youkai_skill"));
            var userBonus = new TableParser<YwpUserYoukaiBonusEffect>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_youkai_bonus_effect"));
            var userDeck = new TableParser<YwpUserYoukaiDeck>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_youkai_deck"));
            var userDict = new TableParser<YwpUserDictionary>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_dictionary"));

            var userYokaiItem = userYokai.Items.FirstOrDefault(x => x.YoukaiId == deserialized.YokaiID);
            if (userYokaiItem == null)
            {
                var errRes = new MsgBoxResponse("You don't own this Yo-kai", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errRes)));
                return;
            }
            var mstItem = mstYokai.Items.FirstOrDefault(x => x.YoukaiId == deserialized.YokaiID);
            if(mstItem == null)
            {
                var errRes = new MsgBoxResponse("Yo-kai doesn't exist", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errRes)));
                return;
            }

            var canEvolve = userYokaiItem.Level >= mstItem.EvolutionLevel;
            if(!canEvolve)
            {
                var errRes = new MsgBoxResponse("Yo-kai can't evolve yet", "Error");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errRes)));
                return;
            }

            //Delete old yokai and give new one
            var oldLevel = userYokaiItem.Level;
            YoukaiManager.DeleteYoukai(userYokai, userSkill, deserialized.YokaiID, userBonus);
            DictionaryManager.EditDictionary(ref userDict, deserialized.YokaiID, true, false);
            await YoukaiManager.AddYoukai(userYokai, mstItem.EvolutionYoukaiId, userSkill, userBonus, deserialized.Level5UserID);
            DictionaryManager.EditDictionary(ref userDict, mstItem.EvolutionYoukaiId, true, true);
            var evolvedYokaiUserItem = userYokai.Items.FirstOrDefault(x => x.YoukaiId == mstItem.EvolutionYoukaiId);
            evolvedYokaiUserItem.Level = oldLevel;

            var deck = userDeck.Items[0];
            if (deck.MiddleYoukaiId == deserialized.YokaiID) deck.MiddleYoukaiId = mstItem.EvolutionYoukaiId;
            if (deck.MiddleLeftYoukaiId == deserialized.YokaiID) deck.MiddleLeftYoukaiId = mstItem.EvolutionYoukaiId;
            if (deck.MiddleRightYoukaiId == deserialized.YokaiID) deck.MiddleRightYoukaiId = mstItem.EvolutionYoukaiId;
            if (deck.FarRightYoukaiId == deserialized.YokaiID) deck.FarRightYoukaiId = mstItem.EvolutionYoukaiId;
            if (deck.FarLeftYoukaiId == deserialized.YokaiID) deck.FarLeftYoukaiId = mstItem.EvolutionYoukaiId;

            var popup = new YokaiWonPopup(mstItem.EvolutionYoukaiId, userYokai, userSkill);
            var res = new EvolveYoukaiResponse();
            res.UserYoukai = userYokai.ToString();
            res.UserData = userData;
            res.UserDeck = userDeck.ToString();
            res.UserSkill = userSkill.ToString();
            res.UserBonus = userBonus.ToString();
            res.UserDict = userDict.ToString();
            res.Popup = popup;
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_youkai", res.UserYoukai);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_youkai_skill", res.UserSkill);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_youkai_bonus_effect", res.UserBonus);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_youkai_deck", res.UserDeck);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_dictionary", res.UserDict);


            await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));

        }
    }
}
