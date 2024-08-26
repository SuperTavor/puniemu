using Newtonsoft.Json;
using Puniemu.src.Utils.UserDataManager;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.GetGdkeyAccounts.DataClasses
{
    public struct SUdkeyPlayerItem
    {
        [JsonProperty("iconId")]
        public ePlayerIcon IconID { get; set; } // ID of the player's icon.

        [JsonProperty("playerName")]
        public string PlayerName { get; set; } // Name of the player

        [JsonProperty("youkaiId")]
        public int PartnerYokaiID { get; set; } // The center yokai in the party

        [JsonProperty("lastUpdateDate")]
        public string LastUpdateDate { get; set; } // Last time the game was played

        [JsonProperty("titleId")]
        public ePlayerTitle TitleID { get; set; } // Title of the character (kun, chan, etc...)

        [JsonProperty("gdkey")]
        public string GDKey { get; set; } // GDKey

        [JsonProperty("userId")]
        public string UserID { get; set; } // UserID

        [JsonProperty("playStartDate")]
        public string StartDate { get; set; } // First time the game was played

        //because constructors cant be async
        public static async Task<SUdkeyPlayerItem?> ConstructAsync(string gdkey)
        {
            SYwpUserData userData = new();
            try
            {
                userData = await CUserDataManager.GetYwpUserAsync<SYwpUserData>(gdkey, "ywp_user_data");
            }
            catch(CUserDataManager.TableNotFoundException)
            {
                return null;
            }

            SUdkeyPlayerItem playerItem = new();

            playerItem.IconID = (ePlayerIcon)userData.IconID;
            playerItem.PlayerName = userData.PlayerName;
            //2235000 until we have access to this property
            playerItem.PartnerYokaiID = 2235000;
            var startTimestamp = await CUserDataManager.GetYwpUserAsync<long>(gdkey, "start_date");
            var startTimestampString = DateTimeOffset.FromUnixTimeSeconds(startTimestamp).DateTime.ToString("yyyy-MM-dd HH:mm:ss");
            playerItem.StartDate = startTimestampString;
            //Placeholder as this can only be implemented when we're done with login.nhn
            playerItem.LastUpdateDate = "1970-01-01 00:00:00";
            playerItem.TitleID = (ePlayerTitle)userData.CharacterTitleID;
            playerItem.GDKey = gdkey;
            playerItem.UserID = "0";
            return playerItem;
        }
    }
}
