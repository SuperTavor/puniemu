using Newtonsoft.Json;
using Puniemu.Src.ConfigManager;
using Puniemu.Src.Server.GameServer.DataClasses;

namespace Puniemu.Src.Server.GameServer.Init.DataClasses
{
    public struct SInitResponse
    {
        private T DeserializeGameDataToTypeAndCheckValidity<T>(string gamedataName)
        {
            T? output = JsonConvert.DeserializeObject<T>(CConfigManager.GameDataManager.GamedataCache[gamedataName]);
            if (output == null) throw new FormatException($"{gamedataName} static gamedata should be a(n) {typeof(T).FullName}");
            return output;
        }
        public SInitResponse()
        {
            this.ServerDt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            this.MstVersionMaster = int.Parse(CConfigManager.GameDataManager.GamedataCache["mstVersionMaster"]);
            this.ResultCode = 0;
            this.NextScreenType = 0;
            this.YwpMstVersionMaster = CConfigManager.GameDataManager.GamedataCache["ywp_mst_version_master"];
            this.HitodamaShopSaleList = DeserializeGameDataToTypeAndCheckValidity<List<int>>("hitodamaShopSaleList");
            this.GameServerURL = CConsts.OG_GAMESERVER_URL;
            this.StoreURL = "";
            this.IsEnableSerialCode = 1;
            this.APKey = "";
            this.ImgServer = CConfigManager.Cfg!.Value.BaseDataDownloadURL;
            this.ResultType = 0;
            this.DispNoticeFlag = 2;
            this.ShopSaleList = DeserializeGameDataToTypeAndCheckValidity<List<int>>("shopSaleList");
            this.YwpToken = "";
            this.YMoneyShopSaleList = DeserializeGameDataToTypeAndCheckValidity<List<int>>("ymoneyShopSaleList");
            this.NoticePageList = DeserializeGameDataToTypeAndCheckValidity<List<Dictionary<string,int>>>("noticePageList");
            this.IsEnableFriendInvite = 1;
            this.MasterReacquisitionHour = 2;
            this.IsEnableYokaiMedal = 1;
            this.IsEnableL5ID = 0;
            this.ThreeKingdomTeamEventButtonHiddenFlg = 1;
            this.TeamEventButtonHiddenFlg = 1;
            this.L5IDURL = "l5id";
            this.IsAppleTrial = false;
        }
        // Timestamp when the response was sent
        [JsonProperty("serverDt")]
        public long ServerDt { get; set; }

        // Version of assets on the server
        [JsonProperty("mstVersionMaster")]
        public int MstVersionMaster { get; set; }

        // 0 here
        [JsonProperty("resultCode")]
        public int ResultCode { get; set; }

        // 0 here
        [JsonProperty("nextScreenType")]
        public int NextScreenType { get; set; }

        // Constant based on version
        [JsonProperty("ywp_mst_version_master")]
        public string YwpMstVersionMaster { get; set; }

        // Constant based on version
        [JsonProperty("hitodamaShopSaleList")]
        public List<int> HitodamaShopSaleList { get; set; }

        // URL of the OG server
        [JsonProperty("gameServerUrl")]
        public string GameServerURL { get; set; }

        // URL to the Google Play store if an update is needed. Since this response is intended as a valid response, this will be empty
        [JsonProperty("storeURL")]
        public string StoreURL { get; set; }

        // 1 here
        [JsonProperty("isEnableSerialCode")]
        public int IsEnableSerialCode { get; set; }

        // Level5 ID API key. Since our API is unlimited and we don't care about the key, this can just be empty
        [JsonProperty("apkey")]
        public string APKey { get; set; }

        // URL for the data download. 
        [JsonProperty("imgServer")]
        public string ImgServer { get; set; }

        // 0 here
        [JsonProperty("resultType")]
        public int ResultType { get; set; }

        // 2 here
        [JsonProperty("dispNoticeFlag")]
        public int DispNoticeFlag { get; set; }

        // Constant based on version
        [JsonProperty("shopSaleList")]
        public List<int> ShopSaleList { get; set; }

        // Empty here
        [JsonProperty("ywpToken")]
        public string YwpToken { get; set; }

        // Constant based on version
        [JsonProperty("ymoneyShopSaleList")]
        public List<int> YMoneyShopSaleList { get; set; }

        // Constant based on version
        [JsonProperty("noticePageList")]
        public List<Dictionary<string, int>> NoticePageList { get; set; }

        //  We just put /l5id to point it to the game server's l5id path
        [JsonProperty("l5idUrl")]
        public string L5IDURL { get; set; }

        // False here
        [JsonProperty("isAppleTrial")]
        public bool IsAppleTrial { get; set; }

        // 1 here
        [JsonProperty("isEnableFriendInvite")]
        public int IsEnableFriendInvite { get; set; }

        // 2 here
        [JsonProperty("masterReacquisitionHour")]
        public int MasterReacquisitionHour { get; set; }

        // 1 here
        [JsonProperty("isEnableYoukaiMedal")]
        public int IsEnableYokaiMedal { get; set; }

        // 0 here
        [JsonProperty("isEnableL5ID")]
        public int IsEnableL5ID { get; set; }

        // 0 here
        [JsonProperty("threeKingdomTeamEventButtonHiddenFlg")]
        public int ThreeKingdomTeamEventButtonHiddenFlg { get; set; }

        // 0 here
        [JsonProperty("teamEventButtonHiddenFlg")]
        public int TeamEventButtonHiddenFlg { get; set; }

    }
}
