using Puniemu.Src.Server.GameServer.DataClasses;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Puniemu.Src.UserDataManager.DataClasses
{
    [Table("account")]
    public class Account : BaseModel
    {
        [Column("gdkey")]
        [PrimaryKey("gdkey", false)]
        public string? Gdkey { get; set; }

        [Column("user_id")]
        public string? UserId { get; set; }

        [Column("character_id")]
        public string? CharacterId { get; set; }

        [Column("last_lgn_time")]
        public string? LastLoginTime { get; set; }

        [Column("start_date")]
        public long StartDate { get; set; }

        [Column("login_stamp")]
        public string LoginStamp { get; set; }

        [Column("opening_tutorial_flag")]
        public bool OpeningTutorialFlag { get; set; }

        // -------------------------
        // YWP user fields (theres a lot)
        // -------------------------

        [Column("ywp_user_data")]
        public YwpUserData? YwpUserData { get; set; }

        [Column("ywp_user_icon_budge")]
        public string? YwpUserIconBudge { get; set; }

        [Column("ywp_user_tutorial_list")]
        public string? YwpUserTutorialList { get; set; }

        [Column("ywp_user_menufunc")]
        public string? YwpUserMenufunc { get; set; }

        [Column("ywp_user_map")]
        public string? YwpUserMap { get; set; }

        [Column("ywp_user_hist_total")]
        public string? YwpUserHistTotal { get; set; }

        [Column("ywp_user_hist_youkai_total")]
        public string? YwpUserHistYoukaiTotal { get; set; }

        [Column("ywp_user_hist_puzzle_daily")]
        public string? YwpUserHistPuzzleDaily { get; set; }

        [Column("ywp_user_hist_puzzle_weekly")]
        public string? YwpUserHistPuzzleWeekly { get; set; }

        [Column("ywp_user_hist_youkai_daily")]
        public string? YwpUserHistYoukaiDaily { get; set; }

        [Column("ywp_user_youkai")]
        public string? YwpUserYoukai { get; set; }

        [Column("ywp_user_youkai_skill")]
        public string? YwpUserYoukaiSkill { get; set; }

        [Column("ywp_user_youkai_deck")]
        public string? YwpUserYoukaiDeck { get; set; }

        [Column("ywp_user_dictionary")]
        public string? YwpUserDictionary { get; set; }

        [Column("ywp_user_conflate")]
        public string? YwpUserConflate { get; set; }

        [Column("ywp_user_item")]
        public string? YwpUserItem { get; set; }

        [Column("ywp_user_shop_item_remain_cnt")]
        public string? YwpUserShopItemRemainCnt { get; set; }

        [Column("ywp_user_shop_item_unlock")]
        public string? YwpUserShopItemUnlock { get; set; }

        [Column("ywp_user_stage")]
        public string? YwpUserStage { get; set; }

        [Column("ywp_user_player_icon")]
        public string? YwpUserPlayerIcon { get; set; }

        [Column("ywp_user_player_title")]
        public string? YwpUserPlayerTitle { get; set; }

        [Column("ywp_user_self_rank")]
        public string? YwpUserSelfRank { get; set; }

        [Column("ywp_user_event")]
        public string? YwpUserEvent { get; set; }

        [Column("ywp_user_medal_point_trade")]
        public string? YwpUserMedalPointTrade { get; set; }

        [Column("ywp_user_youkai_medal_cnt")]
        public string? YwpUserYoukaiMedalCnt { get; set; }

        [Column("ywp_user_event_point_trade")]
        public string? YwpUserEventPointTrade { get; set; }

        [Column("ywp_user_steal_progress")]
        public string? YwpUserStealProgress { get; set; }

        [Column("ywp_user_score_attack_reward")]
        public string? YwpUserScoreAttackReward { get; set; }

        [Column("ywp_user_youkai_legend_release_history")]
        public string? YwpUserYoukaiLegendReleaseHistory { get; set; }

        [Column("ywp_user_event_tutorial")]
        public string? YwpUserEventTutorial { get; set; }

        [Column("ywp_user_raid_boss")]
        public string? YwpUserRaidBoss { get; set; }

        [Column("ywp_user_event_ranking_reward")]
        public string? YwpUserEventRankingReward { get; set; }

        [Column("ywp_user_drive_progress")]
        public string? YwpUserDriveProgress { get; set; }

        [Column("ywp_user_youkai_bonus_effect")]
        public string? YwpUserYoukaiBonusEffect { get; set; }

        [Column("ywp_user_event_progress")]
        public string? YwpUserEventProgress { get; set; }

        [Column("ywp_user_treasure_series")]
        public string? YwpUserTreasureSeries { get; set; }

        [Column("ywp_user_treasure")]
        public string? YwpUserTreasure { get; set; }

        [Column("ywp_user_present_box_list")]
        public string? YwpUserPresentBoxList { get; set; }

        [Column("ywp_user_tournament_message")]
        public string? YwpUserTournamentMessage { get; set; }

        [Column("ywp_user_friend_stage")]
        public string? YwpUserFriendStage { get; set; }

        [Column("ywp_user_all_rank")]
        public string? YwpUserAllRank { get; set; }

        [Column("ywp_user_friend_rank")]
        public string? YwpUserFriendRank { get; set; }

        [Column("ywp_user_friend_star_rank")]
        public string? YwpUserFriendStarRank { get; set; }

        [Column("ywp_user_friend_dictionary_rank")]
        public string? YwpUserFriendDictionaryRank { get; set; }

        [Column("ywp_user_stage_rank")]
        public string? YwpUserStageRank { get; set; }

        [Column("ywp_user_local_youkai_new")]
        public string? YwpUserLocalYoukaiNew { get; set; }

        [Column("ywp_user_friend")]
        public string? YwpUserFriend { get; set; }

        [Column("ywp_user_friend_request_recv")]
        public string? YwpUserFriendRequestRecv { get; set; }

        [Column("ywp_user_gacha")]
        public string? YwpUserGacha { get; set; }

        [Column("ywp_user_local_item_select")]
        public string? YwpUserLocalItemSelect { get; set; }

        [Column("ywp_user_friend_raid_boss")]
        public string? YwpUserFriendRaidBoss { get; set; }

        [Column("ywp_user_local_treasure_series_unlocked")]
        public string? YwpUserLocalTreasureSeriesUnlocked { get; set; }

        [Column("ywp_user_raid_boss_attack")]
        public string? YwpUserRaidBossAttack { get; set; }

        [Column("ywp_user_friend_rank_event")]
        public string? YwpUserFriendRankEvent { get; set; }

        [Column("ywp_user_local_stage_searched")]
        public string? YwpUserLocalStageSearched { get; set; }

        [Column("ywp_user_local_event_movie_viewed")]
        public string? YwpUserLocalEventMovieViewed { get; set; }

        [Column("ywp_user_self_rank_event")]
        public string? YwpUserSelfRankEvent { get; set; }

        [Column("ywp_user_all_rank_event")]
        public string? YwpUserAllRankEvent { get; set; }

        [Column("ywp_user_local_raid_boss_cutin_viewed")]
        public string? YwpUserLocalRaidBossCutinViewed { get; set; }

        [Column("ywp_user_local_oni_cutin_viewed")]
        public string? YwpUserLocalOniCutinViewed { get; set; }

        [Column("ywp_user_present_box")]
        public string? YwpUserPresentBox { get; set; }

        [Column("ywp_user_mission")]
        public string? YwpUserMission { get; set; }

        [Column("ywp_user_local_medal_trade_chked")]
        public string? YwpUserLocalMedalTradeChked { get; set; }

        [Column("ywp_user_local_event_trade_chked")]
        public string? YwpUserLocalEventTradeChked { get; set; }

        [Column("ywp_user_local_player_icon_select")]
        public string? YwpUserLocalPlayerIconSelect { get; set; }

        [Column("ywp_user_local_player_title_select")]
        public string? YwpUserLocalPlayerTitleSelect { get; set; }

        [Column("ywp_user_local_webpage_chked")]
        public string? YwpUserLocalWebpageChked { get; set; }

        [Column("ywp_user_local_event_help_chked")]
        public string? YwpUserLocalEventHelpChked { get; set; }

        [Column("ywp_user_local_shop_item_unlock")]
        public string? YwpUserLocalShopItemUnlock { get; set; }

        [Column("ywp_user_local_shop_item_select")]
        public string? YwpUserLocalShopItemSelect { get; set; }

        [Column("ywp_user_watch")]
        public string? YwpUserWatch { get; set; }

        [Column("ywp_user_local_watch_select")]
        public string? YwpUserLocalWatchSelect { get; set; }

        [Column("ywp_user_local_watch_unlock")]
        public string? YwpUserLocalWatchUnlock { get; set; }

        [Column("ywp_user_hitodama_buy")]
        public string? YwpUserHitodamaBuy { get; set; }

        [Column("ywp_user_login_stamp_list")]
        public string? YwpUserLoginStampList { get; set; }


        public Account()
        {
         
        }
    }
}