import requests
import json
import os
from sharedLogic.NHN import *
ALL_TABLE = "ywp_mst_youkai_skill_group|ywp_mst_shop_hitodama_list|ywp_mst_dash_gear|ywp_mst_event_stage_assist|ywp_mst_event_youkai_assist|ywp_mst_gacha_youkai_choice|ywp_mst_treasure_series_reward|ywp_mst_youkai_limit|ywp_mst_game_shoot_gimmick_param|ywp_mst_youkai_group|ywp_mst_game_rule_group|ywp_mst_event_match|ywp_mst_youkai_logo|ywp_mst_youkai_boss_gimmick|ywp_mst_exchange_reward|ywp_mst_sound|ywp_mst_l5id_status_reward|ywp_mst_youkai_transform|ywp_mst_youkai_bonus_effect|ywp_mst_youkai_boss_block|ywp_mst_score_attack_point_trade|ywp_mst_rob_skill_effect|ywp_mst_steal_compatibility_bonus|ywp_mst_deck|ywp_mst_coin_purchase_age_limit|ywp_mst_stage|ywp_mst_youkai_enemy_set|ywp_mst_youkai_deck_effect|ywp_mst_crystal_effect|ywp_mst_medal_point_battle|ywp_mst_loading_message|ywp_mst_event_watch_assist|ywp_mst_event_quest|ywp_mst_event_block|ywp_mst_youkai_skill|ywp_mst_bonus_block_lot|ywp_mst_youkai_compatibility|ywp_mst_youkai_boss|ywp_mst_medal_point_trade|ywp_mst_youkai_level_open|ywp_mst_treasure|ywp_mst_steal_point|ywp_mst_event_point_buff_lv|ywp_mst_key_value|ywp_mst_event_group_assist_disp|ywp_mst_youkai_enemy_gimmick|ywp_mst_youkai_legend_release|ywp_mst_drive_point|ywp_mst_map|ywp_mst_event_youkai_assist_disp|ywp_mst_youkai_bonus_effect_level|ywp_mst_event_watch_assist_effect|ywp_mst_steal_skill_bonus|ywp_mst_event_group_assist|ywp_mst_watch_manufact|ywp_mst_production_material|ywp_mst_nyantos_block_param|ywp_mst_conflate|ywp_mst_steal_reward_set|ywp_mst_dash_gimmick|ywp_mst_group_manage|ywp_mst_shop_item_list|ywp_mst_version_resource|ywp_mst_event_friend_bonus|ywp_mst_youkai_bonus_effect_group|ywp_mst_game_rule_param|ywp_mst_ads_play|ywp_mst_mission|ywp_mst_event_quest_sub|ywp_mst_production|ywp_mst_box_reward|ywp_mst_box_reward_lot|ywp_mst_treasure_series|ywp_mst_stage_condition|ywp_mst_game_block|ywp_mst_tutorial_message|ywp_mst_steal_time_rank|ywp_mst_event_point_trade_release|ywp_mst_login_stamp_reward|ywp_mst_watch|ywp_mst_youkai_skill_group_disp|ywp_mst_coin_purchase_master|ywp_mst_youkai_enemy_torituki|ywp_mst_event_point_trade|ywp_mst_gacha|ywp_mst_item|ywp_mst_event_level|ywp_mst_youkai_skill_level|ywp_mst_youkai_level|ywp_mst_tutorial|ywp_mst_youkai_strong_skill|ywp_mst_nyantos_block|ywp_mst_shock_wave|ywp_mst_goku_effect|ywp_mst_map_mob|ywp_mst_player_title|ywp_mst_youkai_enemy_param|ywp_mst_serial_code_event|ywp_mst_tournament_continuous_win|ywp_mst_youkai|ywp_mst_player_icon|ywp_mst_youkai_transform_level|ywp_mst_event_enemy_youkai_assist|ywp_mst_event_point_buff|ywp_mst_bonus_block|ywp_mst_youkai_bonus_effect_group_disp|ywp_mst_map_group|ywp_mst_watch_effect|ywp_mst_youkai_strong_skill_level|ywp_mst_login_stamp|ywp_mst_youkai_bonus_effect_exclude"

output_folder = input("Output folder: ")
appver = input("Game version to spoof (latest version only): ").strip()

initReqPayload = f'{{"appVer":"{appver}","deviceId":"0","level5UserId":"0","mstVersionVer":0,"osType":2,"signature":"s4X9CoyxGma3kGuAp5woThgvBX3dCi77Slh5RcOo6ybmMTt0J4CGiZwyiCsil7P3MVgjiVt+kGE1MqvttCXLB+hlOpyTkJp5a78TXthBNVw=","userId":"0","ywpToken":"0"}}'
masterReqPayload = f'{{"appVer":"{appver}","deviceId":"d-50a6781418815598cb80657362959a0943684146efb6da8358870bef8c34d3e0","level5UserId":"0","mstVersionVer":0,"osType":2,"tableNames":"all","userId":"0","ywpToken":"0"}}'
loginReqPayload = f'{{"appVer":"{appver}","batteryInfo":{{"level":85,"state":2,"technology":"Li-ion","temperature":328,"voltage":3500}},"deviceId":"d-k3l6lh7d0jtafxq3ez5twlf5t1c4mcbwx905t6y8qwg50siubxj2yrkriq2lhf","deviceName":"z3q","gdkeySignature":"4c794e61006327c2baa5c6945439b2797a052ce923c75132ad6264a5a16c93f5","gdkeyValue":"g-h7wm3eepmnyr1pnb2w1cz9zmzpqtyls1qr02ot6hkygz0siuct92yr792q2lhq","isL5IDLinked":0,"level5UserId":"g-h7wm3eepmnyr1pnb2w1cz9zmzpqtyls1qr02ot6hkygz0siuct92yr792q2lhq","modelName":"SM-G988N","mstVersionVer":16464,"osType":2,"osVersion":"9","signNonce":"74decdf4-e86d-4228-951e-b11ccd342a4d","signTimestamp":"1724703138","signature":"s4X9CoyxGma3kGuAp5woThgvBX3dCi77Slh5RcOo6ybmMTt0J4CGiZwyiCsil7P3MVgjiVt+kGE1MqvttCXLB+hlOpyTkJp5a78TXthBNVw=","udkeySignature":"6801fe8fde40145938be564c0a79c7d45b996d4da3ac7b03fdf9b2c3e4901bd9","udkeyValue":"d-k3l6lh7d0jtafxq3ez5twlf5t1c4mcbwx905t6y8qwg50siubxj2yrkriq2lhf","userId":"3347321526146","ywpToken":"0"}}'


try:
    #Send message to init.nhn
    encryptedInitRes = requests.post(GAMESERVER+"init.nhn", headers=headers, data=encrypt_req(initReqPayload))
    init_dict = json.loads(decrypt_res(encryptedInitRes.text))

    files = {
        "ywp_mst_version_master": init_dict["ywp_mst_version_master"],
        "hitodamaShopSaleList": json.dumps(init_dict["hitodamaShopSaleList"]),
        "shopSaleList": json.dumps(init_dict["shopSaleList"]),
        "ymoneyShopSaleList": json.dumps(init_dict["ymoneyShopSaleList"]),
        "noticePageList": json.dumps(init_dict["noticePageList"]),
        "mstVersionMaster": json.dumps(init_dict["mstVersionMaster"])
    }

    #Send message to getMaster
    encryptedMasterRes = requests.post(GAMESERVER+"getMaster.nhn",headers=headers,data=encrypt_req(masterReqPayload))
    master_dict = json.loads(decrypt_res(encryptedMasterRes.text))
    for tbl in ALL_TABLE.split("|"):
        files[tbl] = json.dumps(master_dict[tbl],ensure_ascii=False)
    os.makedirs(output_folder, exist_ok=True)
    #send message to login
    encryptedLoginRes = requests.post(GAMESERVER+"login.nhn",headers=headers,data=encrypt_req(loginReqPayload))
    login_dict = json.loads(decrypt_res(encryptedLoginRes.text))
    for key in login_dict.keys():
        if "ywp_mst" in key or key == "leaderYoukaiBGM" or key == "noticePageListFlg":
            files[key] = json.dumps(login_dict[key])
            if key == "ywp_mst_event": files[key] = files[key].replace("None","null")
        elif "ywp_user" in key:
            files[key + "_def"] = json.dumps(login_dict[key])
    for key, value in files.items():
        with open(os.path.join(output_folder, f"{key}.txt"), "w",encoding="utf-8") as f:
            f.write(value.replace("'", "\""))

    print("Finished")
except Exception as e:
    print(f"An error occurred: {e}")
