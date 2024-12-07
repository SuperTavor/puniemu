using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Requests.GetGdkeyAccounts.DataClasses
{
    public struct UdkeyPlayerItem
    {
        [JsonProperty("iconId")]
        public PlayerIcon IconID { get; set; } // ID of the player's icon.

        [JsonProperty("playerName")]
        public string PlayerName { get; set; } // Name of the player

        [JsonProperty("youkaiId")]
        public int PartnerYokaiID { get; set; } // The center yokai in the party

        [JsonProperty("lastUpdateDate")]
        public string LastUpdateDate { get; set; } // Last time the game was played

        [JsonProperty("titleId")]
        public PlayerTitle TitleID { get; set; } // Title of the character (kun, chan, etc...)

        [JsonProperty("gdkey")]
        public string GDKey { get; set; } // GDKey

        [JsonProperty("userId")]
        public string UserID { get; set; } // UserID

        [JsonProperty("playStartDate")]
        public string StartDate { get; set; } // First time the game was played

        //because constructors cant be async
        public static async Task<UdkeyPlayerItem?> ConstructAsync(string gdkey)
        {
            YwpUserData userData;
            try
            {
                userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(gdkey, "ywp_user_data");
            }
            catch (UserDataManager.Logic.UserDataManager.TableNotFoundException)
            {
                return null;
            }

            UdkeyPlayerItem playerItem = new();

            playerItem.IconID = (PlayerIcon)userData.IconID;
            playerItem.PlayerName = userData.PlayerName;
            var UserDeck = new TableParser.Logic.TableParser((await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(gdkey, "ywp_user_youkai_deck"))!);
            playerItem.PartnerYokaiID = int.Parse(UserDeck.Table[0][1]);
            var startTimestamp = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<long>(gdkey, "start_date");
            var startTimestampString = DateTimeOffset.FromUnixTimeMilliseconds(startTimestamp).DateTime.ToString("yyyy-MM-dd HH:mm:ss");
            playerItem.StartDate = startTimestampString;
            //Placeholder as this can only be implemented when we're done with login.nhn
            playerItem.LastUpdateDate = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(gdkey, "lgn_date");
            playerItem.TitleID = (PlayerTitle)userData.CharacterTitleID;
            playerItem.GDKey = gdkey;
            playerItem.UserID = userData.UserID;
            return playerItem;
        }
    }
}
