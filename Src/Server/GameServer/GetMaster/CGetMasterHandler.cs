using Newtonsoft.Json;
using Puniemu.Src.Utils.NHNCrypt;
using Puniemu.Src.Server.GameServer.GetMaster.DataClasses;
using System.Text;
using Puniemu.Src.Utils.GeneralUtils;
using Puniemu.Src.ConfigManager;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.CodeDom;
namespace Puniemu.Src.Server.GameServer.GetMaster
{
    public class CGetMasterHandler
    {
        // To not unmarshal every time and improve performance, we store the unmarshalled versions of previously unmarshalled jsons
        private static Dictionary<string, Dictionary<string, object?>> UnmarshalCache = new();
        private const string ALL_TABLE = "ywp_mst_youkai_skill_group|ywp_mst_shop_hitodama_list|ywp_mst_dash_gear|ywp_mst_event_stage_assist|ywp_mst_event_youkai_assist|ywp_mst_gacha_youkai_choice|ywp_mst_treasure_series_reward|ywp_mst_youkai_limit|ywp_mst_game_shoot_gimmick_param|ywp_mst_youkai_group|ywp_mst_game_rule_group|ywp_mst_event_match|ywp_mst_youkai_logo|ywp_mst_youkai_boss_gimmick|ywp_mst_exchange_reward|ywp_mst_sound|ywp_mst_l5id_status_reward|ywp_mst_youkai_transform|ywp_mst_youkai_bonus_effect|ywp_mst_youkai_boss_block|ywp_mst_score_attack_point_trade|ywp_mst_rob_skill_effect|ywp_mst_steal_compatibility_bonus|ywp_mst_deck|ywp_mst_coin_purchase_age_limit|ywp_mst_stage|ywp_mst_youkai_enemy_set|ywp_mst_youkai_deck_effect|ywp_mst_crystal_effect|ywp_mst_medal_point_battle|ywp_mst_loading_message|ywp_mst_event_watch_assist|ywp_mst_event_quest|ywp_mst_event_block|ywp_mst_youkai_skill|ywp_mst_bonus_block_lot|ywp_mst_youkai_compatibility|ywp_mst_youkai_boss|ywp_mst_medal_point_trade|ywp_mst_youkai_level_open|ywp_mst_treasure|ywp_mst_steal_point|ywp_mst_event_point_buff_lv|ywp_mst_key_value|ywp_mst_event_group_assist_disp|ywp_mst_youkai_enemy_gimmick|ywp_mst_youkai_legend_release|ywp_mst_drive_point|ywp_mst_map|ywp_mst_event_youkai_assist_disp|ywp_mst_youkai_bonus_effect_level|ywp_mst_event_watch_assist_effect|ywp_mst_steal_skill_bonus|ywp_mst_event_group_assist|ywp_mst_watch_manufact|ywp_mst_production_material|ywp_mst_nyantos_block_param|ywp_mst_conflate|ywp_mst_steal_reward_set|ywp_mst_dash_gimmick|ywp_mst_group_manage|ywp_mst_shop_item_list|ywp_mst_version_resource|ywp_mst_event_friend_bonus|ywp_mst_youkai_bonus_effect_group|ywp_mst_game_rule_param|ywp_mst_ads_play|ywp_mst_mission|ywp_mst_event_quest_sub|ywp_mst_production|ywp_mst_box_reward|ywp_mst_box_reward_lot|ywp_mst_treasure_series|ywp_mst_stage_condition|ywp_mst_game_block|ywp_mst_tutorial_message|ywp_mst_steal_time_rank|ywp_mst_event_point_trade_release|ywp_mst_login_stamp_reward|ywp_mst_watch|ywp_mst_youkai_skill_group_disp|ywp_mst_coin_purchase_master|ywp_mst_youkai_enemy_torituki|ywp_mst_event_point_trade|ywp_mst_gacha|ywp_mst_item|ywp_mst_event_level|ywp_mst_youkai_skill_level|ywp_mst_youkai_level|ywp_mst_tutorial|ywp_mst_youkai_strong_skill|ywp_mst_nyantos_block|ywp_mst_shock_wave|ywp_mst_goku_effect|ywp_mst_map_mob|ywp_mst_player_title|ywp_mst_youkai_enemy_param|ywp_mst_serial_code_event|ywp_mst_tournament_continuous_win|ywp_mst_youkai|ywp_mst_player_icon|ywp_mst_youkai_transform_level|ywp_mst_event_enemy_youkai_assist|ywp_mst_event_point_buff|ywp_mst_bonus_block|ywp_mst_youkai_bonus_effect_group_disp|ywp_mst_map_group|ywp_mst_watch_effect|ywp_mst_youkai_strong_skill_level|ywp_mst_login_stamp|ywp_mst_youkai_bonus_effect_exclude";
        //Tables sometimes requested by the server that are never delivered, even on the official servers.
        private static Dictionary<string,object?> UnmarshalOrGetFromCache(string jsonName, string jsonStr)
        {
            if(!UnmarshalCache.ContainsKey(jsonName))
            {
                try
                {
                    Dictionary<string, object?> unmarshalledJson = new();
                    unmarshalledJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonStr);
                    UnmarshalCache[jsonName] = unmarshalledJson!;
                }
                catch
                {
                    Console.WriteLine(jsonName);
                }
            }
            return UnmarshalCache[jsonName];
        }

        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Response.ContentType = "application/json";  
            var encRequest = Encoding.UTF8.GetString(ctx.Request.BodyReader.ReadAsync().Result.Buffer);
            var requestJsonString = CNHNCrypt.DecryptRequest(encRequest);
            Dictionary<string, object> requestJson = JsonConvert.DeserializeObject<Dictionary<string , object>>(requestJsonString!)!;
            // Load base MasterData JSON. the base MasterData JSON contains data other than the requested tables that is shipped with the requested tables.
            var MasterDataJson = CBaseMasterDataBuilder.Build();
            //Tables contains all the requested tables
            string[] tables;
            if(!requestJson.ContainsKey("tableNames"))
            {
                await CGeneralUtils.SendBadRequest(ctx);
                return;
            }
            else
            {
                var tblNames = (string)requestJson["tableNames"];
                if(tblNames != "all")
                {
                    tables = tblNames.Split('|');
                }
                else
                {
                    tables = ALL_TABLE.Split('|');
                }
            }
            //Put all the requested tables into the base json
            foreach(var tblName in tables)
            {
                //Not sending the stuff you don't have seemingly works? I may be shooting myself in the foot with this rn
                //god knows, check back in like a month
                if(CConfigManager.GameDataManager.GamedataCache.ContainsKey(tblName))
                {
                    var selectedJsonUnmarshalled = UnmarshalOrGetFromCache(tblName, CConfigManager.GameDataManager.GamedataCache[tblName]);
                    MasterDataJson[tblName] = selectedJsonUnmarshalled;
                }
            }
            var outResponse = CNHNCrypt.EncryptResponse(JsonConvert.SerializeObject(MasterDataJson));
            await ctx.Response.WriteAsync(outResponse);

        }
    }
}
