import requests
import json
import os
from sharedLogic.NHN import *
ALL_TABLE = "ywp_mst_youkai_skill_group|ywp_mst_shop_hitodama_list|ywp_mst_dash_gear|ywp_mst_event_stage_assist|ywp_mst_event_youkai_assist|ywp_mst_gacha_youkai_choice|ywp_mst_treasure_series_reward|ywp_mst_youkai_limit|ywp_mst_game_shoot_gimmick_param|ywp_mst_youkai_group|ywp_mst_game_rule_group|ywp_mst_event_match|ywp_mst_youkai_logo|ywp_mst_youkai_boss_gimmick|ywp_mst_exchange_reward|ywp_mst_sound|ywp_mst_l5id_status_reward|ywp_mst_youkai_transform|ywp_mst_youkai_bonus_effect|ywp_mst_youkai_boss_block|ywp_mst_score_attack_point_trade|ywp_mst_rob_skill_effect|ywp_mst_steal_compatibility_bonus|ywp_mst_deck|ywp_mst_coin_purchase_age_limit|ywp_mst_stage|ywp_mst_youkai_enemy_set|ywp_mst_youkai_deck_effect|ywp_mst_crystal_effect|ywp_mst_medal_point_battle|ywp_mst_loading_message|ywp_mst_event_watch_assist|ywp_mst_event_quest|ywp_mst_event_block|ywp_mst_youkai_skill|ywp_mst_bonus_block_lot|ywp_mst_youkai_compatibility|ywp_mst_youkai_boss|ywp_mst_medal_point_trade|ywp_mst_youkai_level_open|ywp_mst_treasure|ywp_mst_steal_point|ywp_mst_event_point_buff_lv|ywp_mst_key_value|ywp_mst_event_group_assist_disp|ywp_mst_youkai_enemy_gimmick|ywp_mst_youkai_legend_release|ywp_mst_drive_point|ywp_mst_map|ywp_mst_event_youkai_assist_disp|ywp_mst_youkai_bonus_effect_level|ywp_mst_event_watch_assist_effect|ywp_mst_steal_skill_bonus|ywp_mst_event_group_assist|ywp_mst_watch_manufact|ywp_mst_production_material|ywp_mst_nyantos_block_param|ywp_mst_conflate|ywp_mst_steal_reward_set|ywp_mst_dash_gimmick|ywp_mst_group_manage|ywp_mst_shop_item_list|ywp_mst_version_resource|ywp_mst_event_friend_bonus|ywp_mst_youkai_bonus_effect_group|ywp_mst_game_rule_param|ywp_mst_ads_play|ywp_mst_mission|ywp_mst_event_quest_sub|ywp_mst_production|ywp_mst_box_reward|ywp_mst_box_reward_lot|ywp_mst_treasure_series|ywp_mst_stage_condition|ywp_mst_game_block|ywp_mst_tutorial_message|ywp_mst_steal_time_rank|ywp_mst_event_point_trade_release|ywp_mst_login_stamp_reward|ywp_mst_watch|ywp_mst_youkai_skill_group_disp|ywp_mst_coin_purchase_master|ywp_mst_youkai_enemy_torituki|ywp_mst_event_point_trade|ywp_mst_gacha|ywp_mst_item|ywp_mst_event_level|ywp_mst_youkai_skill_level|ywp_mst_youkai_level|ywp_mst_tutorial|ywp_mst_youkai_strong_skill|ywp_mst_nyantos_block|ywp_mst_shock_wave|ywp_mst_goku_effect|ywp_mst_map_mob|ywp_mst_player_title|ywp_mst_youkai_enemy_param|ywp_mst_serial_code_event|ywp_mst_tournament_continuous_win|ywp_mst_youkai|ywp_mst_player_icon|ywp_mst_youkai_transform_level|ywp_mst_event_enemy_youkai_assist|ywp_mst_event_point_buff|ywp_mst_bonus_block|ywp_mst_youkai_bonus_effect_group_disp|ywp_mst_map_group|ywp_mst_watch_effect|ywp_mst_youkai_strong_skill_level|ywp_mst_login_stamp|ywp_mst_youkai_bonus_effect_exclude"

output_folder = input("Output folder: ")
appver = input("Game version to spoof (latest version only): ").strip()

initReqPayload = ("OjJL5BoedRXqPuM3gCOvqR1imLmeSFYvVwDvp3u5KVuVhkzAxCdFdeHf4xdqLUuJ7bevsNj18QnbXTHiCCimnLZxZb6gOi1QuY2nD2DhIygns07zJf9FiQ3A_cWtZbtrYY6EqNWtHPr3Vysb2vJfsiishJR-JHGSyvkfwrnY9PbjNYslfOD-lEn05-vNnTvDFBHhFz4kMfg0k28jezdMmPz44ahgztDmZu6AwBY_CjZaD-_9qmb4Kxz-F0CcV5R-yFP9HyovAZ3FNaTERHZPHnWDHkPSzE4x65xWGfkkirzgpeEhX6JOvtFIshBXxstJsBw00RenzLMbKt85OQ8lGw==")
masterReqPayload = "PJcD6ige4Dp1qV80AOgjJjD83KLZniAgDTGQf38bRb6VhkzAxCdFdeHf4xdqLUuJ8P03_MeFYoFboqGwJ-F4024nCZVS-E8OPqK-oyk8M2Xh07cRowreRRv04u3rXByye1ORHocH4xtuUsIYT4-dVFPFBsmh5AfbwnMFd9kq7OhibdOrM0bpbgl7yyDjLe8WyycoJrFk6pu6BRSr9qrPzXPXLx6v6_Nv-jJ2E7Qc97ciBSdvtMtlLJjOrLcZZERUO2MoXQn3_IPhdM4UK5lHFsOvGeep_xcgDb36cFC4lfI="
loginReqPayload = "JEAFqqNeQ9vdvJOMnycvcPrkxTCQnk4XQQdLGy1S6mclIwFDGphL91UBB42Va_dne2H9YrVt3suUOi3ZlhvTfZq9oJgq-W6m5M4thjsIGghBCNJqiwIdGOGLhTetGv83UWi34qM6i5Tiuf4V7jw5vFRhSd2AcA26vfKPmt3fUhGDc1WCcMxZbvkvCsiWYWQYqMbJXNO09E_fIR7cltBBdY4g57YXgWS__FiiKizgnwhJazZDMR7sl-q6rDBN93DmWj7mwmL3dBB7O6DlX2GaNutZMnZEyYQLTFKGQc_WKZnoI11_Q0PcRhDlCRNiGfMMRwm_nyPErSW8UF_vljkHuz6_Gy87GIEoxTWxz1I8BnKDZ9rtVRjraD5prb-kAZBcHToJ65rsumH1yQK7jVATedhkpmZ37TgYAcwXlvzRhXL-ZQ2KDnpEdaJHEVJelqkCbH85ZVjk8y-TTe0AMyjzo5HVOwJG1AdnaQqxbcJYJl2TbHKk4o96pS4EBW-Q4OWGUy6RRXXlsq3LS0v18XzSGgsAd3KLptYZeuhMzx0IfveE5GkjTGH_DtIx9YTWxTPOlZmRDAIYOgZeqX6RY8ENPvimKxyY7OsenA3IdBkcfT9Q2Nu8kU8RvUmbSfTimEAOiTylK1K66ZT4SHgx0fs1apG73eetZLPZ2C1J7CoNyVHm5b5bxN5o-WxmaRkusapNdqqDlw9BmS1q4-e3mVB-L-BzNWFLu765c842uNCShv7WcIUQXONeNbQl0o1VKG-cpINdtdHbcSeESPLo-GRiQhRlgNC_0JHkrTaKaSOZ79seAQSqjwPrBQSSOto-vXVY8IIKwXrSxOErZy4d0dYwewKVg3Y-JMARKkd7N4WRZzdHuH2Nxcg2okeyuHweIxeU2fO_cqUVX27cwjzm8YJSiLZlv-yYznX4pZS_WubNSVb97nWFIARj8HuENEFcXas3jJgNyruSMTQO6AWwtx8vu-HKN4GwCZ8MRvT2ffiOCNGbyNm65tFCBOBcXMxniUXsf3M5AaUWThy_cEI-xawom-RqcLl_59ovshLIxLP5rIBa2vjx0M5KjZZU5ExZyErJSAy7NDnjvdzPp_9k_jopu-hIa4hl9d5NivTxMv0UnoFDmQSbRWvrJWCXSARjMCa1kEPThqwzfFChfTAPU9c15siPS5zPoNAivKDm-rkZyYY4XtJYpVBnaQXncfJym17oqB3dbGGagn9LWB68qlxnrmzcJiar4CV3Pzcws9XCjV1RhUrCefEVGR75RURIHN7rKiJUAGvTair-vXBB46upQw=="
headers = {
    "Content-Type": "application/json",
    "Accept": "application/json",
    "Accept-Encoding": "identity",
    "Connection": "Keep-Alive",
    "User-Agent": "Dalvik/2.1.0 (Linux; U; Android 9; SM-G965N Build/QP1A.190711.020)"
}

try:
    #Send message to init.nhn
    encryptedInitRes = requests.post(SERVER+"init.nhn", headers=headers, data=initReqPayload)
    init_dict = json.loads(decrypt_res(encryptedInitRes.text))

    files = {
        "ywp_mst_version_master": init_dict["ywp_mst_version_master"],
        "hitodamaShopSaleList": str(init_dict["hitodamaShopSaleList"]),
        "shopSaleList": str(init_dict["shopSaleList"]),
        "ymoneyShopSaleList": str(init_dict["ymoneyShopSaleList"]),
        "noticePageList": str(init_dict["noticePageList"]),
        "mstVersionMaster": str(init_dict["mstVersionMaster"])
    }

    #Send message to getMaster
    encryptedMasterRes = requests.post(SERVER+"getMaster.nhn",headers=headers,data=masterReqPayload)
    master_dict = json.loads(decrypt_res(encryptedMasterRes.text))
    for tbl in ALL_TABLE.split("|"):
        files[tbl] = json.dumps(master_dict[tbl])
    os.makedirs(output_folder, exist_ok=True)

    encryptedLoginRes = requests.post(SERVER+"login.nhn",headers=headers,data=loginReqPayload)
    login_dict = json.loads(decrypt_res(encryptedLoginRes.text))
    files["DefaultTutorialList"] = login_dict["ywp_user_tutorial_list"]
    for key, value in files.items():
        with open(os.path.join(output_folder, f"{key}.txt"), "w") as f:
            f.write(value.replace("'", "\""))

    print("Finished")
except Exception as e:
    print(f"An error occurred: {e}")
