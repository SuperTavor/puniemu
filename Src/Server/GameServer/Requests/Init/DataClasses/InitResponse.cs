using Newtonsoft.Json;
using Puniemu.Src.ConfigManager.Logic;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
namespace Puniemu.Src.Server.GameServer.Requests.Init.DataClasses
{
    public class InitResponse : PuniemuResponseBase
    {
        public InitResponse()
        {
            MstVersionMaster = int.Parse(ConfigManager.Logic.ConfigManager.GameDataManager!.GamedataCache["mstVersionMaster"]);
            ResultCode = 0;
            NextScreenType = 0;
            YwpMstVersionMaster = ConfigManager.Logic.ConfigManager.GameDataManager.GamedataCache["ywp_mst_version_master"];
            GameServerURL = Consts.OG_GAMESERVER_URL;
            IsEnableSerialCode = 1;
            APKey = "";
            ImgServer = ConfigManager.Logic.ConfigManager.Cfg!.Value.BaseDataDownloadURL;
            ResultType = 0;
            DispNoticeFlag = 2;
            NoticePageList = GeneralUtils.DeserializeGameDataToTypeAndCheckValidity<List<Dictionary<string, int>>>("noticePageList");
            IsEnableFriendInvite = 1;
            MasterReacquisitionHour = 2;
            IsEnableYokaiMedal = 1;
            IsEnableL5ID = 0;
            ThreeKingdomTeamEventButtonHiddenFlg = 1;
            TeamEventButtonHiddenFlg = 1;
            L5IDURL = "l5id";
            IsAppleTrial = false;
        }

        // Constant based on version
        [JsonProperty("ywp_mst_version_master")]
        public string YwpMstVersionMaster { get; set; }

        // URL of the OG server
        [JsonProperty("gameServerUrl")]
        public string GameServerURL { get; set; }

        // 1 here
        [JsonProperty("isEnableSerialCode")]
        public int IsEnableSerialCode { get; set; }

        // Level5 ID API key. Since our API is unlimited and we don't care about the key, this can just be empty
        [JsonProperty("apkey")]
        public string APKey { get; set; }

        // URL for the data download. 
        [JsonProperty("imgServer")]
        public string ImgServer { get; set; }

        // 2 here
        [JsonProperty("dispNoticeFlag")]
        public int DispNoticeFlag { get; set; }


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
