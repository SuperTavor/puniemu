namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public static class Consts
    {
        public const string OG_GAMESERVER_URL = "https:\\/\\/gameserver.yw-p.com";
        public const string DEFAULT_OBTAINED_ICONS_AND_TITLES= "1*2";
        public const string ALL_TABLE = "ywp_mst_youkai_skill_group|ywp_mst_shop_hitodama_list|ywp_mst_dash_gear|ywp_mst_event_stage_assist|ywp_mst_event_youkai_assist|ywp_mst_gacha_youkai_choice|ywp_mst_treasure_series_reward|ywp_mst_youkai_limit|ywp_mst_game_shoot_gimmick_param|ywp_mst_youkai_group|ywp_mst_game_rule_group|ywp_mst_event_match|ywp_mst_youkai_logo|ywp_mst_youkai_boss_gimmick|ywp_mst_exchange_reward|ywp_mst_sound|ywp_mst_l5id_status_reward|ywp_mst_youkai_transform|ywp_mst_youkai_bonus_effect|ywp_mst_youkai_boss_block|ywp_mst_score_attack_point_trade|ywp_mst_rob_skill_effect|ywp_mst_steal_compatibility_bonus|ywp_mst_deck|ywp_mst_coin_purchase_age_limit|ywp_mst_stage|ywp_mst_youkai_enemy_set|ywp_mst_youkai_deck_effect|ywp_mst_crystal_effect|ywp_mst_medal_point_battle|ywp_mst_loading_message|ywp_mst_event_watch_assist|ywp_mst_event_quest|ywp_mst_event_block|ywp_mst_youkai_skill|ywp_mst_bonus_block_lot|ywp_mst_youkai_compatibility|ywp_mst_youkai_boss|ywp_mst_medal_point_trade|ywp_mst_youkai_level_open|ywp_mst_treasure|ywp_mst_youkai_bonus_effect_exclude|ywp_mst_myfave_message|ywp_mst_steal_point|ywp_mst_event_point_buff_lv|ywp_mst_key_value|ywp_mst_event_group_assist_disp|ywp_mst_youkai_enemy_gimmick|ywp_mst_youkai_legend_release|ywp_mst_drive_point|ywp_mst_map|ywp_mst_event_youkai_assist_disp|ywp_mst_youkai_bonus_effect_level|ywp_mst_event_watch_assist_effect|ywp_mst_steal_skill_bonus|ywp_mst_event_group_assist|ywp_mst_watch_manufact|ywp_mst_production_material|ywp_mst_nyantos_block_param|ywp_mst_conflate|ywp_mst_steal_reward_set|ywp_mst_dash_gimmick|ywp_mst_group_manage|ywp_mst_shop_item_list|ywp_mst_version_resource|ywp_mst_event_friend_bonus|ywp_mst_youkai_bonus_effect_group|ywp_mst_game_rule_param|ywp_mst_ads_play|ywp_mst_mission|ywp_mst_event_quest_sub|ywp_mst_production|ywp_mst_box_reward|ywp_mst_box_reward_lot|ywp_mst_treasure_series|ywp_mst_stage_condition|ywp_mst_game_block|ywp_mst_tutorial_message|ywp_mst_steal_time_rank|ywp_mst_event_point_trade_release|ywp_mst_login_stamp_reward|ywp_mst_watch|ywp_mst_youkai_skill_group_disp|ywp_mst_coin_purchase_master|ywp_mst_youkai_enemy_torituki|ywp_mst_event_point_trade|ywp_mst_gacha|ywp_mst_myfave_reward|ywp_mst_item|ywp_mst_event_level|ywp_mst_youkai_skill_level|ywp_mst_myfave|ywp_mst_youkai_level|ywp_mst_tutorial|ywp_mst_youkai_strong_skill|ywp_mst_nyantos_block|ywp_mst_shock_wave|ywp_mst_goku_effect|ywp_mst_map_mob|ywp_mst_player_title|ywp_mst_youkai_enemy_param|ywp_mst_serial_code_event|ywp_mst_tournament_continuous_win|ywp_mst_youkai|ywp_mst_player_icon|ywp_mst_youkai_transform_level|ywp_mst_event_enemy_youkai_assist|ywp_mst_event_point_buff|ywp_mst_bonus_block|ywp_mst_youkai_bonus_effect_group_disp|ywp_mst_map_group|ywp_mst_watch_effect|ywp_mst_youkai_strong_skill_level|ywp_mst_login_stamp";
        public static readonly List<string> LOGIN_TABLES = new List<string>
        {
            "leaderYoukaiBGM",
            "ywp_user_medal_point_trade",
            "ywp_user_conflate",
            "ywp_user_treasure_series",
            "ywp_user_youkai",
            "ywp_user_stage_rank",
            "ywp_user_youkai_legend_release_history",
            "ywp_user_tutorial_list",
            "ywp_user_raid_boss",
            "ywp_user_event_condition",
            "ywp_user_player_icon",
            "ywp_user_youkai_bonus_effect",
            "ywp_user_event",
            "ywp_user_item",
            "ywp_mst_event_tutorial_message",
            "ywp_user_hist_youkai_daily",
            "ywp_user_youkai_strong_skill",
            "ywp_user_hist_puzzle_weekly",
            "ywp_user_map",
            "ywp_user_youkai_medal_cnt",
            "ywp_user_youkai_skill",
            "ywp_user_icon_budge",
            "ywp_user_steal_progress",
            "ywp_user_mini_game_map",
            "ywp_user_event_point_trade",
            "ywp_user_data",
            "ywp_user_shop_item_remain_cnt",
            "ywp_mst_event_tutorial",
            "ywp_user_shop_item_unlock",
            "ywp_user_player_title",
            "ywp_user_ads_play",
            "ywp_user_friend_stage",
            "ywp_mst_version_master",
            "ywp_user_drive_progress",
            "ywp_mst_youkai_bonus_effect_exclude",
            "ywp_user_hist_youkai_total",
            "ywp_user_mini_game_map_friend",
            "ywp_user_menufunc",
            "ywp_user_dictionary",
            "ywp_mst_event_group_assist_disp",
            "ywp_user_hist_total",
            "ywp_user_hist_puzzle_daily",
            "ywp_user_stage_relation_progress",
            "ywp_user_treasure",
            "ywp_mst_event",
            "ywp_mst_map",
            "ywp_mst_event_youkai_assist_disp",
            "ywp_user_stage",
            "ywp_user_score_attack_reward",
            "ywp_mst_event_condition",
            "ywp_user_youkai_deck",
            "ywp_user_score_attack_point_trade",
            "ywp_user_watch",
            "ywp_user_event_tutorial",
            "ywp_user_event_ranking_reward",
        };
        public static readonly List<string> GAME_START_TABLES = new List<string>
        {
            "ywp_user_event_condition",
            "ywp_user_event",
            "ywp_user_item",
            "ywp_mst_youkai_bonus_effect_exclude",
            "ywp_user_dictionary",
            "ywp_mst_event",
            "ywp_mst_game_const",
            "ywp_mst_event_condition",
        };
        public static readonly List<string> DECK_EDIT_TABLES = new List<string>
        {
            "ywp_user_data",
            "ywp_mst_youkai_bonus_effect_exclude",
            "ywp_mst_event_youkai_assist_disp",
            "ywp_user_tutorial_list",
        };
    }
}
