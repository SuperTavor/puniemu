using Newtonsoft.Json;
using System.Text;
using System.Buffers;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.MissionReward.DataClasses;
using Puniemu.Src.TableParser.Logic;
using Puniemu.Src.Server.GameServer.DataClasses.Mission;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.TableParser.DataClasses;
namespace Puniemu.Src.Server.GameServer.Requests.MissionReward.Logic
{
    public class MissionRewardHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<MissionRewardRequest>(requestJsonString!);
            var userMission = new TableParser<YwpUserMission>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_mission"));
            var userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID, "ywp_user_data");
            var userItem = new TableParser<YwpUserItem>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_item"));
            var userYokai = new TableParser<YwpUserYoukai>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_youkai"));
            var userSkill = new TableParser<YwpUserYoukaiSkill>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_youkai_skill"));
            var userBonus = new TableParser<YwpUserYoukaiBonusEffect>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized.Level5UserID, "ywp_user_youkai_bonus_effect"));
            //Check if have mission
            var uMissionItem = userMission.Items.FirstOrDefault(x => x.MissionID == deserialized.MissionID);
            if (uMissionItem == null)
            {
                var err = new MsgBoxResponse("Can't find mission", "Err");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(err)));
                return;
            }

            //Check if mission we supposedly need the reward for is even complete
            if (uMissionItem.MissionCompleteStatus != MissionCompleteStatus.CompletePendingReward)
            {
                var err = new MsgBoxResponse("Can't get reward for this mission", "Err");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(err)));
                return;
            }

            //Check the reward type
            var mstMissionItem = MissionManager.GetMstMission().Items.FirstOrDefault(x => x.MissionID == deserialized.MissionID);
            if (mstMissionItem == null)
            {
                var err = new MsgBoxResponse("Can't find mission", "Err");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(err)));
                return;
            }

            var res = new MissionRewardResponse()
            {
                MstMission = MissionManager.GetMstMission().ToString(),
                UserData = userData
            };
            if(mstMissionItem.RewardType == RewardType.YMoney)
            {
                userData.YMoney += mstMissionItem.YMoneySpiritCount;
            }
            else if(mstMissionItem.RewardType == RewardType.Hitodama)
            {
                userData.Hitodama += mstMissionItem.YMoneySpiritCount;
            }
            else if(mstMissionItem.RewardType == RewardType.Item)
            {
                res.Item = new ItemWonPopup()
                {
                    ItemId = mstMissionItem.RewardID,
                    Count = 1,
                };
                ItemManager.AddItem(userItem, mstMissionItem.RewardID, 1);
            }
            else if(mstMissionItem.RewardType == RewardType.Yokai)
            {
                res.Youkai = new YokaiWonPopup(mstMissionItem.RewardID, userYokai, userSkill);
                YoukaiManager.AddYoukai(userYokai, mstMissionItem.RewardID, userSkill, userBonus);
            }
            else
            {
                var err = new MsgBoxResponse("Unsupported reward type.", "Err");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(err)));
                return;
            }

            uMissionItem.MissionCompleteStatus = MissionCompleteStatus.CompleteRewardAcquired;
            res.UserData = userData;
            res.UserYoukai = userYokai.ToString();
            res.UserSkill = userSkill.ToString();
            res.UserBonus = userBonus.ToString();
            MissionManager.TryUnlockNextMission(deserialized.MissionID, userMission);
            res.UserMission = userMission.ToString();
            res.UserItem = userItem.ToString();
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_data", userData);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_youkai", res.UserYoukai);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_youkai_skill", res.UserSkill);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_youkai_bonus_effect", res.UserBonus);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_mission", res.UserMission);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized.Level5UserID, "ywp_user_item", res.UserItem);
            await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(res)));

        }
    }
}

