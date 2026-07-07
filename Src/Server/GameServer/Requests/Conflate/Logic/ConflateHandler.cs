using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Requests.Conflate.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.GetRanking.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using Puniemu.Src.UserDataManager.Logic;
using System.Buffers;
namespace Puniemu.Src.Server.GameServer.Requests.Conflate.Logic
{
    public class ConflateHandler
    {

        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = System.Text.Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<ConflateRequest>(requestJsonString!);
            var res = new ConflateResponse();
            var gm = DataManager.Logic.DataManager.GameDataManager;
            var mstConflateRaw = (string)JsonConvert.DeserializeObject<Dictionary<string, object>>(gm.GamedataCache["ywp_mst_conflate"])["tableData"];
            var mstConflate = new TableParser<YwpMstConflate>(mstConflateRaw);
            var userYokai = new TableParser<YwpUserYoukai>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_youkai"));
            var userSkill = new TableParser<YwpUserYoukaiSkill>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_youkai_skill"));
            var userItem = new TableParser<YwpUserItem>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_item"));
            var userDictionary = new TableParser<YwpUserDictionary>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_dictionary"));
            var userBonus = new TableParser<YwpUserYoukaiBonusEffect>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_youkai_bonus_effect"));
            res.YwpUserData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID, "ywp_user_data");

            //Find how the fusion looks
            var mstConflateItem = mstConflate.Items.FirstOrDefault(x => x.ConflateID == deserialized.ConflateID);
            if (mstConflateItem == null)
            {
                await ctx.Response.WriteAsync(JsonConvert.SerializeObject(new MsgBoxResponse("Invalid conflate", "Err")));
                return;
            }
            //Do checks
            if (res.YwpUserData.YMoney < mstConflateItem.YMoneyCost)
            {
                await ctx.Response.WriteAsync(JsonConvert.SerializeObject(new MsgBoxResponse("Not enough Y-Money", "Err")));
                return;
            }
            //delete yokai/item that is fused with
            try
            {
                if (!(CheckIfFusionObjectGood(mstConflateItem.FuseObject1Type, mstConflateItem.FuseObject1ID, userYokai, userItem) && CheckIfFusionObjectGood(mstConflateItem.FuseObject2Type, mstConflateItem.FuseObject2ID, userYokai, userItem)))
                {
                    throw new InvalidOperationException("Missing fusion components");
                }
                ApplyFusionObject(mstConflateItem.FuseObject1Type, mstConflateItem.FuseObject1ID, userYokai, userSkill, userItem);
                ApplyFusionObject(mstConflateItem.FuseObject2Type, mstConflateItem.FuseObject2ID, userYokai, userSkill, userItem);
            }
            catch(InvalidOperationException ex)
            {
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(new MsgBoxResponse(ex.Message, "Err"))));
                return;
            }
            catch(NotImplementedException ex)
            {
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(new MsgBoxResponse(ex.Message, "Err"))));
                return;
            }
            res.YwpUserData.YMoney -= mstConflateItem.YMoneyCost;
            //Befriend result yokai
            res.Youkai = new YokaiWonPopup(mstConflateItem.ResultID, userYokai, userSkill);
            await YoukaiManager.AddYoukai(userYokai, mstConflateItem.ResultID, userSkill, userBonus, deserialized.Level5UserID);
            DictionaryManager.EditDictionary(ref userDictionary, mstConflateItem.ResultID, true, true);
            //Save response and data
            res.YwpUserYoukai = userYokai.ToString();
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_youkai", res.YwpUserYoukai);
            res.YwpUserYoukaiSkill = userSkill.ToString();
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_youkai_skill", res.YwpUserYoukaiSkill);
            res.YwpUserDictionary = userDictionary.ToString();
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_dictionary", res.YwpUserDictionary);
            res.YwpUserItem = userItem.ToString();
            res.YwpUserBonusEffect = userBonus.ToString();
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_item", res.YwpUserItem);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_data", res.YwpUserData);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_youkai_bonus_effect", res.YwpUserBonusEffect);
            //add filler tables
            res.YwpUserIconBudge = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_icon_budge");
            res.YwpUserYoukaiDeck = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_youkai_deck");
            res.YwpUserMenuFunc = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_menufunc");
            await MissionManager.UpdateProgress(deserialized.Level5UserID, GameServer.DataClasses.Mission.MissionType.FuseTotalYokai, 1);
            await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));
        }

        private static bool CheckIfFusionObjectGood(
             RewardType rewardType,
             long fusionObjId,
             TableParser<YwpUserYoukai> userYokai,
             TableParser<YwpUserItem> userItem)
        {
            return rewardType switch
            {
                RewardType.Yokai => YoukaiManager.GetYoukaiIndex(userYokai, fusionObjId) != -1,
                RewardType.Item => userItem.FindIndex([fusionObjId.ToString()]) != -1,
                _ => false
            };
        }
        private static void ApplyFusionObject(RewardType rewardType, long fusionObjId, TableParser<YwpUserYoukai> userYokai, TableParser<YwpUserYoukaiSkill> userSkill, TableParser<YwpUserItem> userItem)
        { 
            switch(rewardType)
            {
                case RewardType.Yokai:
                    YoukaiManager.DeleteYoukai(userYokai, userSkill, fusionObjId);
                    break;
                case RewardType.Item:
                    ItemManager.RemoveItem(userItem, fusionObjId, 1);
                    break;
                default:
                    throw new NotImplementedException("Unknown reward type for fuse objects");
            }
        }
    }
}
