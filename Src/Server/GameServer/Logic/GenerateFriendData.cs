using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.InitGacha.DataClasses;
using System.Buffers;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Logic
{
    public class FriendEntry
    {
        [JsonProperty("userId")]
        public string? UserId { get; set; }
        [JsonProperty("playerName")]
        public string? PlayerName { get; set; }
        [JsonProperty("iconId")]
        public int IconId { get; set; }
        [JsonProperty("titleId")]
        public int TitleId { get; set; }
        [JsonProperty("youkaiId")]
        public long YoukaiId { get; set; }
        [JsonProperty("lastPlayDtSentence")]
        public string? LastPlayDtSentence { get; set; }
        [JsonProperty("hitodamaSendFlg")]
        public int HitodamaSendFlg { get; set; }
        [JsonProperty("onedariSendFlg")]
        public int OnedariSendFlg { get; set; }
        [JsonProperty("mapLockSendFlg")]
        public int MapLockSendFlg { get; set; }
        [JsonProperty("characterId")]
        public string? CharacterId { get; set; }
        [JsonProperty("lastPlayDt")]
        public string? LastPlayDt { get; set; }
    }
    public class GenerateFriendData
    {
        public static string GetTimeDifferenceString(string stringDate)
        {
            DateTime givenDate = DateTime.ParseExact(stringDate, "yyyy-MM-dd HH:mm:ss", null);
            TimeSpan diff = DateTime.Now - givenDate;

            if (diff.TotalMinutes < 60)
            {
                return $"+{(int)diff.TotalMinutes} mins";
            }
            else if (diff.TotalHours < 24)
            {
                return $"+{(int)diff.TotalHours} hrs";
            }
            else
            {
                int days = (int)diff.TotalDays;
                if (days > 10)
                    days = 10;
                return $"+{days} days";
            }
        }
        public static async void RefreshYwpUserFriend(string gdkey, int TitleId, int IconId, string PlayerName, long YoukaiId, string LastPlayDt)
        {
            // update friends list
            var me = (await UserDataManager.Logic.DBService.GetYwpUserAsync<YwpUserData>(gdkey, "ywp_user_data"))!;
            var myFriendList = await UserDataManager.Logic.DBService.GetYwpUserAsync<List<FriendEntry>>(gdkey, "ywp_user_friend");
            foreach (FriendEntry item in myFriendList!)
            {
                string? targetGdkey = await UserDataManager.Logic.DBService.GetGdkeyFromUserId(item.UserId!);
                var friendFriendList = await UserDataManager.Logic.DBService.GetYwpUserAsync<List<FriendEntry>>(targetGdkey, "ywp_user_friend");
                foreach (FriendEntry item2 in friendFriendList!)
                {
                    if (item2.UserId == me.UserID)
                    {
                        if (TitleId > 0)
                            item2.TitleId = TitleId;
                        if (IconId > 0)
                            item2.IconId = IconId;
                        if (!PlayerName.IsNullOrEmpty())
                            item2.PlayerName = PlayerName;
                        if (YoukaiId > 0)
                            item2.YoukaiId = YoukaiId;
                        if (!LastPlayDt.IsNullOrEmpty())
                            item2.LastPlayDt = LastPlayDt;
                    }
                }
                await UserDataManager.Logic.DBService.SetYwpUserAsync(targetGdkey, "ywp_user_friend", friendFriendList!);
            }
        }
        public static async void RefreshYwpUserFriendRank(string gdkey, int addStars, int mode)
        {
            static void EditElement(FriendRankEntry element, int addStars, YwpUserData userData)
            {
                element.TitleId = userData.CharacterTitleID;
                element.PlayerName = userData.PlayerName;
                element.IconId = userData.IconID;
                if (addStars > 0)
                {
                    element.GetStarModiDt = new ModiDt(DateTime.Now);
                    element.GetStar += addStars;
                }
            }
            // update friends rank list
            var me = await UserDataManager.Logic.DBService.GetYwpUserAsync<YwpUserData>(gdkey, "ywp_user_data");
            List<FriendRankEntry>? myFriendRankList = null;
            string? table = null;
            if (mode == 0)
                table = "ywp_user_friend_star_rank";
            else if (mode == 1)
                table = "ywp_user_friend_rank";
            else
                throw new Exception();

            myFriendRankList = await UserDataManager.Logic.DBService.GetYwpUserAsync<List<FriendRankEntry>>(gdkey, table);
            foreach (FriendRankEntry item in myFriendRankList!)
            {
                if (item.Self == 1)
                {
                    EditElement(item, addStars, me!);
                    await UserDataManager.Logic.DBService.SetYwpUserAsync(gdkey, table, myFriendRankList!);
                    continue;
                }
                string? targetGdkey = await UserDataManager.Logic.DBService.GetGdkeyFromUserId(item.UserId!);
                var friendFriendRankList = await UserDataManager.Logic.DBService.GetYwpUserAsync<List<FriendRankEntry>>(targetGdkey, "ywp_user_friend");
                foreach (FriendRankEntry item2 in friendFriendRankList!)
                {
                    if (item2.UserId == me!.UserID)
                    {
                        EditElement(item2, addStars, me);
                    }
                }
                await UserDataManager.Logic.DBService.SetYwpUserAsync(targetGdkey, table, friendFriendRankList!);
            }
        }
    }
}
