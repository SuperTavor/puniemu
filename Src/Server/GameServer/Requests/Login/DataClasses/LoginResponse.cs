using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.Login.DataClasses
{
    public class LoginResponse
    {
        // Time when the res was sent out
        [JsonProperty("serverDt")]
        public long ServerDate { get; set; }

        // ywp_user property. The default is an empty list.
        [JsonProperty("ywp_user_medal_point_trade")]
        public List<object> YwpUserMedalPointTrade { get; set; }

        // Constant.
        [JsonProperty("leaderYoukaiBGM")]
        public string LeaderYoukaiBgm { get; set; }

        // ywp_user property. The default is an empty string.
        [JsonProperty("ywp_user_conflate")]
        public string YwpUserConflate { get; set; }

        // ywp_user property. The default is an empty string.
        [JsonProperty("ywp_user_treasure_series")]
        public string YwpUserTreasureSeries { get; set; }

        // ywp_user property detailing the obtained Youkai. Default is a constant.
        [JsonProperty("ywp_user_youkai")]
        public string YwpUserYoukai { get; set; }

        // ywp_user property probably detailing the amount of stars gotten on stages? Idk, default is empty list.
        [JsonProperty("ywp_user_stage_rank")]
        public List<object> YwpUserStageRank { get; set; }

        // ywp_user property. Default is an empty string.
        [JsonProperty("ywp_user_youkai_legend_release_history")]
        public string YwpUserYoukaiLegendReleaseHistory { get; set; }

        // Details if the opening tutorial should be played. This is false by default for obvious reasons.
        [JsonProperty("openingTutorialFlg")]
        public int OpeningTutorialFlag { get; set; }

        // ywp_user property detailing the completed tutorials. Initialized to a constant.
        [JsonProperty("ywp_user_tutorial_list")]
        public string YwpTutorialList { get; set; }

        // ywp_user property. Initialized to empty list.
        [JsonProperty("ywp_user_raid_boss")]
        public List<object> YwpUserRaidBoss { get; set; }

        // ywp_user property, probably detailing event progress. Default is constant (per version obviously).
        [JsonProperty("ywp_user_event_condition")]
        public List<object> YwpUserEventCond { get; set; }

        // ywp_user property probably detailing obtained items. Initialized to empty string.
        [JsonProperty("ywp_user_item")]
        public string YwpUserItem { get; set; }

        // ywp_mst property detailing the dialogues for the event. Constant (obviously).
        [JsonProperty("ywp_mst_event_tutorial_message")]
        public List<object> YwpMstEventTxt { get; set; }

        // ywp_user property probably detailing the items the user obtained from the shop. Initialized as empty string.
        [JsonProperty("ywp_user_shop_item_unlock")]
        public string YwpUserShopItemUnlock { get; set; }

        // ywp_user property represented as a table, detailing the obtained player titles (e.g. chan, kun).
        [JsonProperty("ywp_user_player_title")]
        public string YwpUserPlayerTitle { get; set; }

        // ywp_user property represented as a table, detailing the obtained player titles (e.g. chan, kun).
        [JsonProperty("ywp_user_player_icon")]
        public string YwpUserPlayerIcon { get; set; }

        // ywp_user property probably detailing the ad config. Default is constant.
        [JsonProperty("ywp_user_ads_play")]
        public List<object> YwpUserAdsPlay { get; set; }

        // Flag for if the news page should be opened immediately after logging in. Constant.
        [JsonProperty("noticePageListFlg")]
        public int NoticePageListFlag { get; set; }

        // Server assets version. Constant.
        [JsonProperty("mstVersionMaster")]
        public int MstVersionMaster { get; set; }

        // ywp_user property initialized to maybe the stages the user's friends completed. Initialized to list.
        [JsonProperty("ywp_user_friend_stage")]
        public List<object> YwpUserFriendStage { get; set; }

        // 0 here
        [JsonProperty("resultCode")]
        public int ResultCode { get; set; }

        // 0 here
        [JsonProperty("nextScreenType")]
        public int NextScreenType { get; set; }

        // Idk, should be good as 0.
        [JsonProperty("monthlyPurchasableLeft")]
        public long MonthlyPurchasableLeft { get; set; }

        // ywp_mst property. Constant.
        [JsonProperty("ywp_mst_version_master")]
        public string YwpMstVersionMaster { get; set; }

        // ywp_user property. Idk. Initialized to list.
        [JsonProperty("ywp_user_drive_progress")]
        public List<object> YwpUserDriveProgress { get; set; }

        [JsonProperty("ywp_user_data")]
        public Dictionary<string,object> UserData { get; set; }

        // Constant.
        [JsonProperty("hitodamaShopSaleList")]
        public List<int> HitodamaShopSaleList { get; set; }

        // ywp_mst property, constant.
        [JsonProperty("ywp_mst_youkai_bonus_effect_exclude")]
        public List<object> YwpMstYoukaiBonusEffectExclude { get; set; }

        // ywp_user property. I have no idea. Maybe some kinda record of the stuff you did? Idk, keeping this shi empty.
        [JsonProperty("ywp_user_hist_youkai_total")]
        public string YwpUserHistYoukaiTotal { get; set; }

        // ywp_user property. Default is empty list.
        [JsonProperty("ywp_user_mini_game_map_friend")]
        public List<object> YwpUserMiniGameMapFriend { get; set; }

        // ywp_user property. Probably details the unlocked menus. Default is constant.
        [JsonProperty("ywp_user_menufunc")]
        public string YwpUserMenuFunc { get; set; }

        // ywp_user property detailing Youkai you've seen or befriended. Default is constant.
        [JsonProperty("ywp_user_dictionary")]
        public string YwpUserDict { get; set; }

        // ywp_mst property. Constant.
        [JsonProperty("ywp_mst_event_group_assist_disp")]
        public string YwpUserEventGroupAssistDisp { get; set; }

        // ywp_user property. Idk what this is, history of some sort? Default is constant.
        [JsonProperty("ywp_user_hist_total")]
        public string YwpUserHistTotal { get; set; }

        // ywp_user property. I think it keeps track of the streak. Default is constant.
        [JsonProperty("ywp_user_hist_puzzle_daily")]
        public string YwpUserHistPuzzleDaily { get; set; }

        // Constant.
        [JsonProperty("ymoneyShopSaleList")]
        public List<object> YMoneyShopSaleList { get; set; }

        // Empty.
        [JsonProperty("ywpToken")]
        public string YwpToken { get; set; }

        // ywp_user property. Initialized to list.
        [JsonProperty("ywp_user_stage_relation_progress")]
        public List<object> YwpUserStageRelationProgress { get; set; }

        // ywp_mst property, constant.
        [JsonProperty("ywp_mst_event")]
        public List<object> YwpMstEvent { get; set; }

        // Should be empty.
        [JsonProperty("token")]
        public string Token { get; set; }

        // ywp_mst property,  constant.
        [JsonProperty("ywp_mst_event_youkai_assist_disp")]
        public string YwpMstEventYoukaiAssistDisp { get; set; }

        // ywp_user property, default is empty
        [JsonProperty("ywp_user_stage")]
        public string YwpUserStage { get; set; }

        // Empty.
        [JsonProperty("dialogMsg")]
        public string DialogMsg { get; set; }

        // ywp_user property, default is empty list.
        [JsonProperty("ywp_user_score_attack_reward")]
        public List<object> YwpUserScoreAttackReward { get; set; }

        // ywp_mst property, default is constant.
        [JsonProperty("ywp_mst_event_condition")]
        public string YwpMstEventCondition { get; set; }

        // ywp_user property, initialized as constant.
        [JsonProperty("ywp_user_youkai_deck")]
        public string YwpUserYoukaiDeck { get; set; }

        // ywp_user property with nested objects, initialized as an object.
        [JsonProperty("ywp_user_score_attack_point_trade")]
        public Dictionary<string,object> YwpUserScoreAttackPointTrade { get; set; }

        // ywp_user property, default is list with nested objects.
        [JsonProperty("ywp_user_watch")]
        public List<Dictionary<string,object>> YwpUserWatch { get; set; }

        // ywp_user property, default is an empty list.
        [JsonProperty("ywp_user_event_tutorial")]
        public List<object> YwpUserEventTutorial { get; set; }

        // ywp_user property, default is an empty list.
        [JsonProperty("ywp_user_event_ranking_reward")]
        public List<object> YwpUserEventRankingReward { get; set; }
    }
}
