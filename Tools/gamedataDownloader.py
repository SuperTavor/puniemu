import requests
import json
from base64 import b64encode, b64decode
from gzip import decompress
from Cryptodome.Cipher import AES
from Cryptodome.Util.Padding import unpad, pad
import hashlib
import os
ALL_TABLE = "ywp_mst_youkai_skill_group|ywp_mst_shop_hitodama_list|ywp_mst_dash_gear|ywp_mst_event_stage_assist|ywp_mst_event_youkai_assist|ywp_mst_gacha_youkai_choice|ywp_mst_treasure_series_reward|ywp_mst_youkai_limit|ywp_mst_game_shoot_gimmick_param|ywp_mst_youkai_group|ywp_mst_game_rule_group|ywp_mst_event_match|ywp_mst_youkai_logo|ywp_mst_youkai_boss_gimmick|ywp_mst_exchange_reward|ywp_mst_sound|ywp_mst_l5id_status_reward|ywp_mst_youkai_transform|ywp_mst_youkai_bonus_effect|ywp_mst_youkai_boss_block|ywp_mst_score_attack_point_trade|ywp_mst_rob_skill_effect|ywp_mst_steal_compatibility_bonus|ywp_mst_deck|ywp_mst_coin_purchase_age_limit|ywp_mst_stage|ywp_mst_youkai_enemy_set|ywp_mst_youkai_deck_effect|ywp_mst_crystal_effect|ywp_mst_medal_point_battle|ywp_mst_loading_message|ywp_mst_event_watch_assist|ywp_mst_event_quest|ywp_mst_event_block|ywp_mst_youkai_skill|ywp_mst_bonus_block_lot|ywp_mst_youkai_compatibility|ywp_mst_youkai_boss|ywp_mst_medal_point_trade|ywp_mst_youkai_level_open|ywp_mst_treasure|ywp_mst_steal_point|ywp_mst_event_point_buff_lv|ywp_mst_key_value|ywp_mst_event_group_assist_disp|ywp_mst_youkai_enemy_gimmick|ywp_mst_youkai_legend_release|ywp_mst_drive_point|ywp_mst_map|ywp_mst_event_youkai_assist_disp|ywp_mst_youkai_bonus_effect_level|ywp_mst_event_watch_assist_effect|ywp_mst_steal_skill_bonus|ywp_mst_event_group_assist|ywp_mst_watch_manufact|ywp_mst_production_material|ywp_mst_nyantos_block_param|ywp_mst_conflate|ywp_mst_steal_reward_set|ywp_mst_dash_gimmick|ywp_mst_group_manage|ywp_mst_shop_item_list|ywp_mst_version_resource|ywp_mst_event_friend_bonus|ywp_mst_youkai_bonus_effect_group|ywp_mst_game_rule_param|ywp_mst_ads_play|ywp_mst_mission|ywp_mst_event_quest_sub|ywp_mst_production|ywp_mst_box_reward|ywp_mst_box_reward_lot|ywp_mst_treasure_series|ywp_mst_stage_condition|ywp_mst_game_block|ywp_mst_tutorial_message|ywp_mst_steal_time_rank|ywp_mst_event_point_trade_release|ywp_mst_login_stamp_reward|ywp_mst_watch|ywp_mst_youkai_skill_group_disp|ywp_mst_coin_purchase_master|ywp_mst_youkai_enemy_torituki|ywp_mst_event_point_trade|ywp_mst_gacha|ywp_mst_item|ywp_mst_event_level|ywp_mst_youkai_skill_level|ywp_mst_youkai_level|ywp_mst_tutorial|ywp_mst_youkai_strong_skill|ywp_mst_nyantos_block|ywp_mst_shock_wave|ywp_mst_goku_effect|ywp_mst_map_mob|ywp_mst_player_title|ywp_mst_youkai_enemy_param|ywp_mst_serial_code_event|ywp_mst_tournament_continuous_win|ywp_mst_youkai|ywp_mst_player_icon|ywp_mst_youkai_transform_level|ywp_mst_event_enemy_youkai_assist|ywp_mst_event_point_buff|ywp_mst_bonus_block|ywp_mst_youkai_bonus_effect_group_disp|ywp_mst_map_group|ywp_mst_watch_effect|ywp_mst_youkai_strong_skill_level|ywp_mst_login_stamp|ywp_mst_youkai_bonus_effect_exclude"
KEY = bytes([0xa8, 0x65, 0xd7, 0xe5, 0xe2, 0x45, 0x8f, 0x8c, 0xe1, 0xb5, 0xec, 0xd0, 0x87, 0xe5, 0x45, 0x94])

def decrypt_res(input_text: str) -> str:
    input_text = input_text.strip().replace('-', '+').replace('_', '/')
    input_text += "==" if not input_text.endswith("==") else ""
    decrypted = unpad(AES.new(KEY, AES.MODE_ECB).decrypt(b64decode(input_text)), AES.block_size)[20:]
    return decompress(decrypted).decode("utf-8")

def encrypt_req(to_encrypt: str) -> str:
    to_encrypt_bytes = to_encrypt.encode('utf-8')
    digest = calc_digest(to_encrypt_bytes)
    padded = pad(digest + to_encrypt_bytes, AES.block_size)
    encrypted = AES.new(KEY, AES.MODE_ECB).encrypt(padded)
    return b64encode(encrypted).decode().replace('+', '-').replace('/', '_')

def calc_digest(payload: bytes) -> bytes:
    SALT = b"0bk2kvtFE2"
    sha1 = hashlib.sha1()
    sha1.update(SALT + b' ' + payload)
    digest = sha1.digest()
    sha1.update(SALT + digest)
    return sha1.digest()

SERVER = "https://gameserver.yw-p.com/"
output_folder = input("Output folder: ")
appver = input("Game version to spoof (latest version only): ").strip()

init_request = json.dumps({
    "appVer": appver,
    "deviceId": "0",
    "level5UserId": "0",
    "mstVersionVer": 0,
    "osType": 2,
    "signature": "s4X9CoyxGma3kGuAp5woThgvBX3dCi77Slh5RcOo6ybmMTt0J4CGiZwyiCsil7P3MVgjiVt+kGE1MqvttCXLB+hlOpyTkJp5a78TXthBNVw=",
    "userId": "0",
    "ywpToken": "0"
})

initReqPayload = ("OjJL5BoedRXqPuM3gCOvqR1imLmeSFYvVwDvp3u5KVuVhkzAxCdFdeHf4xdqLUuJ7bevsNj18QnbXTHiCCimnLZxZb6gOi1QuY2nD2DhIygns07zJf9FiQ3A_cWtZbtrYY6EqNWtHPr3Vysb2vJfsiishJR-JHGSyvkfwrnY9PbjNYslfOD-lEn05-vNnTvDFBHhFz4kMfg0k28jezdMmPz44ahgztDmZu6AwBY_CjZaD-_9qmb4Kxz-F0CcV5R-yFP9HyovAZ3FNaTERHZPHnWDHkPSzE4x65xWGfkkirzgpeEhX6JOvtFIshBXxstJsBw00RenzLMbKt85OQ8lGw==")
masterReqPayload = "PJcD6ige4Dp1qV80AOgjJjD83KLZniAgDTGQf38bRb6VhkzAxCdFdeHf4xdqLUuJ8P03_MeFYoFboqGwJ-F4024nCZVS-E8OPqK-oyk8M2Xh07cRowreRRv04u3rXByye1ORHocH4xtuUsIYT4-dVFPFBsmh5AfbwnMFd9kq7OhibdOrM0bpbgl7yyDjLe8WyycoJrFk6pu6BRSr9qrPzXPXLx6v6_Nv-jJ2E7Qc97ciBSdvtMtlLJjOrLcZZERUO2MoXQn3_IPhdM4UK5lHFsOvGeep_xcgDb36cFC4lfI="
headers = {
    "Content-Type": "application/json",
    "Accept": "application/json",
    "Accept-Encoding": "identity",
    "Connection": "Keep-Alive",
    "User-Agent": "Dalvik/2.1.0 (Linux; U; Android 9; SM-G965N Build/QP1A.190711.020)"
}

try:
    encrypted_res = requests.post(SERVER+"init.nhn", headers=headers, data=initReqPayload)
    res_dict = json.loads(decrypt_res(encrypted_res.text))

    files = {
        "ywp_mst_version_master": res_dict["ywp_mst_version_master"],
        "hitodamaShopSaleList": str(res_dict["hitodamaShopSaleList"]),
        "shopSaleList": str(res_dict["shopSaleList"]),
        "ymoneyShopSaleList": str(res_dict["ymoneyShopSaleList"]),
        "noticePageList": str(res_dict["noticePageList"]),
        "mstVersionMaster": str(res_dict["mstVersionMaster"])
    }

    encryptedMasterRes = requests.post(SERVER+"getMaster.nhn",headers=headers,data=masterReqPayload)
    master_dict = json.loads(decrypt_res(encryptedMasterRes.text))
    for tbl in ALL_TABLE.split("|"):
        files[tbl] = json.dumps(master_dict[tbl])
    os.makedirs(output_folder, exist_ok=True)

    for key, value in files.items():
        with open(os.path.join(output_folder, f"{key}.txt"), "w") as f:
            f.write(value.replace("'", "\""))

    print("Finished")
except Exception as e:
    print(f"An error occurred: {e}")
